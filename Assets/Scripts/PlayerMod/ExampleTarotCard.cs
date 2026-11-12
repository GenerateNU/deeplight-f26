using UnityEngine;

/// <summary>
/// Example implementation of a tarot card.
/// </summary>
[CreateAssetMenu(fileName = "TheFool", menuName = "Tarot/The Fool")]
public class ExampleTarotCard : TarotCard
{
    public override string CardName
    {
        get
        {
            Debug.Log("The Fool");
            return "The Fool";
        }
    }

    public override string CardDescription
    {
        get
        {
            Debug.Log("A new beginning. Affects the game objects it is applied to.");
            return "A new beginning. Affects the game objects it is applied to.";
        }
    }

    public override CardRarity Rarity
    {
        get
        {
            Debug.Log(CardRarity.Common);
            return CardRarity.Common;
        }
    }

    /// <summary>
    /// An abstract method that each tarot card will implement for their special abilities
    /// </summary>
    public override void ApplyTarotCard()
    {
        Debug.Log("Tarot Activated!");
    }
}
