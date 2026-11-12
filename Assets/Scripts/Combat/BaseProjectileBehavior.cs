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

    [SerializeField] private int baseProjectileDamage = 1;
    [SerializeField] private float baseProjectileSpeed = 5f;
    [SerializeField] private float baseProjectileLifespan = 3f;
    [SerializeField] private int baseProjectilePierce = 1;

    private int currentProjectileDamage;
    private float currentProjectileSpeed;
    private float currentProjectileLifespan;
    private float currentProjectilePierce;

    [Header("Projectile Settings")]

    [SerializeField] private ProjectileType type;

    [Header("Projectile Cosmetics")]

    [SerializeField] private AudioSource projectileExplosionSFX;

    // Internal Utilities

    private float timer;
    private int currentRemainingPierce;

    // ----------------------------------------- Unity Defaults -------------------------------------------- //

    /// <summary>
    /// A method that runs once per frame
    /// </summary>
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            //plays collision/fizz out sound effect
            projectileExplosionSFX.Play();
            DisableProjectile();
        }
    }

    /// <summary>
    /// A method that runs when this GameObject is enabled
    /// </summary>
    private void OnEnable()
    {
        SetProjectileStats();
        timer = currentProjectileLifespan;
        currentRemainingPierce = baseProjectilePierce;
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
            rb.AddForce(transform.forward * currentProjectileSpeed, ForceMode.VelocityChange);
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

        if (objectHit.TryGetComponent<HealthComponent>(out HealthComponent hc))
        {
            if ((objectHit.CompareTag("Player") && type == ProjectileType.Enemy) || (objectHit.CompareTag("Enemy") && type == ProjectileType.Player))
            {
                hc.Damage(currentProjectileDamage);
            }

            currentRemainingPierce -= 1;

            if (currentRemainingPierce <= 0)
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
        currentProjectileDamage = baseProjectileDamage;
        currentProjectileSpeed = baseProjectileSpeed;
        currentProjectileLifespan = baseProjectileLifespan;
        currentProjectilePierce = baseProjectilePierce;

        // TODO: per-enemy stat variance (e.g. stronger enemies sharing this prefab).
        // The SetOnly* setters can't do this yet: they run after OnEnable, so timer and
        // Move() have already used the defaults. Needs a single method that applies
        // overrides and re-runs the timer/pierce reset and Move(). Enemy multipliers
        // would live on AttackPatternRuntime, since the pattern asset is shared.
        if (type == ProjectileType.Enemy)
        {
            return;
        }
        else if (GameObject.Find("Stat Handler").TryGetComponent<StatHandler>(out StatHandler statHandler))
        {
            currentProjectileDamage = (statHandler.GetStat(AttackDamage) != 0f) ? statHandler.GetStat(AttackDamage) * baseProjectileDamage : baseProjectileDamage;
            currentProjectileSpeed = (statHandler.GetStat(ProjectileSpeed) != 0f) ? statHandler.GetStat(ProjectileSpeed) * baseProjectileSpeed : baseProjectileSpeed;
            currentProjectileLifespan = (statHandler.GetStat(ProjectileLifespan) != 0f) ? statHandler.GetStat(ProjectileLifespan) * baseProjectileLifespan : baseProjectileLifespan;
            currentProjectilePierce = (statHandler.GetStat(ProjectilePierce) != 0f) ? statHandler.GetStat(ProjectilePierce) * baseProjectilePierce : baseProjectilePierce;
        }
    }

    /// <Summary>
    /// Sets only the currentProjectileDamage to a specific value
    /// </Summary>
    public void SetOnlyDamage(int damage)
    {
        currentProjectileDamage = damage;
    }

    /// <Summary>
    /// Sets only the currentProjectileSpeed to a specific value
    /// </Summary>
    public void SetOnlySpeed(float speed)
    {
        currentProjectileSpeed = speed;
    }

    /// <Summary>
    /// Sets only the currentProjectileLifespan to a specific value
    /// </Summary>
    public void SetOnlyLifespan(float lifespan)
    {
        currentProjectileLifespan = lifespan;
    }

    /// <Summary>
    /// Sets only the currentProjectilePierce to a specific value
    /// </Summary>
    public void SetOnlyPierce(int pierce)
    {
        currentProjectilePierce = pierce;
    }
}
