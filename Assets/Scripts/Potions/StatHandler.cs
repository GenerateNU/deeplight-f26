using UnityEngine;
using System;
using System.Collections.Generic;

public class StatHandler : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private List<StatEntry> stats = new List<StatEntry>();

    [Header("PotionsAcquired")]
    [SerializeField] private List<Potion> potionsAcquired = new List<Potion>();

    private Dictionary<Stat, StatEntry> statLookup = new Dictionary<Stat, StatEntry>();

    /// <Summary>
    /// Runs on Start
    /// </Summary>
    void Start()
    {
        AddPotionToList(GameObject.Find("RareAttackDamagePotion").GetComponent<Potion>());
    }

    /// <Summary>
    /// Runs on Awake
    /// </Summary>
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

    /// <Summary>
    /// Runs in the editor whenever the script changes, so new enum values show up in the Inspector
    /// </Summary>
    void OnValidate()
    {
        SyncStatEntries();
    }

    /// <Summary>
    /// Adds an entry for every Stat enum value that doesn't have one yet
    /// </Summary>
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

    /// <Summary>
    /// Gets the Stat that needs to be updated
    /// </Summary>
    /// <param name="potion"></param> The desired Stat to get
    /// <Returns></Returns> A Stat
    public float GetStat(Stat stat)
    {
        return statLookup.TryGetValue(stat, out StatEntry entry) ? entry.currentValue : 0f;
    }

    /// <Summary>
    /// Adds a potion to the list of acquired potions
    /// </Summary>
    /// <param name="potion"></param> The Potion to add to the list
    void AddPotionToList(Potion potion)
    {
        potionsAcquired.Add(potion);
        SetStat(potion.GetPotionData().GetStat());
    }

    /// <Summary>
    /// Sets a stat's current value based on the potions of its type in the acquired potions list
    /// </Summary>
    /// <param name="potion"></param> The desired Stat to set
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
            if (potion.GetPotionData().GetStat() != stat)
            { continue; }

            if (potion.GetOperation() == Add)
            { additiveSum += potion.GetFinalModifierValue(); }
            else if (potion.GetOperation() == Multiply)
            { multiplier *= 1 + potion.GetFinalModifierValue(); }
        }

        // Additive effects apply to the base first, then multiplicative ones
        entry.currentValue = (entry.baseValue + additiveSum) * multiplier;
    }
}
