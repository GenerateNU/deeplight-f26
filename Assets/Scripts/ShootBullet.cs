using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float projectileSpeed = 5f; 
    //public float projectileRange = 20f; // how far the bullet can go before getting destroyed 


    public GameObject projectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!projectile)
        {
            projectile = GameObject.FindGameObjectWithTag("projectile");
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

    void Shoot()
    {
        if(projectile)
        {
            GameObject projectileObject = Instantiate(projectile, transform.position + transform.forward, transform.rotation);
            Rigidbody rb = projectileObject.GetComponent<Rigidbody>();

            if(rb)
            {
                rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
            }
        }

    }
}
