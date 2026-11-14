using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health bar UI for any game object with a health bar. Listens to OnHealthChanged for updating
/// the health bar UI.
///
/// Attach to Health bar prefab for the respective game object. Then, drag the game object
/// into the health field and drag the health bar slider into the slider field.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private HealthComponent health;
    [SerializeField] private Slider slider;

    /// <summary>
    /// When the HealthBarUI is enabled, it listens for any updates to health
    /// </summary>
    private void OnEnable()
    {
        if (!health || !slider)
        {
            Debug.LogError("HealthBarUI is missing a HealthComponent or Slider reference", this);
            return;
        }

        health.OnHealthChanged += UpdateBar;
        UpdateBar(health.GetCurrentHealth(), health.GetMaxHealth());
    }

    /// <summary>
    /// stops the health bar from updating
    /// </summary>
    private void OnDisable()
    {
        if (!health)
        {
            return;
        }

        health.OnHealthChanged -= UpdateBar;
    }

    /// <summary>
    /// Updates the slider UI
    /// </summary>
    /// <param name="current"> current health of game object </param>
    /// <param name="max"> max health of game object </param>
    private void UpdateBar(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
