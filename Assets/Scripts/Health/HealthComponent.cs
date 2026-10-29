using UnityEngine;
using System;

/// <summary>
/// Health API for any game object.
/// Contains a max health and current health, and an event for when either variable changes.
/// Also an event for when the game object dies, or the health <=0.
///
/// Any component that needs to update based on changes to health or when the game object dies,
/// should contain listeners to the events.
///
/// Any component that should deal damage or heal the game object can call the Damage or Heal
/// methods.
/// </summary>
public class HealthComponent : MonoBehaviour
{
    [Header("Health Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    /// <summary>
    /// Event that fires when theres any changes to currentHealth or maxHealth.
    /// </summary>
    public event Action<int, int> OnHealthChanged;

    /// <summary>
    /// Event that fires when health reaches 0.
    /// </summary>
    public event Action OnDeath;

    /// <summary>
    /// Runs on start
    /// </summary>
    private void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// for accessing the current health
    /// </summary>
    /// <returns> current health of game object </returns>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// for accessing the max health
    /// </summary>
    /// <returns> the max health of game object </returns>
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Sets a new max health.
    /// In the case that the new max health is lower than the current health,
    /// the current health is set to the max health.
    /// </summary>
    /// <param name="newMaxHealth"> The new max health value </param>
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = Mathf.Max(1, newMaxHealth);
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Subtracts damage value from the current health.
    /// Changes state to dead if health goes below 0.
    /// </summary>
    /// <param name="damageVal">
    /// Damage value based on game object
    /// </param>
    public void Damage(int damageVal)
    {
        if (currentHealth > 0 && damageVal > 0)
        {
            int newHealth = currentHealth - damageVal;

            if (newHealth > 0)
            {
                currentHealth = newHealth;
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
            else
            {
                currentHealth = 0;
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
                OnDeath?.Invoke();
            }
        }
    }

    /// <summary>
    /// Adds the Heal value to the current health.
    /// caps health at max health.
    /// </summary>
    /// <param name="healVal"> Amount to heal game object </param>
    /// <returns> The amount of health actually restored, 0 if none </returns>
    public int Heal(int healVal)
    {
        if (currentHealth < maxHealth && currentHealth > 0 && healVal > 0)
        {
            int healed = Mathf.Min(healVal, maxHealth - currentHealth);
            currentHealth += healed;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            return healed;
        }
        return 0;
    }

}
