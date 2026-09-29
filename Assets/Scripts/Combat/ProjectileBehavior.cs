using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class represents the behavior of a projectile. It deals with collision detection and destruction of projectiles.
/// </summary>
public class ProjectileBehavior : MonoBehaviour
{
    [Header ("Projectile Information")]
    
    [SerializeField] private string projectileType;

    [SerializeField] private int projectileDamage;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifespan;

    // -------------------- UNITY DEFAULTS ----------------------//

    /// <summary>
    /// Runs on start
    /// </summary>
    private void Start()
    {
        if(projectileLifespan != null)
            StartCoroutine(DestroyAfterLifespan());
    }

    // -------------------- PROJECTILE ESSENTIALS --------------------- //

    /// <summary>
    /// This method destroys a projectile during a collision with the player. If colliding with the player, will call TakeDamage on the player's 
    /// health component script. Projectile will be destroyed if shot by a player and hits an enemy, or shot by the enemy and hits a player.
    /// </summary>
    /// <param name="other"></param> Represents a collider object.
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<HealthComponent>())
        {
            other.GetComponent<HealthComponent>().TakeDamage(projectileDamage);
        }

        if((projectileType == "playerProjectile" && other.CompareTag("Enemy")) || (projectileType == "enemyProjectile") && other.CompareTag("Player")
            || other.CompareTag("Environment"))
            DestroyProjectile();
    }

    /// <summary>
    /// Destroys the projectile after its lifespand has elapsed
    /// </summary>
    private IEnumerator DestroyAfterLifespan()
    {
        yield return new WaitForSeconds(projectileLifespan);
        Destroy(gameObject);
    }

    /// <summary>
    ///  This method calls the Destroy utility method to remove the object from memory.
    /// </summary>
    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    // ---------------------- SETTERS ----------------------- //

    /// <summary>
    /// Sets the projectile type to the value input into the function
    /// </summary>
    /// <param name="type"></param> the string dictating the type of projectile
    public void SetType(string type)
    {
        projectileType = type;
    }

    /// <summary>
    /// Sets the projectile damage to the value input into the function
    /// </summary>
    /// <param name="damage"></param> the int dictating the damage of the projectile
    public void SetDamage(int damage)
    {
        projectileDamage = damage;
    }

    /// <summary>
    /// Sets the projectile speed to the value input into the function
    /// </summary>
    /// <param name="speed"></param> the float dictating the speed of the projectile
    public void SetSpeed(float speed)
    {
        projectileSpeed = speed;
    }

    /// <summary>
    /// Sets the projectile lifespan to the value input into the function
    /// </summary>
    /// <param name="lifespan"></param> the float dictating the lifespan of the projectile
    public void SetLifespan(float lifespan)
    {
        projectileLifespan = lifespan;
    }


}
