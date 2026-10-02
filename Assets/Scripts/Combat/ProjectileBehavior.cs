using UnityEngine;

/// <summary>
/// This class represents the behavior of a projectile. It deals with collision detection and 
/// returning projectiles to the pool.
/// </summary>
public class ProjectileBehavior : MonoBehaviour
{
    [Header("Projectile Settings")] 
    [SerializeField] private float duration = 3f;

    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
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
    /// This method destroys a projectile during a collision.
    /// </summary>
    /// <param name="other"></param> Represents a collider object.
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Target"))
        {
            DisableProjectile();  
        }
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
}
