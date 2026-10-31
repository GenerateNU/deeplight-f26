using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Data asset describing how one enemy class attacks: how long it winds up, how it aims,
/// how many projectiles leave and in what shape. Owns all the timing and aiming logic;
/// subclasses only decide how a single Fire call turns into spawned projectiles.
///
/// Tick-based. The enemy calls Begin once on entering its attack state, then Tick every
/// frame until it returns false.
/// </summary>
public abstract class AttackPattern : ScriptableObject
{
    /// <summary>
    /// AtPlayer       - aim straight at the target.
    /// Predictive     - lead the target based on its velocity and the projectile speed.
    /// FixedDirection - ignore the target, fire relative to the facing the enemy had when the attack began.
    /// Rotating       - sweep around from that starting facing at RotationSpeed degrees per second.
    /// </summary>
    public enum AimMode
    {
        AtPlayer,
        Predictive,
        FixedDirection,
        Rotating
    }

    [Header("Projectile")]
    [Tooltip("Which projectile this pattern fires. The pool buckets instances by this prefab.")]
    [SerializeField] protected GameObject _projectilePrefab;
    [Tooltip("Only used by Predictive aim to work out how far ahead to lead. Match the projectile's own speed.")]
    [SerializeField] protected float _projectileSpeed = 20f;

    [Header("Timing")]
    [Tooltip("Time after entering the attack state before the first shot. Scaled by the enemy's windup multiplier.")]
    [SerializeField] protected float _windUpTime = 0.5f;
    [Tooltip("Time between shots once firing has started.")]
    [SerializeField] protected float _coolDown = 0.2f;

    [Header("Aiming")]
    [SerializeField] protected AimMode _aimMode = AimMode.AtPlayer;
    [Tooltip("How quickly aim catches up to the target. 0 never tracks, ~5 lags noticeably, 20+ is effectively perfect.")]
    [SerializeField] protected float _trackingSensitivity = 12f;
    [Tooltip("Fraction of the windup spent tracking. 1 tracks the whole windup, 0.75 locks on for the final quarter.")]
    [SerializeField][Range(0f, 1f)] protected float _trackingWindupFraction = 1f;
    [Tooltip("Keep tracking between shots once firing has started. Off means the whole volley commits to the locked direction.")]
    [SerializeField] protected bool _trackWhileFiring = true;
    [Tooltip("Rotating aim only: degrees per second the aim sweeps.")]
    [SerializeField] protected float _rotationSpeed = 90f;

    [Header("Shape")]
    [Tooltip("Offsets in degrees from the aim direction. A single 0 entry is a straight shot.")]
    [SerializeField] protected List<int> _firingAngles = new List<int> { 0 };
    [Tooltip("Fire every angle at once, rather than one angle per shot.")]
    [SerializeField] protected bool _simultaneousShooting;
    [Tooltip("How many times the whole angle set repeats in one attack. 10 with a single angle is a rapid-fire stream.")]
    [SerializeField][Min(1)] protected int _burstCount = 1;

    /// <summary>
    /// How many times Fire gets called during one attack.
    /// </summary>
    public int GetFireCountPerAttack()
    {
        if (_firingAngles.Count == 0)
        {
            return 0;
        }

        // Simultaneous fires every angle in a single call, consecutive fires one angle per call.
        int callsPerBurst = _simultaneousShooting ? 1 : _firingAngles.Count;
        return callsPerBurst * Mathf.Max(1, _burstCount);
    }

    /// <summary>
    /// Total projectiles spawned by one full attack. Replaces the old num projectiles field.
    /// </summary>
    public int GetProjectileCountPerAttack()
    {
        return _firingAngles.Count * Mathf.Max(1, _burstCount);
    }

