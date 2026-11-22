using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns potions on event
/// </summary>
public class PotionDropper : MonoBehaviour
{
  [SerializeField] private RngStreamsConfig rngConfig;
  [SerializeField, Range(0f, 1f)] private float chanceOfDrop = 0.7f;
  [SerializeField] private PotionPickup pickupPrefab;

  [Tooltip("Relative chance of each rarity, in enum order: Common, Rare, UltraRare, Mythic.")]
  [SerializeField] private float[] rarityWeights = { 70f, 20f, 8f, 2f };

  /// <summary>
  /// Anything destructible or killable that should drop a potion calls this method when destroyed
  /// </summary>
  public void OnDeath()
  {
    if (rngConfig == null)
    {
      Debug.LogWarning("PotionDropper is missing an RngStreamsConfig, no potion dropped.", this);
      return;
    }

    if (rngConfig.Get(RngStreamsConfig.RngStreamType.Loot).Chance(chanceOfDrop))
    {
      Drop();
    }
  }

  /// <summary>
  /// Spawns a pickup for a random potion with a random rarity at this object's position
  /// </summary>
  public void Drop()
  {
    if (pickupPrefab == null || rngConfig == null)
    {
      Debug.LogWarning("PotionDropper needs a pickup prefab and an RngStreamsConfig.", this);
      return;
    }

    List<Potion> potions = AllPotions.Instance.GetAllPotions();
    if (potions.Count == 0)
    {
      Debug.LogWarning("AllPotions has no potions to drop.", this);
      return;
    }

    RngStream loot = rngConfig.Get(RngStreamsConfig.RngStreamType.Loot);
    Potion randomPotion = potions[loot.Range(0, potions.Count)];

    PotionPickup pickup = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
    pickup.Initialize(randomPotion.GetStat(), RollRarity(loot), randomPotion.GetOperation());
  }

  /// <summary>
  /// Picks a rarity using rarityWeights, indexed by the Rarity enum
  /// </summary>
  private Rarity RollRarity(RngStream loot)
  {
    float total = 0f;
    foreach (float weight in rarityWeights)
    {
      total += weight;
    }

    float roll = loot.Value() * total;
    for (int i = 0; i < rarityWeights.Length; i++)
    {
      roll -= rarityWeights[i];
      if (roll < 0f)
      {
        return (Rarity)i;
      }
    }

    return Rarity.Common;
  }
}