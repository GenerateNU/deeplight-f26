using UnityEngine;

/// <summary>
/// Health component for game object
/// </summary>
public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    private void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    /// <summary>
    /// Subtracts damage value from the current health.
    /// Changes state to dead if health goes below 0.
    /// </summary>
    /// <param name="damageVal">
    /// Damage value based on game object
    /// </param>
    public void TakeDamage(int damageVal)
    {
        int newHealth = currentHealth - damageVal;

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