    /// <summary>
    /// Whether aim is currently following the target. Useful for telegraphs, e.g. a sniper
    /// laser that stops sweeping once the enemy has committed to a direction.
    /// </summary>
    public bool IsTracking(AttackPatternRuntime runtime)
    {
        if (runtime == null || runtime.Phase == AttackPatternRuntime.AttackPhase.Done)
        {
            return false;
        }

        if (_aimMode == AimMode.FixedDirection)
        {
            return false;
        }

        if (runtime.Phase == AttackPatternRuntime.AttackPhase.Firing)
        {
            return _trackWhileFiring;
        }

        return runtime.WindupProgress < _trackingWindupFraction;
    }

    /// <summary>
    /// Call once when the enemy enters its attack state.
    /// windupMultiplier lets one pattern asset feel slower or snappier per enemy class.
    /// </summary>
    public void Begin(AttackPatternRuntime runtime, float windupMultiplier = 1f)
    {
        if (runtime == null)
        {
            return;
        }

        runtime.ShotIndex = 0;
        runtime.RotationOffset = 0f;

        float windup = Mathf.Max(0f, _windUpTime * windupMultiplier);
        runtime.WindupDuration = windup;
        runtime.Timer = windup;
        runtime.Phase = AttackPatternRuntime.AttackPhase.Windup;

        // Snapshot the facing the attack is built around, so fixed and rotating patterns
        // are stable even while the body turns.
        if (runtime.Owner != null)
        {
            Vector3 forward = AttackPatternRuntime.Flatten(runtime.Owner.forward);
            if (forward != Vector3.zero)
            {
                runtime.BaseDirection = forward;
            }
        }

        // Snap aim on entry rather than lerping up from a stale direction.
        Vector3 desired = GetDesiredAimDirection(runtime);
        if (desired != Vector3.zero)
        {
            runtime.AimDirection = desired;
        }

        if (GetFireCountPerAttack() <= 0)
        {
            runtime.Phase = AttackPatternRuntime.AttackPhase.Done;
        }
    }

    /// <summary>
    /// Call every frame. Returns false when the attack is finished.
    /// </summary>
    public bool Tick(AttackPatternRuntime runtime, float deltaTime)
    {
        // Do nothing if the attack is finished
        if (runtime == null || runtime.Phase == AttackPatternRuntime.AttackPhase.Done)
        {
            return false;
        }

        // Aim keeps updating during both windup and cooldown, subject to the tracking rules.
        UpdateAim(runtime, deltaTime);

        runtime.Timer = runtime.Timer - deltaTime;

        if (runtime.Timer > 0f)
        {
            return true;
        }

        // Enemy wind-up finished, now enemy attacks
        if (runtime.Phase == AttackPatternRuntime.AttackPhase.Windup)
        {
            runtime.Phase = AttackPatternRuntime.AttackPhase.Firing;
        }

        // Shoot and count the shot
        Fire(runtime);
        runtime.ShotIndex = runtime.ShotIndex + 1;

        // Check if that was the last shot
        if (runtime.ShotIndex >= GetFireCountPerAttack())
        {
            runtime.Phase = AttackPatternRuntime.AttackPhase.Done;
            return false;
        }

        // More shots to go, wait for the cooldown
        runtime.Timer = _coolDown;
        return true;
    }

    /// <summary>
    /// Moves the runtime's aim toward where this pattern wants to be pointing.
    /// Fixed and rotating aim are set directly; target-following aim eases in at a rate
    /// set by tracking sensitivity, which is what makes an enemy lag behind a strafing player.
    /// </summary>
    protected virtual void UpdateAim(AttackPatternRuntime runtime, float deltaTime)
    {
        runtime.UpdateTargetVelocity(deltaTime);

        if (_aimMode == AimMode.Rotating)
        {
            runtime.RotationOffset = runtime.RotationOffset + (_rotationSpeed * deltaTime);
        }

        Vector3 desired = GetDesiredAimDirection(runtime);
        if (desired == Vector3.zero)
        {
            return;
        }

        // These two do not follow the target, so the tracking gate does not apply.
        if (_aimMode == AimMode.FixedDirection || _aimMode == AimMode.Rotating)
        {
            runtime.AimDirection = desired;
            return;
        }

        if (!IsTracking(runtime) || _trackingSensitivity <= 0f)
        {
            return;
        }

        // Exponential approach, so the feel is the same whatever the frame rate.
        float t = 1f - Mathf.Exp(-_trackingSensitivity * deltaTime);
        runtime.AimDirection = Vector3.Slerp(runtime.AimDirection, desired, t).normalized;
    }

