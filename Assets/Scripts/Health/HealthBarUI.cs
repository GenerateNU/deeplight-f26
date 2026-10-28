using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    /// <summary>
    /// Health bar UI for player. Listens to OnHealthChanged for updating
    /// the health bar UI.
    /// </summary>
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
