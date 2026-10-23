using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
///
/// </summary>
/// <typeparam name="T">The game objects that will be manipulated by the card.</typeparam>
public abstract class TarotCard<T> : MonoBehaviour
{
    [NonSerialized] protected List<T> _objectsToManipulate = new List<T>();

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

    // Methods:

    /// <summary>
    /// An abstract method that each tarot card will implement for their special abilities
    /// </summary>
    /// <param name="_objectsToManipulate">A list of game objects that the tarot card impacts.</param>
    public abstract void ApplyTarotCard(List<T> _objectsToManipulate);
}
