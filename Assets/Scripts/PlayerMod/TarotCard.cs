using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T">The game objects that will be manipulated by the card.</typeparam>
public abstract class TarotCard<T> : MonoBehaviour
{
    [NonSerialized] private String _cardName;
    [NonSerialized] private String _cardDescription;
    [NonSerialized] private CardRarity _cardRarity;
    [NonSerialized] private List<T> _objectsToManipulate;

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
    /// A method that returns the tarot card's name. 
    /// </summary>
    /// <returns></returns>
    public String GetName()
    {
        return _cardName;
    }

    /// <summary>
    /// A method that returns the tarot card's description. 
    /// </summary>
    /// <returns></returns>
    public String GetDescription()
    {
        return _cardDescription;
    }

    /// <summary>
    /// A method that returns the tarot card's rarity. 
    /// </summary>
    /// <returns></returns>
    public CardRarity GetRarity()
    {
        return _cardRarity;
    }

    /// <summary>
    /// An abstract method that each tarot card will implement for their special abilities 
    /// </summary>
    /// <param name="_objectsToManipulate">A list of game objects that the tarot card impacts.</param>
    public abstract void ApplyTarotCard(List<T> _objectsToManipulate);
}
