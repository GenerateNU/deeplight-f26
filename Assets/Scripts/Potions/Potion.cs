using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enum for the Rarity of a potion
/// </summary>
public enum Rarity
{
    Common = 0,
    Rare = 1,
    UltraRare = 2,
    Mythic = 3
}

/// <summary>
/// Enum for the stat-modifying operation of the potion
/// </summary>
public enum Operation
{
    Add,
    Multiply
}

/// <summary>
/// A serializable Potion object with a Stat, Rarity, and Operation
/// </summary>
[Serializable]
public class Potion
{
    [Header("Generic Potion Data")]
    [SerializeField] private Stat stat;
    [SerializeField] private List<FloatRange> ranges = new List<FloatRange>(4);

    [Header("Unique Potion Qualities")]
    [Tooltip("Operation: Add or Multiply.")]
    [SerializeField] private Operation operation;

    [Header("LootRngStream")]
    [SerializeField] private RngStreamsConfig rngStreamsConfig;

    private Rarity rarity;
    private float finalModifierValue = 0f;

    /// <summary>
    /// Rolls this potion's modifier value. Call once when the potion is granted.
    /// </summary>
    private void Roll()
    {
        finalModifierValue = RollModifierValue();
    }

    /// <summary>
    /// Returns a fresh copy of this definition with its own rolled value.
    /// </summary>
    public Potion CreateRolledInstance(Rarity rarity)
    {
        Potion copy = (Potion)MemberwiseClone();
        copy.ranges = new List<FloatRange>(ranges);
        copy.rarity = rarity;
        copy.Roll();
        return copy;
    }

    /// <summary>
    /// Rolls a value within this potion's rarity-specific range using the loot stream
    /// </summary>
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

    /// <summary>
    /// Gets the Rarity from the PotionData file
    /// </summary>
    /// <Returns></Returns> The Rarity of the potion
    public Rarity GetRarity() => rarity;

    /// <summary>
    /// Gets the Operation from the PotionData file
    /// </summary>
    /// <Returns></Returns> The Operation of the potion
    public Operation GetOperation() => operation;

    /// <summary>
    /// Gets the final modifier value
    /// </summary>
    /// <Returns></Returns> The final modifier value
    public float GetFinalModifierValue() => finalModifierValue;

    /// <summary>
    /// Gets the Stat of the potion
    /// </summary>
    /// <Returns></Returns> The Stat of the potion
    public Stat GetStat() => stat;

    /// <summary>
    /// Gets the list of FloatRanges of the potion
    /// </summary>
    /// <Returns></Returns> The list of FloatRanges of the potion
    public List<FloatRange> GetRanges() => ranges;
}
