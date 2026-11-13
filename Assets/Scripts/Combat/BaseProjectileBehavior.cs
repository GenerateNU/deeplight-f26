using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ProjectileType
{
    Player,
    Enemy
}

public class BaseProjectileBehavior : MonoBehaviour
{
    [Header("Projectile Stats")]

    [SerializeField] private int _baseProjectileDamage = 1;
    [SerializeField] private float _baseProjectileSpeed = 5f;
    [SerializeField] private float _baseProjectileLifespan = 3f;
    [SerializeField] private int _baseProjectilePierce = 1;
    [SerializeField] private int _baseHeadshotMultiplier = 2;

    private float _currentProjectileDamage;
    private float _currentProjectileSpeed;
    private float _currentProjectileLifespan;
    private float _currentProjectilePierce;

    [Header("Projectile Settings")]

    [SerializeField] private ProjectileType _type;

    [Header("Projectile Cosmetics")]

    [SerializeField] private AudioSource _projectileExplosionSFX;

    // Internal Utilities

    private float _timer;
    private int _currentRemainingPierce;

    // ----------------------------------------- Unity Defaults -------------------------------------------- //

    /// <summary>
    /// A method that runs once per frame
    /// </summary>
    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            //plays collision/fizz out sound effect
            _projectileExplosionSFX.Play();
            DisableProjectile();
        }
    }

    /// <summary>
    /// A method that runs when this GameObject is enabled
    /// </summary>
    private void OnEnable()
    {
        SetProjectileStats();
        _timer = _currentProjectileLifespan;
        _currentRemainingPierce = _baseProjectilePierce;
        Move();
    }

    // --------------------------------------- Projectile Motion ------------------------------------------------//

    /// <Summary>
    /// Zeros the projectile's linear and angular velocity, then launches it forward with the current projectile speed
    /// </Summary>
    private void Move()
    {
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // TODO: current CorePattern attack also adds force meaning 2x force is added !!!
            rb.AddForce(transform.forward * _currentProjectileSpeed, ForceMode.VelocityChange);
        }
    }

    // --------------------------------------- Collision Behavior -----------------------------------------------//


    /// <Summary>
    /// Runs when the projectile is triggered by another collider. Checks if the object collided with is an enemy or player, and causes them to take damage
    /// depending on if the projectile came from the other. Reduces projectile's remaining pierce and disables it if pierce reaches 0.
    /// </Summary>
    private void OnTriggerEnter(Collider otherCollider)
    {
        GameObject objectHit = otherCollider.gameObject;

        HealthComponent hc = otherCollider.GetComponentInParent<HealthComponent>();

        if (hc != null)
        {
            if ((objectHit.CompareTag("Player") && _type == ProjectileType.Enemy) || ((objectHit.CompareTag("Enemy") || otherCollider.CompareTag("EnemyHead")) && _type == ProjectileType.Player))
            {
                // FIX: This truncation error could be an issue if we use weird multipliers.
                if (otherCollider.CompareTag("EnemyHead"))
                {
                    hc.Damage((int)_currentProjectileDamage * _baseHeadshotMultiplier);
                }
                else
                {
                    hc.Damage((int)_currentProjectileDamage);
                }
            }

            _currentRemainingPierce -= 1;

            if (_currentRemainingPierce <= 0)
            {
                DisableProjectile();
            }
        }
    }

    /// <summary>
    ///  This method calls the SetActive() method to disable the object from the scene.
    /// </summary>
    private void DisableProjectile()
    {
        if (gameObject.activeSelf)
        {
            ObjectPool.instance.ReturnPooledObject(gameObject);
        }
    }

    // --------------------------------------------- Projectile Stats -------------------------------------------------- //

    /// <Summary>
    /// Sets the projetile's stats if it's a player projectile based on the stat handler's current stat values. Has protections for if the stat doesn't exist in
    /// the Stat enum by setting the current value to the base value.
    /// </Summary>
    private void SetProjectileStats()
    {
        _currentProjectileDamage = _baseProjectileDamage;
        _currentProjectileSpeed = _baseProjectileSpeed;
        _currentProjectileLifespan = _baseProjectileLifespan;
        _currentProjectilePierce = _baseProjectilePierce;

        // TODO: per-enemy stat variance (e.g. stronger enemies sharing this prefab).
        // The SetOnly* setters can't do this yet: they run after OnEnable, so timer and
        // Move() have already used the defaults. Needs a single method that applies
        // overrides and re-runs the timer/pierce reset and Move(). Enemy multipliers
        // would live on AttackPatternRuntime, since the pattern asset is shared.
        if (_type == ProjectileType.Enemy)
        {
            return;
        }
        else if (FindAnyObjectByType<StatHandler>() is StatHandler statHandler)
        {
            _currentProjectileDamage = (statHandler.GetStat(Stat.AttackDamage) != 0f) ? statHandler.GetStat(Stat.AttackDamage) * _baseProjectileDamage : _baseProjectileDamage;
            _currentProjectileSpeed = (statHandler.GetStat(Stat.ProjectileSpeed) != 0f) ? statHandler.GetStat(Stat.ProjectileSpeed) * _baseProjectileSpeed : _baseProjectileSpeed;
            _currentProjectileLifespan = (statHandler.GetStat(Stat.ProjectileLifespan) != 0f) ? statHandler.GetStat(Stat.ProjectileLifespan) * _baseProjectileLifespan : _baseProjectileLifespan;
            _currentProjectilePierce = (statHandler.GetStat(Stat.ProjectilePierce) != 0f) ? statHandler.GetStat(Stat.ProjectilePierce) * _baseProjectilePierce : _baseProjectilePierce;
        }
    }

    /// <Summary>
    /// Sets only the currentProjectileDamage to a specific value
    /// </Summary>
    public void SetOnlyDamage(int damage)
    {
        _currentProjectileDamage = damage;
    }

    /// <Summary>
    /// Sets only the currentProjectileSpeed to a specific value
    /// </Summary>
    public void SetOnlySpeed(float speed)
    {
        _currentProjectileSpeed = speed;
    }

    /// <Summary>
    /// Sets only the currentProjectileLifespan to a specific value
    /// </Summary>
    public void SetOnlyLifespan(float lifespan)
    {
        _currentProjectileLifespan = lifespan;
    }

    /// <Summary>
    /// Sets only the currentProjectilePierce to a specific value
    /// </Summary>
    public void SetOnlyPierce(int pierce)
    {
        _currentProjectilePierce = pierce;
    }
}
