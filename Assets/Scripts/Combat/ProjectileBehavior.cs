using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class represents the behavior of a projectile. It deals with collision detection and 
/// returning projectiles to the pool.
/// </summary>
public class ProjectileBehavior : MonoBehaviour
{
    [Header ("Projectile Settings")]
    
    [SerializeField] private string projectileType;

    [SerializeField] private int projectileDamage = 1;
    //[SerializeField] private float projectileSpeed;

    [SerializeField] private float duration = 3f;

    private float timer;

    //SFX and particles
    public AudioSource projectileExplodeSFX;

    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.x);

        GameObject closestEnemy = null;
        float shortestDistToProjectile = 100f;

        foreach (Collider collider in colliders)
        {
            float collDistToProjectile = Vector3.Distance(transform.position, collider.gameObject.transform.position);
            Debug.Log("Collided with: " + collider.name);

            if (collider.gameObject.CompareTag("Enemy"))
            {
                if (closestEnemy == null)
                {
                    closestEnemy = collider.gameObject;
                    shortestDistToProjectile = Vector3.Distance(transform.position, closestEnemy.transform.position);
                }
                else if (collDistToProjectile < shortestDistToProjectile)
                {
                    closestEnemy = collider.gameObject;
                    shortestDistToProjectile = collDistToProjectile;
                }
            }
        }

        if(closestEnemy.TryGetComponent<HealthComponent>(out HealthComponent hc))
        {
            hc.TakeDamage(projectileDamage);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            //plays collision/fizz out sound effect 
            projectileExplodeSFX.Play();
            DisableProjectile();  
        }
    }

    /// <summary>
    /// A method that runs when this GameObject is enabled 
    /// </summary>
    private void OnEnable()
    {
        timer = duration;
    }

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
            //plays collision/fizz out sound effect 
            projectileExplodeSFX.Play();
            DisableProjectile();  
        }

        if((projectileType == "playerProjectile" && otherCollider.CompareTag("Enemy")) || ((projectileType == "enemyProjectile") && otherCollider.CompareTag("Player"))
            || otherCollider.CompareTag("Environment"))
            //plays collision/fizz out sound effect 
            projectileExplodeSFX.Play();
            DisableProjectile();
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
