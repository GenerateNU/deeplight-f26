using UnityEngine;
using System;

/// <summary>
/// Enum that handles all of the possible Stats
/// </Sumamry>
public enum Stat
{
    AttackDamage,
    AttackRate,
    ProjectileSpeed,
    ProjectilePierce,
    DashDistance,
    DashCooldown,
    DashInvincibilityTime
}

/// <summary>
/// Serializable class for a StatEntry, which holds fields for the Stat, its base value (usually 1), and the current value
/// after potion modifications have been applied.
/// </Sumamry>
[Serializable]
public class StatEntry
{
    public Stat stat;
    public float baseValue = 1f;
    public float currentValue;
}
