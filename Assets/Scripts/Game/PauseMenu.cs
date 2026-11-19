using UnityEngine;

/// <summary>
/// Shows and hides the pause UI in response to StateManager state changes.
/// Lives on a child of the GameManager so the canvas survives scene loads.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas pauseMenuCanvas;

    [Tooltip("Child object holding the actual menu (buttons, dimmer, etc). Toggled on pause.")]
    [SerializeField] private GameObject pausePanel;

    private void Awake()
    {
        if (pauseMenuCanvas == null)
        { pauseMenuCanvas = GetComponent<Canvas>(); }
        if (pauseMenuCanvas == null)
        { Debug.LogError("[PauseMenu] needs a Canvas on this object, or one assigned in the inspector.", this); }

        if (pausePanel == null)
        { Debug.LogError("[PauseMenu] pausePanel must be assigned to a child object.", this); }

        SetVisible(false);
    }

    private void OnEnable()
    {
        StateManager.OnStateChanged += HandleStateChanged;

        if (StateManager.Instance != null)
        { HandleStateChanged(StateManager.Instance.GetCurrentState()); }
    }

    private void OnDisable()
    {
        StateManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState state)
    {
        SetVisible(state == GameState.pause);
    }

    private void SetVisible(bool visible)
    {
        if (pausePanel != null)
        { pausePanel.SetActive(visible); }
    }

    // ---------------------------------------- Button hooks ------------------------------------------ //

    /// <summary>
    /// Hook to the Resume button. Returns to whichever state the pause was entered from.
    /// </summary>
    public void OnResumePressed()
    {
        if (GameManager.Instance != null)
        { GameManager.Instance.TogglePause(); }
    }

    /// <summary>
    /// Hook to the Restart button.
    /// </summary>
    public void OnRestartPressed()
    {
        if (GameManager.Instance != null)
        { GameManager.Instance.RestartRun(); }
    }

    /// <summary>
    /// Hook to the Main Menu button.
    /// </summary>
    public void OnMainMenuPressed()
    {
        if (GameManager.Instance != null)
        { GameManager.Instance.ReturnToMainMenu(); }
    }
}
