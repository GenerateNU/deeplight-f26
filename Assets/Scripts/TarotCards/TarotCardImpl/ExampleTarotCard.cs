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
    /// An abstract method that each tarot card will implement for their special abilities.
    /// It takes in TarotContext which holds objects needed for each tarot card to apply their changes.
    /// </summary>
    /// <param name="context">A class that holds objects needed for tarot cards.</param>
    public override void ApplyTarotCard(TarotContext context)
    {
        Debug.Log("Tarot Activated!");
    }

    /// <summary>
    /// An abstract method that each tarot card will implement to remove their special abilites when a run ends.
    /// </summary>
    public override void RemoveTarotCard()
    {
        Debug.Log("Tarot removed!");
    }
}
