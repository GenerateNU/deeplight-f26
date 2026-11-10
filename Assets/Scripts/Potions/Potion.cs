using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <Summary>
/// Enum for the Rarity of a potion
/// </Summary>
public enum Rarity
{
    Common = 0,
    Rare = 1,
    UltraRare = 2,
    Mythic = 3
}

/// <Summary>
/// Enum for the stat-modifying operation of the potion
/// </Summary>
public enum Operation
{
    Add,
    Multiply
}

/// <Summary>
/// A serializable Potion object with a Stat, Rarity, and Operation
/// </Summary>
[Serializable]
public class Potion
{
    [Header("Generic Potion Data")]
    [SerializeField] private Stat stat;
    [SerializeField] private List<FloatRange> ranges = new List<FloatRange>(4);

    [Header("Unique Potion Qualities")]
    [SerializeField] private Rarity rarity;
    [Tooltip("Operation: Add or Multiply.")]
    [SerializeField] private Operation operation;

    [Header("LootRngStream")]
    [SerializeField] private RngStreamsConfig rngStreamsConfig;

    private float finalModifierValue = 0f;

    /// <summary>
    /// Rolls this potion's modifier value. Call once when the potion is granted.
    /// </summary>
    public void Roll()
    {
        finalModifierValue = RollModifierValue();
    }

    /// <summary>
    /// Returns a fresh copy of this definition with its own rolled value.
    /// </summary>
    public Potion CreateRolledInstance()
    {
        Potion copy = (Potion)MemberwiseClone();
        copy.ranges = new List<FloatRange>(ranges);
        copy.Roll();
        return copy;
    }

    /// <Summary>
    /// Rolls a value within this potion's rarity-specific range using the loot stream
    /// </Summary>
    private float RollModifierValue()
    {
        if (rngStreamsConfig == null)
        {
            Debug.LogWarning("missing RngStreamsConfig, modifier not rolled.");
            return 0f;
        }
        
        int index = (int)rarity;

        if (index < 0 || index >= ranges.Count)
        {
            Debug.LogWarning("No range defined for rarity " + rarity + ".");
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
    /// Gets the final modifier value
    /// </Summary>
    /// <Returns></Returns> The final modifier value
    public float GetFinalModifierValue() => finalModifierValue;

    /// <Summary>
    /// Gets the Stat of the potion
    /// </Summary>
    /// <Returns></Returns> The Stat of the potion
    public Stat GetStat() => stat;

    /// <Summary>
    /// Gets the list of FloatRanges of the potion
    /// </Summary>
    /// <Returns></Returns> The list of FloatRanges of the potion
    public List<FloatRange> GetRanges() => ranges;
}
