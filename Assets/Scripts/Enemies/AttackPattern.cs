using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// TODO
/// </summary>
public abstract class AttackPattern : ScriptableObject
{
    [SerializeField] protected GameObject _projectilePrefab;
    [SerializeField] protected float _windUpTime; // time before shooting after entering attack state
    [SerializeField] protected float _coolDown; // time between shots (idle state)
    [SerializeField] protected AimMode _aimMode;
    [SerializeField] protected float _trackingSensitivity;
    // I took out num projectiles because that can be easily calculated below. We can add back if wanted
    [SerializeField] protected List<int> _firingAngles = new List<int>();
    [SerializeField] protected bool _simultaneousShooting; // if the player shoot from all angles simultaneously or consecutively


    /// <summary>
    /// TODO
    /// </summary>
    public enum AimMode
    {
        AtPlayer,
        Predictive,
        FixedDirection,
        Rotating
    }

    /// <summary>
    /// How many times Fire gets called during one attack.
    /// </summary>
    /// <returns></returns>
    protected int GetFireCountPerAttack()
    {
        if (_simultaneousShooting)
        {
            // all angles fire together at the same time
            return 1;
        }
        else
        {
            // one angle per Fire call
            return _firingAngles.Count;
        }
    }

    /// <summary>
    /// Call once when the enemy enters its attack state.
    /// </summary>
    /// <param name="runtime"></param>
    public void Begin(AttackPatternRuntime runtime)
    {
        runtime.Phase = AttackPatternRuntime.AttackPhase.Windup;
        runtime.Timer = _windUpTime;
        runtime.ShotIndex = 0;

        if (_firingAngles.Count == 0)
        {
            runtime.Phase = AttackPatternRuntime.AttackPhase.Done;
        }
    }

    /// <summary>
    /// Call every frame. Returns false when the attack is finished.
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="deltaTime"></param>
    /// <returns></returns>
    public bool Tick(AttackPatternRuntime runtime, float deltaTime)
    {
        // Do nothing if attack if finished
        if (runtime.Phase == AttackPatternRuntime.AttackPhase.Done)
        {
            return false;
        }

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
    /// An abstract method for enemies to fire at the player. 
    /// Each enemy will impelement this themselves. 
    /// </summary>
    /// <param name="rt"></param>
    protected abstract void Fire(AttackPatternRuntime runtime);
}