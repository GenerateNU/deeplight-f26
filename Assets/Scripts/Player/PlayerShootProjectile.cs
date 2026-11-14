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
    [Header("Projectile Settings")]
    [SerializeField] public float projectileSpeed = 5f;
    [SerializeField] public float projectileRange = 20f;
    [Header("Projectile Prefab")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("Aiming")]
    [SerializeField] private Camera aimCamera;          // defaults to Camera.main
    [SerializeField] private float spawnDistance = 2f;  // how far along the center ray the projectile appears

    //animation controller
    [SerializeField] private Animator playerAnimator;
    [SerializeField] AudioClip shootSFX;

    /// <summary>
    /// event that raises when the player fires a projectile.
    /// </summary>
    public event Action OnPlayerShoot;

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

            // Center-of-screen ray: no raycast, no target snapping
            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            direction = ray.direction;
            spawnPos = ray.origin + ray.direction * spawnDistance;

            projectile.transform.SetPositionAndRotation(spawnPos, Quaternion.LookRotation(direction));
            OnPlayerShoot?.Invoke();
            projectile.SetActive(true);
        }
    }
}
