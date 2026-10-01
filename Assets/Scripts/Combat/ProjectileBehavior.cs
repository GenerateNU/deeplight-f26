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
    private void OnTriggerEnter(Collider otherCollider)
    {
        /*
        Currently, this has no distinction between whether the collision is on a player or an enemy, as well
        as no distinction for what the projectile was shot from (player or enemy). We don't want enemy projectiles to
        damage other enemies and the player projectile to damage themselves or other players (if that were to get implemented
        down the road).
        */
        // TODO 
        // Implement a check to see if the player projectile is hitting an enemy, or an enemy projectile is hitting the player.
        if(otherCollider.TryGetComponent<HealthComponent>(out HealthComponent hc))
        {
            hc.TakeDamage(projectileDamage);
        }

        if((projectileType == "playerProjectile" && otherCollider.CompareTag("Enemy")) || ((projectileType == "enemyProjectile") && otherCollider.CompareTag("Player"))
            || otherCollider.CompareTag("Environment"))
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
