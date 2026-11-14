using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Asset holding every possible tarot card implementation. Lives in Resources so it can be
/// reached from anywhere, and persists across scenes because it's an asset.
/// </summary>
[CreateAssetMenu(fileName = "CardPool", menuName = "Tarot/Card Pool")]
public class CardPool: ScriptableObject
{
    public bool excludeFromPlayerDeck = true;
    public int cardSelectionSize = 3;
    public GameObject player = GameObject.FindGameObjectWithTag("Player");

    [Header("RNG")]
    [SerializeField] private RngStreamsConfig rngConfig;

    /// <summary>
    /// The TarotCards stream from the run's RNG config, used for every card draw.
    /// </summary>
    private RngStream LootRng => rngConfig.Get(RngStreamsConfig.RngStreamType.TarotCards);
    private static CardPool instance;

    public static CardPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<CardPool>("CardPool");
                if (instance == null)
                {
                    Debug.LogError("No CardPool asset found at Resources/CardPool.");
                }
            }
            return instance;
        }
    }

    [Header("Card Pool")]
    [SerializeField] private List<TarotCard> cards = new List<TarotCard>();

    /// <summary>
    /// Returns a list of all available cards
    /// </summary>
    public List<TarotCard> GetAllCards() => cards;

    /// <summary>
    /// Returns one selection of cards from the pool based on the selection size 
    /// and exclusion setting
    /// </summary>    
    public List<TarotCard> DrawSelection()
    {
        List<TarotCard> selection = new List<TarotCard>();
        // lower bound for cards selected is tied to the size of the card pool
        int numCards = Mathf.Min(cardSelectionSize, cards.Count);
        PlayerDeck playerDeck = player.GetComponent<PlayerDeck>();

        while (selection.Count < cardSelectionSize)
        {
            int index = LootRng.Range(0, cards.Count);
            TarotCard selectedCard = cards[index];
            if (excludeFromPlayerDeck && playerDeck.Owns(selectedCard)) // and player already has this card
            {
                continue; // try again
            }
            else
            {
                selection.Add(selectedCard);
            }
        }

        return selection;
    }
}