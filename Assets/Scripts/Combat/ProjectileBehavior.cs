using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class represents the behavior of a projectile. It deals with collision detection and destruction of projectiles.
/// </summary>
public class ProjectileBehavior : MonoBehaviour
{
    [Header ("Projectile Settings")]
    
    [SerializeField] private string projectileType;

    [SerializeField] private int projectileDamage;
    //[SerializeField] private float projectileSpeed;

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
}
