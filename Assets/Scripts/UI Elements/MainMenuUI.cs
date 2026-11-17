using UnityEngine;

/// <summary>
/// Wires the main menu buttons to GameManager. Lives in the MainMenu scene.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    public void OnStartGamePressed()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[MainMenuUI] No GameManager. Is the boot scene loaded?", this);
            return;
        }
        GameManager.Instance.StartRun();
    }

    public void OnTutorialPressed()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[MainMenuUI] No GameManager. Is the boot scene loaded?", this);
            return;
        }
        GameManager.Instance.StartTutorial();
    }
}
