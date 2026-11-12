using System;
using UnityEngine;

/// <summary>
/// A representation of a Tarot Card with a name, description, and rarity.
///
/// Each Tarot card will be a child of TarotCard and contain a special ability
/// for the player. The logic for that ability is contained in the ApplyTarotCard
/// method, which must be overriden by the child. Whichever objects are needed to
/// apply the effects of the tarot card's ability should be added as an additional
/// field that can be set elsewhere.
///
/// </summary>
public abstract class TarotCard : ScriptableObject
{
    public abstract String CardName { get; }
    public abstract String CardDescription { get; }
    public abstract CardRarity Rarity { get; }

    /// <summary>
    /// An enum representation of all tarot card rarities.
    /// Can and will be expanded in the future.
    /// </summary>
    public enum CardRarity
    {
        Common,
        Rare,
        Epic
    }

    /// <summary>
    /// An abstract method that each tarot card will implement for their special abilities
    /// </summary>
    public abstract void ApplyTarotCard();
}
