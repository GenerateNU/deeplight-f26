using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Stat handler class, which handles stat changes and keeping track of your acquired potions
/// </summary>
public class StatHandler : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private List<StatEntry> stats = new List<StatEntry>();

    [Header("PotionsAcquired")]
    [SerializeField] private List<Potion> potionsAcquired = new List<Potion>();

    private Dictionary<Stat, StatEntry> statLookup = new Dictionary<Stat, StatEntry>();

    /// <summary>
    /// Runs on Awake
    /// </summary>
    void Awake()
    {
        SyncStatEntries();

        statLookup.Clear();
        foreach (StatEntry entry in stats)
        {
            statLookup[entry.stat] = entry;
            entry.currentValue = entry.baseValue;
        }
    }

    /// <summary>
    /// Runs in the editor whenever the script changes, so new enum values show up in the Inspector
    /// </summary>
    void OnValidate()
    {
        SyncStatEntries();
    }

    /// <summary>
    /// Adds an entry for every Stat enum value that doesn't have one yet
    /// </summary>
    private void SyncStatEntries()
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            if (!stats.Exists(e => e.stat == stat))
            {
                stats.Add(new StatEntry { stat = stat });
            }
        }
    }

    /// <summary>
    /// Gets the Stat that needs to be updated
    /// </summary>
    /// <param name="potion"></param> The desired Stat to get
    /// <returns></returns> A Stat
    public float GetStat(Stat stat)
    {
        return statLookup.TryGetValue(stat, out StatEntry entry) ? entry.currentValue : 0f;
    }

    /// <summary>
    /// Adds a potion to the list of acquired potions
    /// </summary>
    /// <param name="potion"></param> The Potion to add to the list
    void AddPotionToList(Potion potion)
    {
        potionsAcquired.Add(potion);
        SetStat(potion.GetStat());
    }

    /// <summary>
    /// Sets a stat's current value based on the potions of its type in the acquired potions list
    /// </summary>
    /// <param name="stat"></param> The desired Stat to set
    void SetStat(Stat stat)
    {
        if (!statLookup.TryGetValue(stat, out StatEntry entry))
        {
            Debug.LogWarning($"SetStat: no entry for stat {stat}.", this);
            return;
        }

        float additiveSum = 0f;
        float multiplier = 1f;

        foreach (Potion potion in potionsAcquired)
        {
            if (potion.GetStat() != stat)
            { continue; }

            if (potion.GetOperation() == Operation.Add)
            { additiveSum += potion.GetFinalModifierValue(); }
            else if (potion.GetOperation() == Operation.Multiply)
            { multiplier *= 1 + potion.GetFinalModifierValue(); }
        }

        // Additive effects apply to the base first, then multiplicative ones
        entry.currentValue = (entry.baseValue + additiveSum) * multiplier;
    }
}
