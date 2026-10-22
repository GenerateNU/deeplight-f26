using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActionType
{
    Attack,
    Dash,
    Block,
    Spell
}

public enum Suit
{
    Wands,
    Sword,
    Block,
    Spell
}

public class TarotData : ScriptableObject
{
    [Header ("Card Definition")]
    [SerializeField] private ActionType cardType;
    [SerializeField] private Suit cardSuit;
    [SerializeField] private float cardRarity;

    public ActionType GetType()
    {
        return cardType;
    }

    public Suit GetSuit()
    {
        return cardSuit;
    }

    public float GetRarity()
    {
        return cardRarity;
    }
}
