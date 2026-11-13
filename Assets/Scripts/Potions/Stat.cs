using UnityEngine;
using System;

/// <summary>
/// Enum that handles all of the possible Stats
/// </sumamry>
public enum Stat
{
    AttackDamage,
    AttackRate,
    ProjectileSpeed,
    ProjectilePierce,
    ProjectileLifespan,
    MovementSpeed,
    DashDistance,
    DashCooldown,
    DashInvincibilityTime
}

/// <summary>
/// Serializable class for a StatEntry, which holds fields for the Stat, its base value (usually 1), and the current value
/// after potion modifications have been applied.
/// </sumamry>
[Serializable]
public class StatEntry
{
    public Stat stat;
    public float baseValue = 1f;
    public float currentValue;
}

/// <summary>
/// A stat change from a tarot card source. 
/// Used for tracking what modifiers have been applied so they can be removed later.
/// </summary>
public class StatModifier
{
    public Stat Stat;
    public Operation Operation;
    public float Value;
    public TarotCard TarotCard;   // which tarot card added it, so it can be removed later
}