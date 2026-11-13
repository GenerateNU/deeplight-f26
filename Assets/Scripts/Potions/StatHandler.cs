using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Stat handler class, which handles stat changes and keeping track of your acquired potions
/// </summary>
// TODO: Make this a singleton class
public class StatHandler : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private List<StatEntry> _stats = new List<StatEntry>();

    [Header("PotionsAcquired")]
    [SerializeField] private List<Potion> _potionsAcquired = new List<Potion>();

    private Dictionary<Stat, StatEntry> _statLookup = new Dictionary<Stat, StatEntry>();
    private List<StatModifier> _modifiers = new List<StatModifier>();


    /// <summary>
    /// Runs on Awake
    /// </summary>
    void Awake()
    {
        SyncStatEntries();

        _statLookup.Clear();
        foreach (StatEntry entry in _stats)
        {
            _statLookup[entry.stat] = entry;
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
            if (!_stats.Exists(e => e.stat == stat))
            {
                _stats.Add(new StatEntry { stat = stat });
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
        return _statLookup.TryGetValue(stat, out StatEntry entry) ? entry.currentValue : 0f;
    }

    /// <summary>
    /// Adds a potion to the list of acquired potions
    /// </summary>
    /// <param name="potion"></param> The Potion to add to the list
    public void AddPotionToList(Potion potion)
    {
        if (potion == null)
        {
            return;
        }
        _potionsAcquired.Add(potion);
        SetStat(potion.GetStat());
    }

    /// <summary>
    /// Sets a stat's current value based on the potions/tarot cards of its type in the acquired potions and tarot card lists.
    /// </summary>
    /// <param name="stat"></param> The desired Stat to set
    void SetStat(Stat stat)
    {
        if (!_statLookup.TryGetValue(stat, out StatEntry entry))
        {
            Debug.LogWarning($"SetStat: no entry for stat {stat}.", this);
            return;
        }

        float additiveSum = 0f;
        float multiplier = 1f;

        foreach (Potion potion in _potionsAcquired)
        {
            if (potion.GetStat() != stat)
            { continue; }

            if (potion.GetOperation() == Operation.Add)
            { additiveSum += potion.GetFinalModifierValue(); }
            else if (potion.GetOperation() == Operation.Multiply)
            { multiplier *= 1 + potion.GetFinalModifierValue(); }
        }

        foreach (StatModifier modifier in _modifiers)
        {
            if (modifier.Stat != stat)
            { continue; }

            if (modifier.Operation == Operation.Add)
            { additiveSum += modifier.Value; }
            else if (modifier.Operation == Operation.Multiply)
            { multiplier *= 1 + modifier.Value; }
        }

        // Additive effects apply to the base first, then multiplicative ones
        entry.currentValue = (entry.baseValue + additiveSum) * multiplier;
    }

    /// <summary>
    /// A method that adds a modifier to an affected game object for tarot card abilities. 
    /// </summary>
    /// <param name="stat">The stat that will be affected.</param>
    /// <param name="operation">The operation to apply to the stat.</param>
    /// <param name="value">The value included in the operation to the stat.</param>
    /// <param name="affectedObject">The tarot card.</param>
    public void AddModifier(Stat stat, Operation operation, float value, TarotCard tarotCard)
    {
        _modifiers.Add(new StatModifier {Stat = stat, Operation = operation, Value = value, TarotCard = tarotCard});
        SetStat(stat);
    }

    /// <summary>
    /// Removes every modifier added by the given tarot card and recalculates the related stats.
    /// </summary>
    /// <param name="tarotCard">The tarot card whose modifiers should be removed.</param>
    public void RemoveModifier(TarotCard tarotCard)
    {
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            StatModifier modifier = _modifiers[i];

            if (modifier.TarotCard != tarotCard)
            { continue; }

            _modifiers.RemoveAt(i);
            SetStat(modifier.Stat);
        }
    }
}
