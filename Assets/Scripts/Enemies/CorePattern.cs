using UnityEngine;

/// <summary>
/// The one concrete attack pattern every enemy class is built from. All five planned
/// enemies (basic, sniper, fodder, quick shot, rapid fire) are configurations of this
/// asset, not separate scripts, so making a new enemy means making a new asset.
///
/// Create one per enemy class via Assets > Create > Enemies > Attack Patterns > Core Pattern.
/// </summary>
[CreateAssetMenu(menuName = "Enemies/Attack Patterns/Core Pattern", fileName = "CorePattern")]
public class CorePattern : AttackPattern
{
    /// <summary>
    /// Simultaneous patterns spawn every angle in one call. Consecutive patterns walk the
    /// angle list one entry per call, wrapping so burst count repeats the whole set.
    /// </summary>
    protected override void Fire(AttackPatternRuntime runtime)
    {
        if (_firingAngles.Count == 0)
        {
            return;
        }

        if (_simultaneousShooting)
        {
            for (int i = 0; i < _firingAngles.Count; i++)
            {
                SpawnProjectile(runtime, _firingAngles[i]);
            }

            return;
        }

        int angleIndex = runtime.ShotIndex % _firingAngles.Count;
        SpawnProjectile(runtime, _firingAngles[angleIndex]);
    }

    /// <summary>
    /// Wire the projectile up here once its own script is in. Everything above this point
    /// is deliberately ignorant of how projectiles move or deal damage.
    /// Example:
    ///   if (projectile.TryGetComponent(out Projectile p)) p.Launch(direction, runtime.Owner);
    /// </summary>
    protected override void OnProjectileSpawned(GameObject projectile, Vector3 direction, AttackPatternRuntime runtime)
    {
        if (projectile.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(direction * _projectileSpeed, ForceMode.VelocityChange);
        }
    }
}
