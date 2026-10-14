using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerShootProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] public float projectileSpeed = 5f;
    [SerializeField] public float projectileRange = 20f;

    [Header("Aiming")]
    [SerializeField] private Camera aimCamera;          // defaults to Camera.main
    [SerializeField] private float spawnDistance = 5f;  // how far along the center ray the projectile appears

    //animation controller
    [SerializeField] private Animator playerAnimator;
    [SerializeField] AudioClip shootSFX;

    private void Awake()
    {
        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }
    }

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
            Vector3 spawnPos;
            Vector3 direction;

            //FOR ANIMATOR
            playerAnimator.SetTrigger("attack");

            AudioSource.PlayClipAtPoint(shootSFX, transform.position);

            if (CompareTag("Player"))
            {
                // Center-of-screen ray: no raycast, no target snapping
                Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                direction = ray.direction;
                spawnPos = ray.origin + ray.direction * spawnDistance;
            }
            else
            {
                // Enemies fire straight out of themselves
                direction = transform.forward;
                spawnPos = transform.position + transform.forward;
            }

            projectile.transform.SetPositionAndRotation(spawnPos, Quaternion.LookRotation(direction));
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            //projectile.SetActive(true); pool does this now 

            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(direction * projectileSpeed, ForceMode.VelocityChange);
            }

            // setting the base attributes of the projectile on the object itself so that information can be used and tampered with
            // after the projectile is spawned, if necessary
            if (projectile.TryGetComponent<ProjectileBehavior>(out ProjectileBehavior pb))
            {
                if (CompareTag("Player"))
                {
                    pb.SetType("playerProjectile");
                }
                else if (CompareTag("Enemy"))
                {
                    pb.SetType("enemyProjectile");
                }
            }
        }
    }
}
