using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Health component for game object
/// </summary>
public class HealthComponent : MonoBehaviour
{
    [Header("Health Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Header("Health Bar")]
    [SerializeField] private Slider healthBar;

    /// <summary>
    /// Runs on start
    /// </summary>
    private void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if(healthBar)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

    }

    /// <summary>
    /// Subtracts damage value from the current health.
    /// Changes state to dead if health goes below 0.
    /// Updates the health bar to reflect new health value.
    /// </summary>
    /// <param name="damageVal">
    /// Damage value based on game object
    /// </param>
    public void TakeDamage(int damageVal)
    {
        int newHealth = currentHealth - damageVal;

        if(healthBar)
        {
            healthBar.value = newHealth;
        }

        if (newHealth > 0) {
            currentHealth = newHealth;
        }
        else
        {
            currentHealth = 0;
            OnDeath();
        }
    }

    /// <summary>
    /// This method represents the behavior when player is dead 
    /// </summary>
    private void OnDeath() 
    {
        Debug.Log("Health is Zero!");
    }
}
