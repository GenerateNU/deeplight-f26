using UnityEngine;
using System;

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

[Serializable]
public class StatEntry
{
    public Stat stat;
    public float baseValue = 1f;
    public float currentValue;
}
