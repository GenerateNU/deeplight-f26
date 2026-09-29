using UnityEngine;

/// <summary>
/// This class deals with the behavior of shooting a projectile.
/// </summary>
public class ShootProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 5f; 
    [SerializeField] private float projectileRange = 20f; 
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
    /// This is the primary shoot method that deals with the math of shooting a projectile.
    /// </summary>
    private void Shoot()
    {
        GameObject projectile = ObjectPool.instance.GetPooledObject();

        if(projectile)
        {
            projectile.transform.SetPositionAndRotation(transform.position + transform.forward, transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            projectile.SetActive(true);

            if(rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
            }
        }

    }
}