    /// <summary>
    /// Where this pattern would ideally be pointing right now, before tracking lag.
    /// </summary>
    protected virtual Vector3 GetDesiredAimDirection(AttackPatternRuntime runtime)
    {
        switch (_aimMode)
        {
            case AimMode.FixedDirection:
                return runtime.BaseDirection;

            case AimMode.Rotating:
                return Quaternion.AngleAxis(runtime.RotationOffset, Vector3.up) * runtime.BaseDirection;

            case AimMode.Predictive:
                return GetPredictiveDirection(runtime);

            default:
                if (runtime.Target == null)
                {
                    return Vector3.zero;
                }

                return AttackPatternRuntime.Flatten(runtime.Target.position - runtime.SpawnPosition);
        }
    }

    /// <summary>
    /// Leads the target by roughly the projectile's travel time.
    /// </summary>
    protected Vector3 GetPredictiveDirection(AttackPatternRuntime runtime)
    {
        if (runtime.Target == null)
        {
            return Vector3.zero;
        }

        Vector3 spawn = runtime.SpawnPosition;
        Vector3 toTarget = runtime.Target.position - spawn;
        toTarget.y = 0f;

        float travelTime = toTarget.magnitude / Mathf.Max(0.01f, _projectileSpeed);
        Vector3 predicted = runtime.Target.position + (runtime.TargetVelocity * travelTime);

        return AttackPatternRuntime.Flatten(predicted - spawn);
    }

    /// <summary>
    /// Takes one projectile out of the pool (or instantiates one), places it at the fire
    /// point and points it at the aim direction rotated by angleDegrees around up.
    /// </summary>
    /// <returns>The spawned projectile, or null if nothing was available.</returns>
    protected GameObject SpawnProjectile(AttackPatternRuntime runtime, float angleDegrees)
    {
        Vector3 aim = runtime.AimDirection;

        if (aim == Vector3.zero)
        {
            aim = runtime.BaseDirection != Vector3.zero ? runtime.BaseDirection : Vector3.forward;
        }

        Vector3 direction = Quaternion.AngleAxis(angleDegrees, Vector3.up) * aim;

        if (_projectilePrefab == null)
        {
            Debug.LogWarning($"{name} has no projectile prefab assigned.", this);
            return null;
        }

        if (ObjectPool.instance == null)
        {
            Debug.LogWarning($"{name} tried to fire with no ObjectPool in the scene.", this);
            return null;
        }

        Vector3 position = runtime.SpawnPosition;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        GameObject projectile = ObjectPool.instance.GetPooledObject(_projectilePrefab);

        if (projectile == null)
        {
            return null;
        }

        projectile.transform.SetPositionAndRotation(position, rotation);

        if (!projectile.activeSelf)
        {
            projectile.SetActive(true);
        }

        OnProjectileSpawned(projectile, direction, runtime);
        Debug.Log("Enemy Shot Projectile");
        return projectile;
    }

    /// <summary>
    /// Hook for handing the projectile over to whatever script drives it. This is the one
    /// place that needs to know about the projectile component.
    /// </summary>
    protected virtual void OnProjectileSpawned(GameObject projectile, Vector3 direction, AttackPatternRuntime runtime)
    {
    }

    /// <summary>
    /// Spawns the projectiles for a single shot. Called once per cooldown step, or once
    /// per volley when shooting simultaneously.
    /// </summary>
    protected abstract void Fire(AttackPatternRuntime runtime);
}