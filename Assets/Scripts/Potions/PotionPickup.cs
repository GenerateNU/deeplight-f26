using UnityEngine;

public class PotionPickup : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<HealthComponent>(out HealthComponent hc))
            {
                Destroy(gameObject);
            }
        }
    }
}
