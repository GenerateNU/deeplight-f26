using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// This class deals with the behavior of shooting a projectile.
/// </summary>
public class ShootProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 5f; 
    [SerializeField] private GameObject projectilePrefab;

    [Header("Object Pool Settings")]
    [SerializeField] private int startingCapacity = 50; 
    [SerializeField] private int maxSize = 500; 
    [SerializeField] private Transform projectileParent;

    private ObjectPool<GameObject> pool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!projectilePrefab)
        {
            Debug.LogWarning("No Projectile Prefab Assigned.");
            return;
        }

        pool = new ObjectPool<GameObject>(
            SpawnProjectile,
            projectile => projectile.gameObject.SetActive(true),
            projectile => projectile.gameObject.SetActive(false),
            projectile => Destroy(projectile.gameObject),
            true, // throw an error if we release a projectile already in the pool 
            startingCapacity, 
            maxSize // max amount of inactive projectiles allowed 
        );

        FillPool();
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
    /// A method that fills the pool with projectiles. 
    /// </summary>
    private void FillPool()
    {
        for (int i = 0; i < startingCapacity; i++)
        {
            pool.Release(SpawnProjectile());
        }
    }

    /// <summary>
    /// A method that spawns a new projectile for the pool. 
    /// </summary>
    /// <returns>The new projectile.</returns>
    private GameObject SpawnProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileParent);
        projectile.GetComponent<ProjectileBehavior>().SetPool(pool);

        return projectile;
    }

    /// <summary>
    /// This is the primary shoot method that deals with the math of shooting a projectile.
    /// </summary>
    private void Shoot()
    {
        GameObject projectile = pool.Get();

        if(projectile)
        {
            projectile.transform.SetPositionAndRotation(transform.position + transform.forward, transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if(rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
            }
        }

    }
}
