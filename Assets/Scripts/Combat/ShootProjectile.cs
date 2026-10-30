using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// This class deals with the behavior of shooting a projectile.
/// </summary>
public class ShootProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] public float projectileSpeed = 5f;
    [SerializeField] public float projectileRange = 20f;
    public event Action OnPlayerShoot;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        // Switch to this if statment for testing: 
        // if (Input.GetButton("Fire1"))
        // {
        //     Shoot();
        // }
    }

    /// <summary>
    /// This is the primary shoot method that deals with the math of shooting a projectile. It also initializes all necessary information for the
    /// projectile onto the object itself.
    /// </summary>
    private void Shoot()
    {
        GameObject projectile = ObjectPool.instance.GetPooledObject();

        if (projectile)
        {
            projectile.transform.SetPositionAndRotation(transform.position + transform.forward, transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            //projectile.SetActive(true); pool does this now 

            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
            }

            // setting the base attributes of the projectile on the object itself so that information can be used and tampered with
            // after the projectile is spawned, if necessary
            if (projectile.TryGetComponent<ProjectileBehavior>(out ProjectileBehavior pb))
            {
                if (this.gameObject.CompareTag("Player"))
                {
                    pb.SetType("playerProjectile");
                    OnPlayerShoot?.Invoke();
                }
                else if (this.gameObject.CompareTag("Enemy"))
                {
                    pb.SetType("enemyProjectile");
                }
            }
        }

    }
}
