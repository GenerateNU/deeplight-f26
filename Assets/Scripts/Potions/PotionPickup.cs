using UnityEngine;

/// <summary>
/// Script for all potion pickups, applies potion effects to player
/// </summary>
public class PotionPickup : MonoBehaviour
{
    [SerializeField] private Stat stat;
    [SerializeField] private Rarity rarity;
    [SerializeField] private Operation operation;

    /// <summary>
    /// Configures which potion this pickup grants. Call right after spawning.
    /// </summary>
    public void Initialize(Stat stat, Rarity rarity, Operation operation)
    {
        this.stat = stat;
        this.rarity = rarity;
        this.operation = operation;
    }

    /// <summary>
    /// If collision is from the player, adds the potion to the player's stathandler and destroys it
    /// </summary>
    /// <param name="other"> collider that collides with potion </param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StatHandler handler = other.GetComponentInParent<StatHandler>();

            if (handler != null)
            {
                Potion potion = AllPotions.Instance.CreatePotion(stat, rarity, operation);
                if (potion == null)
                {
                    Debug.LogWarning($"No potion definition for {stat}/{operation}.", this);
                    return;
                }

                handler.AddPotionToList(potion);
                Destroy(gameObject);
            }
        }
    }
}
