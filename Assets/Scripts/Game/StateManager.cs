using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A class that manages the current state of the game, including transitions between menu, tutorial,
/// loading, play, room transitions, win, and death.
/// </summary>
public class StateManager : MonoBehaviour
{
    public static StateManager Instance { get; private set; }

    public static event Action<GameState> OnStateChanged;

    public static event Action OnLoadingFinished;

    public static event Action OnRunReset;

    [Header("Scene names - must match Build Settings exactly")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameplaySceneName = "Gameplay";
    [SerializeField] private string tutorialSceneName = "Tutorial";

    private GameState state = GameState.mainMenu;
    private bool hasEnteredAState;
    private Coroutine loadRoutine;

    public float LoadProgress { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Sets the game to the main menu state.
    /// </summary>
    private void MainMenuState()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        LoadSceneSingle(mainMenuSceneName);
    }

    /// <summary>   
    /// Sets the game to the pause state.
    /// </summary>
    private void PauseState()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }


    /// <summary>
    /// Sets the game to the tutorial state.
    /// </summary>
    private void TutorialState()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        LoadSceneSingle(tutorialSceneName);
    }

    /// <summary>
    /// Sets the game to the loading state.
    /// </summary>
    private void LoadingState()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        LoadProgress = 0f;

        if (loadRoutine != null)
            StopCoroutine(loadRoutine);
        loadRoutine = StartCoroutine(LoadGameplayRoutine());
    }

    private IEnumerator LoadGameplayRoutine()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(gameplaySceneName, LoadSceneMode.Single);

        if (op == null)
        {
            Debug.LogError($"[StateManager] Scene '{gameplaySceneName}' is not in Build Settings.", this);
            loadRoutine = null;
            SetState(GameState.mainMenu);
            yield break;
        }

        // Unity caps progress at 0.9 until activation, so rescale it for the progress bar.
        while (!op.isDone)
        {
            LoadProgress = Mathf.Clamp01(op.progress / 0.9f);
            yield return null;
        }

        LoadProgress = 1f;
        loadRoutine = null;
        OnLoadingFinished?.Invoke();
    }

    /// <summary>
    /// Sets the game to the in play state.
    /// </summary>
    private void InPlayState()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    /// <summary>
    /// Sets the game to the win state.
    /// </summary>
    private void WinState()
    {
        AudioListener.pause = false;
    }

    /// <summary>
    /// Handles the death state of the game.
    /// </summary>
    private void DeathState()
    {
        AudioListener.pause = false;
    }

    /// <summary>
    /// Restarts the game by reloading a new run from the beginning.
    /// </summary>
    private void RestartState()
    {
        OnRunReset?.Invoke();
        SetState(GameState.loading);
    }

    /// <summary>
    /// Gets the current state of the game.     
    /// </summary>
    public GameState GetCurrentState()
    {
        return state;
    }

    /// <summary>
    /// Sets the current state of the game to the specified new state.
    /// </summary>
    public void SetState(GameState newState)
    {
        // restart resolves immediately rather than being stored.
        if (newState == GameState.restart)
        {
            RestartState();
            return;
        }

        // Re-entering the same state would re-run its setup (and restart a scene load).
        // The flag lets the very first SetState(mainMenu) through on boot.
        if (hasEnteredAState && newState == state)
            return;

        if (hasEnteredAState && !IsTransitionAllowed(state, newState))
        {
            Debug.LogWarning($"[StateManager] Ignored illegal transition {state} -> {newState}", this);
            return;
        }

        hasEnteredAState = true;
        state = newState;

        // Add HandleStateChange(state) funtions to scripts that need reduced functionality in specific cases and
        // subscribe them to OnStateChanged.
        // This is called before the state change, so if that state change destroys the object it won't cause a bug
        // (ie. NullReferenceException).
        //
        // Where to add HandleStateChange(state) in the future:
        // - Enemy: setting the enemy state to idle or just disabling the other states on death so they don't keep attacking an empty corpse
        // - Death Screen: enabling it when the player dies (GameState.death)
        // - Pause Screen: enabling it when the game is paused (GameState.pause)
        // - Player Controller: disabling movement and action input when the win screen or death screen comes up
        OnStateChanged?.Invoke(state);

        //Switch case for each state to call corresponding method
        switch (state)
        {
            case GameState.mainMenu:
                MainMenuState();
                break;
            case GameState.pause:
                PauseState();
                break;
            case GameState.tutorial:
                TutorialState();
                break;
            case GameState.loading:
                LoadingState();
                break;
            case GameState.inPlay:
                InPlayState();
                break;
            case GameState.win:
                WinState();
                break;
            case GameState.death:
                DeathState();
                break;
        }
    }

    /// <summary>
    /// Guards against transitions that indicate a bug elsewhere, e.g. pausing from the menu or
    /// the death screen.
    /// </summary>
    private static bool IsTransitionAllowed(GameState from, GameState to)
    {
        if (to == GameState.pause)
            return from == GameState.inPlay || from == GameState.tutorial;

        if (from == GameState.pause)
            return to == GameState.inPlay || to == GameState.tutorial
                || to == GameState.mainMenu || to == GameState.loading;

        return true;
    }

    /// <summary>
    /// Loads a single scene with sceneName
    /// </summary>
    /// <param name="sceneName"> the name of the scene in the build settings that needs to be loaded
    private void LoadSceneSingle(string sceneName)
    {
        if (loadRoutine != null)
        {
            StopCoroutine(loadRoutine);
            loadRoutine = null;
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

#if UNITY_EDITOR
    // With "Enter Play Mode Options" / domain reload disabled, statics survive between play
    // sessions and stale subscribers pile up. Clear them on each run. (tbh I was getting errors that I had
    // no idea how to fix and John Claude put this here to fix it)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Instance = null;
        OnStateChanged = null;
        OnLoadingFinished = null;
        OnRunReset = null;
    }
#endif

}
