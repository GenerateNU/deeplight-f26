using System;
using UnityEngine;

/// <summary>
/// A class that manages and delegates state behavior to StateManager and updates MetaProgression
/// fields. Lives on a persistent bootstrap object alongside StateManager and MetaProgression, in
/// a small boot scene at build index 0 - if it sat in the main menu scene instead, the
/// SetState(mainMenu) call below would reload that scene and destroy this object on a loop.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public StateManager stateManager;

    [SerializeField] private GameObject currentRoom;
    [SerializeField] private GameObject nextRoom;

    public static event Action<bool> OnRunEnded;
    public static event Action OnEnemyKilled;

    private GameState stateBeforePause;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (stateManager == null) { stateManager = GetComponent<StateManager>(); }
        if (stateManager == null) { Debug.LogError("[GameManager] StateManager must be on the same GameObject.", this); }
    }

    private void OnEnable()
    {
        StateManager.OnLoadingFinished += FinishedLoading;
    }

    private void OnDisable()
    {
        StateManager.OnLoadingFinished -= FinishedLoading;
    }

    /// <summary>
    /// Start is called before the first frame update, sets the run state to the main menu.
    /// </summary>
    void Start()
    {
        stateManager.SetState(GameState.mainMenu);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // ---------------------------------------- Methods called by other scripts ------------------------------------------ //

    /// <summary>
    /// Starts the run after pressing the Main menu play button.
    /// </summary>
    public void StartRun()
    {
        Debug.Log("Starting new run");
        stateManager.SetState(GameState.loading);
    }

    /// <summary>
    /// Starts the tutorial after pressing the tutorial button.
    /// </summary>
    public void StartTutorial()
    {
        Debug.Log("Starting tutorial");
        stateManager.SetState(GameState.tutorial);
    }

    /// <summary>
    /// Called when loading has finished.
    /// </summary>
    public void FinishedLoading()
    {
        stateManager.SetState(GameState.inPlay);
    }

    /// <summary>
    /// Records which room the player is leaving and which one they are entering.
    /// Is called by a door between rooms.
    /// </summary>
    /// <param name="from">The room the player is leaving.</param>
    /// <param name="to">The room the player is entering.</param>
    public void SetUpRoomTransition(GameObject from, GameObject to)
    {
        currentRoom = from;
        nextRoom = to;
    }

    /// <summary>
    /// A method that is called when the player dies.
    /// Should be called by the player's health script.
    /// </summary>
    public void PlayerDied()
    {
        OnRunEnded?.Invoke(false);
        stateManager.SetState(GameState.death);
    }

    /// <summary>
    /// A method that is called when the player defeats the game.
    /// Called by the final room's exit.
    /// </summary>
    public void PlayerWonGame()
    {
        OnRunEnded?.Invoke(true);
        stateManager.SetState(GameState.win);
    }

    /// <summary>
    /// A method that is called when the player dies/wins the game and goes back to the menu.
    /// </summary>
    public void ReturnToMainMenu()
    {
        stateManager.SetState(GameState.mainMenu);
    }

    /// <summary>
    /// Clears run data and restarts a brand new run.
    /// </summary>
    public void RestartRun()
    {
        stateManager.SetState(GameState.restart);
    }

    /// <summary>
    /// A method that is called when an enemy dies.
    /// </summary>
    public void EnemyKilled()
    {
        OnEnemyKilled?.Invoke();
    }

    /// <summary>
    /// A method that either pauses or unpauses the game depending on the current state.
    /// Remembers which state the pause was entered from, so unpausing returns to the tutorial
    /// rather than dropping into inPlay.
    /// </summary>
    public void TogglePause()
    {
        GameState current = stateManager.GetCurrentState();

        if (current == GameState.inPlay || current == GameState.tutorial)
        {
            stateBeforePause = current;
            stateManager.SetState(GameState.pause);
        }
        else if (current == GameState.pause)
        {
            stateManager.SetState(stateBeforePause);
        }
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Instance = null;
        OnRunEnded = null;
        OnEnemyKilled = null;
    }
#endif  
}