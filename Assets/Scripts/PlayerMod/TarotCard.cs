using System;
using UnityEngine;
using System.Collections.Generic;

public abstract class TarotCard<T> : MonoBehaviour
{
    [NonSerialized] private String _cardName;
    [NonSerialized] private String _cardDescription;
    [NonSerialized] private CardRarity _cardRarity;
    [NonSerialized] private List<T> _objectsToManipulate;

    public enum CardRarity
    {
        Common,
        Rare,
        Epic
    }

    // methods 
    public String GetName()
    {
        return _cardName;
    }

    public String GetDescription()
    {
        return _cardDescription;
    }

    public CardRarity GetRarity()
    {
        return _cardRarity;
    }

    public abstract void ApplyTarotCard(List<T> _objectsToManipulate);
}
