using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The player's tarot cards for the current run. Applies each card as it's added
/// and removes them all when the run ends.
/// </summary>
public class PlayerDeck : MonoBehaviour
{
    [Header("Deck")]
    [SerializeField] private List<TarotCard> playerCards = new List<TarotCard>();

    /// <summary>Stores the card and applies its effect.</summary>
    public void AddCard(TarotCard card)
    {
        if (Owns(card))
        {
            Debug.LogError("Player already has " + card.CardName + " , cannot add to deck.");
        }
        else
        {
            playerCards.Add(card);
        }
    }

    /// <summary>True if the player already owns this card.</summary>
    public bool Owns(TarotCard card)
    {
        return playerCards.Contains(card); // equality check based on card name
    }

    /// <summary>Removes every card's effect and empties the deck. Call when the run ends.</summary>
    public void EndRun()
    {
        playerCards.Clear();
    }
}