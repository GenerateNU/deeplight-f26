using UnityEngine;

/// <summary>
/// Destroys this game object when its HealthComponent fires OnDeath.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
public class DestroyOnDeath : MonoBehaviour
{
    private HealthComponent health;

    private void Awake()
    {
        health = GetComponent<HealthComponent>();
    }
    private void OnEnable()
    {
        health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    /// <summary>
    /// Destroys the game object when health reaches zero.
    /// </summary>
    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}
