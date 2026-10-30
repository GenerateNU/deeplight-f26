using UnityEngine;
using System.Collections.Generic;

public class Potion : MonoBehaviour
{
    [Header("Generic Potion Data")]
    [SerializeField] private PotionData potionData;

    [Header("Unique Potion Qualities")]
    [SerializeField] private Rarity rarity;
    [Tooltip("bool operation: true makes the potion's effect additive, whereas false makes it multiplicative.")]
    [SerializeField] private bool operation;

    [Header("LootRngStream")]
    [SerializeField] private RngStreamsConfig rngStreamsConfig;

    private float finalModifierValue = 0f;

    /// <Summary>
    /// Runs on awake
    /// </Summary>
    void Awake()
    {
        finalModifierValue = RollModifierValue();
    }

    /// <Summary>
    /// Rolls a value within this potion's rarity-specific range using the loot stream
    /// </Summary>
    private float RollModifierValue()
    {
        if (rngStreamsConfig == null || potionData == null)
        {
            Debug.LogWarning($"{name}: missing RngStreamsConfig or PotionData, modifier not rolled.", this);
            return 0f;
        }

        List<FloatRange> ranges = potionData.GetRanges();
        int index = (int)rarity;

        if (index < 0 || index >= ranges.Count)
        {
            Debug.LogWarning($"{name}: no range defined for rarity {rarity}.", this);
            return 0f;
        }

        FloatRange range = ranges[index];
        float roll = rngStreamsConfig.Get(RngStreamsConfig.RngStreamType.Loot).Value();

        return Mathf.Lerp(range.min, range.max, roll);
    }

    /// <Summary>
    /// Gets the Rarity from the PotionData file
    /// </Summary>
    /// <Returns></Returns> The Rarity of the potion
    public Rarity GetRarity() => rarity;

    /// <Summary>
    /// Gets the Operation from the PotionData file
    /// </Summary>
    /// <Returns></Returns> The Operation of the potion
    public Operation GetOperation() => operation;

    /// <Summary>
    /// Gets the PotionData file
    /// </Summary>
    /// <Returns></Returns> The PotionData file
    public PotionData GetPotionData() => potionData;

    /// <Summary>
    /// Gets the final modifier value
    /// </Summary>
    /// <Returns></Returns> The final modifier value
    public float GetFinalModifierValue() => finalModifierValue;
}
