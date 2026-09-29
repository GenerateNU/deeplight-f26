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
            DisableProjectile();
        }
    }

    /// <summary>
    ///  This method calls the SetActive() method to disable the object from the scene.
    /// </summary>
    private void DisableProjectile()
    {
        gameObject.SetActive(false);
    }


}
