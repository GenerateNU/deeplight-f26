using UnityEngine;

/// <summary>
/// This class represents the behavior of a projectile. It deals with collision detection and
/// returning projectiles to the pool.
/// </summary>
public class ProjectileBehavior : MonoBehaviour
{
    [Header("Projectile Settings")]

    [SerializeField] private string projectileType;

    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private int headshotMultiplier = 2;
    //[SerializeField] private float projectileSpeed;

    [SerializeField] private float duration = 3f;

    private float timer;

    //SFX and particles
    public AudioSource projectileExplodeSFX;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            //plays collision/fizz out sound effect
            projectileExplodeSFX.Play();
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
    /// This method destroys a projectile during a collision with the player. If colliding with the player, will call Damage on the player's
    /// health component script. Projectile will be destroyed if shot by a player and hits an enemy, or shot by the enemy and hits a player.
    /// </summary>
    /// <param name="other"></param> Represents a collider object.
    private void OnTriggerEnter(Collider otherCollider)
    {
        if (projectileType == "playerProjectile" && (otherCollider.CompareTag("Enemy") || otherCollider.CompareTag("EnemyHead")) || ((projectileType == "enemyProjectile") && otherCollider.CompareTag("Player")) || otherCollider.CompareTag("Environment"))
        {
            //plays collision/fizz out sound effect
            projectileExplodeSFX.Play();

            HealthComponent hc = otherCollider.GetComponentInParent<HealthComponent>();

            if (hc != null)
            {
                if (otherCollider.CompareTag("EnemyHead"))
                {
                    hc.Damage(projectileDamage * headshotMultiplier);
                }
                else
                {
                    hc.Damage(projectileDamage);
                }
            }
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

    // ---------------------- SETTERS ----------------------- //

    /// <summary>
    /// Sets the projectile type to the value input into the function
    /// </summary>
    /// <param name="type"></param> the string dictating the type of projectile
    public void SetType(string type)
    {
        projectileType = type;
    }
}
