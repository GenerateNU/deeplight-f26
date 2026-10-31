using UnityEngine;

/// <summary>
/// Abstract class for any player shooting feedback
/// </summary>
public abstract class ShotFeedback : MonoBehaviour
{
    [SerializeField] private PlayerShootProjectile shooter;

    /// <summary>
    /// Subscribes to the shooter's event.
    /// </summary>
    protected void OnEnable()
    {
        shooter.OnPlayerShoot += HandleShoot;
    }

    /// <summary>
    /// unsubscribes to the shooter's event.
    /// </summary>
    protected void OnDisable()
    {
        shooter.OnPlayerShoot -= HandleShoot;
    }

    /// <summary>
    /// To be overridden by any child class to implement the behavior that happens
    /// when the player shoots
    /// </summary>
    protected abstract void HandleShoot();
}
