using System;
using UnityEngine;

/// <summary>
/// Health component for game object
/// </summary>
public class HealthComponent : MonoBehaviour
{

    public int maxHealth;
    public int currentHealth;

    private void Start()
    {
        maxHealth = 100;
        currentHealth = 0;
        onDeath();
    }

    /// <summary>
    /// Subtracts damage value from the current health.
    /// Changes state to dead if health goes below 0.
    /// </summary>
    /// <param name="damageVal">
    /// Damage value based on game object
    /// </param>
    void takeDamage(int damageVal)
    {
        int newHealth = currentHealth - damageVal;

        if (newHealth > 0) {
            currentHealth = newHealth;
        }
        else
        {
            currentHealth = 0;
            onDeath();
        }

    }


    /// <summary>
    /// States player is dead 
    /// </summary>
    void onDeath() 
    {
        Debug.Log("Player is now dead :)");
    }



}
