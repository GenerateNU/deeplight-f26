using UnityEngine;

/// <summary>
/// This class deals with the behavior of shooting a projectile.
/// </summary>
public class ShootProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed = 5f; 
    [SerializeField] private float projectileRange = 20f; 

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
    /// This is the primary shoot method that deals with the math of shooting a projectile.
    /// </summary>
    private void Shoot()
    {
        GameObject projectile = ObjectPool.instance.GetPooledObject();

        if(projectile)
        {
            projectile.transform.SetPositionAndRotation(transform.position + transform.forward, transform.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            //projectile.SetActive(true); pool does this now 

            if(rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
            }
        }

    }
}
