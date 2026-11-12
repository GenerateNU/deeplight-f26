using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/// <summary>
/// The loading screen. Lives on the GameManager object in the boot scene.
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private float dotInterval = 0.3f;

    private Coroutine textRoutine;

    /// <summary>
    /// Runs on enable
    /// </summary>
    private void OnEnable()
    {
        StateManager.OnStateChanged += HandleStateChanged;
        if (StateManager.Instance != null) HandleStateChanged(StateManager.Instance.GetCurrentState());
    }

    /// <summary>
    /// Runs on disable
    /// </summary>
    private void OnDisable()
    {
        StateManager.OnStateChanged -= HandleStateChanged;
    }

    /// <summary>
    /// Sets the panel active if the game state is loading. Then calls the loading text coroutine.
    private void HandleStateChanged(GameState state)
    {
        bool loading = state == GameState.loading;
        panel.SetActive(loading);

        if (loading)
        {
            if (textRoutine == null && loadingText != null)
                textRoutine = StartCoroutine(LoadingText());
        }
        else if (textRoutine != null)
        {
            StopCoroutine(textRoutine);
            textRoutine = null;
        }
    }

    /// <summary>
    /// Runs every frame
    /// </summary>
    private void Update()
    {
        if (!panel.activeSelf || StateManager.Instance == null) return;
        if (progressBar != null) progressBar.value = StateManager.Instance.LoadProgress;
    }

    /// <summary>
    /// Cycles the dots while the loading screen is up. Stopped by HandleStateChanged when the
    /// state leaves loading.
    /// </summary>
    private IEnumerator LoadingText()
    {
        string[] frames = { "Loading", "Loading .", "Loading . .", "Loading . . ." };
        int i = 0;

        while (true)
        {
            loadingText.text = frames[i];
            i = (i + 1) % frames.Length;
            yield return new WaitForSecondsRealtime(dotInterval);
        }
    }
}