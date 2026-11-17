using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// This class deals with the behavior of shooting a projectile, for specifically a player.
/// TODO: Potentially an abstract shoot projectile class and make children for enemy and player, etc.
/// </summary>
public class PlayerShootProjectile : MonoBehaviour
{
    [Header("Projectile Prefab")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Aiming")]
    [SerializeField] private Camera aimCamera;          // defaults to Camera.main
    [SerializeField] private float spawnDistance = 2f;  // how far along the center ray the projectile appears
    [Tooltip("Where projectiles spawn. Put an empty child at the player's hand/staff tip. If empty, falls back to spawning along the camera ray.")]
    [SerializeField] private Transform firePoint;
    [Tooltip("How far the crosshair ray looks for something to aim at. If nothing is hit, aims at a point this far away.")]
    [SerializeField] private float maxAimDistance = 100f;

    //animation controller
    [SerializeField] private Animator playerAnimator;
    [SerializeField] AudioClip shootSFX;

    /// <summary>
    /// event that raises when the player fires a projectile.
    /// </summary>
    public event Action OnPlayerShoot;

    private readonly RaycastHit[] aimHits = new RaycastHit[16];

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
    }

    /// <summary>
    /// This is the primary shoot method that pulls a player projectile from the pool and initializes its direction and spawn position.
    /// </summary>
    private void Shoot()
    {
        GameObject projectile = ObjectPool.instance.GetPooledObject(projectilePrefab);

        if (projectile)
        {
            Vector3 spawnPos;
            Vector3 direction;

            //FOR ANIMATOR
            playerAnimator.SetTrigger("attack");

            AudioSource.PlayClipAtPoint(shootSFX, transform.position);

            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            direction = ray.direction;

            // Spawn from the fire point (hand/staff) if one is set, otherwise use the old camera-ray spawn.
            if (firePoint != null)
            {
                spawnPos = firePoint.position;

                Vector3 targetPoint = ray.origin + ray.direction * maxAimDistance;

                int hitCount = Physics.RaycastNonAlloc(ray, aimHits, maxAimDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
                float closestDistance = float.MaxValue;

                for (int i = 0; i < hitCount; i++)
                {
                    if (aimHits[i].collider.transform.IsChildOf(transform))
                    {
                        continue;
                    }

                    if (aimHits[i].distance < closestDistance)
                    {
                        closestDistance = aimHits[i].distance;
                        targetPoint = aimHits[i].point;
                    }
                }

                Vector3 toTarget = targetPoint - spawnPos;


                if (toTarget.sqrMagnitude < 0.01f || Vector3.Dot(toTarget, ray.direction) <= 0f)
                {
                    direction = ray.direction;
                }
                else
                {
                    direction = toTarget.normalized;
                }
            }
            else
            {
                spawnPos = ray.origin + ray.direction * spawnDistance;
            }

            projectile.transform.SetPositionAndRotation(spawnPos, Quaternion.LookRotation(direction));
            OnPlayerShoot?.Invoke();
            projectile.SetActive(true);
        }
    }
}
