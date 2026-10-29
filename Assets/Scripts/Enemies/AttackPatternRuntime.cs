using UnityEngine;

/// <summary>
/// Per-enemy state for a running attack. Each enemy owns one of these so enemies never
/// share timers, since the AttackPattern itself is a ScriptableObject shared between them.
/// Holds everything the pattern needs to know about *this* enemy: who is firing, where
/// projectiles leave from, what is being aimed at, and the current aim direction.
/// </summary>
public class AttackPatternRuntime
{
    /// <summary>
    /// Windup - tracking/charging, nothing has been fired yet.
    /// Firing - at least one shot has gone out, cooling down between the rest.
    /// Done   - the cycle is finished, the enemy decides what to do next.
    /// </summary>
    public enum AttackPhase
    {
        Windup,
        Firing,
        Done
    }

    public AttackPhase Phase = AttackPhase.Done;

    /// <summary>Counts down the current windup or cooldown.</summary>
    public float Timer;

    /// <summary>How many times Fire has been called this cycle.</summary>
    public int ShotIndex;

    // --- Context, set once by the enemy ---

    /// <summary>The enemy transform doing the shooting.</summary>
    public Transform Owner;

    /// <summary>Muzzle transform. Falls back to Owner when empty.</summary>
    public Transform FirePoint;

    /// <summary>What the pattern aims at, normally the player.</summary>
    public Transform Target;

    // --- Aim state, driven by the pattern ---

    /// <summary>Current aim direction, flat on the XZ plane. Firing angles are applied on top of this.</summary>
    public Vector3 AimDirection = Vector3.forward;

    /// <summary>Direction the enemy was facing when the attack began. Used by FixedDirection and Rotating.</summary>
    public Vector3 BaseDirection = Vector3.forward;

    /// <summary>Degrees the Rotating aim mode has swept so far this cycle.</summary>
    public float RotationOffset;

    /// <summary>The windup length for this cycle, after the enemy's windup multiplier.</summary>
    public float WindupDuration;

    /// <summary>Smoothed world velocity of the target, used for predictive aim.</summary>
    public Vector3 TargetVelocity { get; private set; }

    private Vector3 _lastTargetPosition;
    private bool _hasLastTargetPosition;

    /// <summary>
    /// Called once by the enemy on Start. Safe to call again if the enemy re-targets.
    /// </summary>
    public void SetContext(Transform owner, Transform firePoint, Transform target)
    {
        Owner = owner;
        FirePoint = firePoint;
        Target = target;

        _hasLastTargetPosition = false;
        TargetVelocity = Vector3.zero;

        if (owner != null)
        {
            Vector3 forward = Flatten(owner.forward);
            if (forward != Vector3.zero)
            {
                AimDirection = forward;
                BaseDirection = forward;
            }
        }
    }

    /// <summary>
    /// Where projectiles should spawn from this frame.
    /// </summary>
    public Vector3 SpawnPosition
    {
        get
        {
            if (FirePoint != null)
            {
                return FirePoint.position;
            }

            if (Owner != null)
            {
                return Owner.position;
            }

            return Vector3.zero;
        }
    }

    /// <summary>
    /// 0 at the start of the windup, 1 at the end of it. Only meaningful while in Windup.
    /// Lets a pattern track for the first part of a windup and lock on for the rest.
    /// </summary>
    public float WindupProgress
    {
        get
        {
            if (WindupDuration <= 0f)
            {
                return 1f;
            }

            return Mathf.Clamp01((WindupDuration - Timer) / WindupDuration);
        }
    }

    /// <summary>
    /// Samples the target's movement so predictive aim has something to lead with.
    /// Lightly smoothed so a single stuttery frame does not throw the lead off.
    /// </summary>
    public void UpdateTargetVelocity(float deltaTime)
    {
        if (Target == null || deltaTime <= 0f)
        {
            TargetVelocity = Vector3.zero;
            _hasLastTargetPosition = false;
            return;
        }

        Vector3 position = Target.position;

        if (_hasLastTargetPosition)
        {
            Vector3 sample = (position - _lastTargetPosition) / deltaTime;
            TargetVelocity = Vector3.Lerp(TargetVelocity, sample, 0.35f);
        }

        _lastTargetPosition = position;
        _hasLastTargetPosition = true;
    }

    /// <summary>
    /// Flattens a vector onto the XZ plane and normalises it. Returns zero if there is
    /// nothing meaningful left after flattening.
    /// </summary>
    public static Vector3 Flatten(Vector3 vector)
    {
        vector.y = 0f;

        if (vector.sqrMagnitude < 0.000001f)
        {
            return Vector3.zero;
        }

        return vector.normalized;
    }
}