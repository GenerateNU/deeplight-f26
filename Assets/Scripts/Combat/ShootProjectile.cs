using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class deals with the behavior of shooting a projectile.
/// </summary>
public class ShootProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 5f;

    [SerializeField] private GameObject projectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!projectile)
        {
            Debug.LogWarning("No Projectile Prefab Assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    /// <summary>
    /// This is the primary shoot method that deals with the math of shooting a projectile. It also initializes all necessary information for the
    /// projectile onto the object itself.
    /// </summary>
    private void Shoot()
    {
        if(projectile)
        {
            // currently bugged with the third person controller (projectiles shoot in the direction the player is facing, not the direction
            // they're aiming), should be fixed at a later date when weapons are implemented (weapon will face direction of the camera independent of the
            // direction the player is facing within reason)
            GameObject projectileObject = Instantiate(projectile, transform.position + transform.forward, transform.rotation);

            Rigidbody rb = projectileObject.GetComponent<Rigidbody>();

            if(rb)
            {
                rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
            }
            
            // setting the base attributes of the projectile on the object itself so that information can be used and tampered with
            // after the projectile is spawned, if necessary
            if(projectileObject.TryGetComponent<ProjectileBehavior>(out ProjectileBehavior pb))
            {
                if(this.gameObject.CompareTag("Player"))
                {
                    pb.SetType("playerProjectile");
                }
                else if(this.gameObject.CompareTag("Enemy"))
                {
                    pb.SetType("enemyProjectile");
                }
            }
        }

    }
}
