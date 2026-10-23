using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ExampleTarotCard : TarotCard<GameObject>
{
    public override string CardName
    {
        get { return "The Fool"; }
    }

    public override string CardDescription
    {
        get { return "A new beginning. Affects the game objects it is applied to."; }
    }

    public override CardRarity Rarity
    {
        get { return CardRarity.Common; }
    }

    /// <summary>
    /// An abstract method that each tarot card will implement for their special abilities
    /// </summary>
    /// <param name="_objectsToManipulate">A list of game objects that the tarot card impacts.</param>
    public override void ApplyTarotCard(List<GameObject> _objectsToManipulate)
    {
        Debug.Log("Tarot Activated!");
    }
}
