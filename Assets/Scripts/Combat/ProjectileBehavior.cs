using UnityEngine;

/// <summary>
/// This class represents the behavior of a projectile. It deals with collision detection and destruction of projectiles.
/// </summary>
public class ProjectileBehavior : MonoBehaviour
{
    /// <summary>
    /// This method destroys a projectile during a collision.
    /// </summary>
    /// <param name="other"></param> Represents a collider object.
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Target"))
        {
            DestroyProjectile();
        }
    }

    /// <summary>
    ///  This method calls the Destroy utility method to remove the object from memory.
    /// </summary>
    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }


}
