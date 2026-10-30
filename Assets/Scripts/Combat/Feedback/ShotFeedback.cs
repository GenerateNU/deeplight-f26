using UnityEngine;

/// <summary>
/// Abstract class for any player shooting feedback
/// </summary>
public abstract class ShotFeedback : MonoBehaviour
{
  [SerializeField] private PlayerShootProjectile shooter;

  protected void OnEnable() => shooter.OnPlayerShoot += HandleShoot;
  protected void OnDisable() => shooter.OnPlayerShoot -= HandleShoot;

  protected abstract void HandleShoot();
}