using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Rarity
{
    Common = 0,
    Rare = 1,
    UltraRare = 2,
    Mythic = 3
}

[CreateAssetMenu(fileName = "PotionData", menuName = "ScriptableObjects/New PotionData")]
public class PotionData : ScriptableObject
{
    [Header("Potion Specifics")]
    [SerializeField] private Stat stat;
    [SerializeField] private List<FloatRange> ranges = new List<FloatRange>(4);

    public Stat GetStat()
    {
        return stat;
    }

    public List<FloatRange> GetRanges()
    {
        return ranges;
    }
}