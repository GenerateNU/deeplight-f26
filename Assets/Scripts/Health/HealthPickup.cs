using UnityEngine;

/// <summary>
/// A script for a prefab that heals the player on collision
/// </summary>
public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<HealthComponent>(out HealthComponent hc))
            {
                if (hc.Heal(healAmount) > 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
