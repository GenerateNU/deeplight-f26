using UnityEngine;
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

[CreateAssetMenu(fileName = "PotionData", menuName = "ScriptableObjects/New PotionData")]
public class PotionData : ScriptableObject
{
    [Header("Potion Specifics")]
    [SerializeField] private Stat stat;
    [SerializeField] private List<FloatRange> ranges = new List<FloatRange>(4);

    /// <Summary>
    /// Gets the Stat of the potion
    /// </Summary>
    /// <Returns></Returns> The Stat of the potion
    public Stat GetStat()
    {
        return stat;
    }

    /// <Summary>
    /// Gets the list of FloatRanges of the potion
    /// </Summary>
    /// <Returns></Returns> The list of FloatRanges of the potion
    public List<FloatRange> GetRanges()
    {
        return ranges;
    }
}
