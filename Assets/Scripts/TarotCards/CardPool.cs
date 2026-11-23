using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Asset holding every possible tarot card implementation. Lives in Resources so it can be
/// reached from anywhere, and persists across scenes because it's an asset.
/// </summary>
[CreateAssetMenu(fileName = "CardPool", menuName = "Tarot/Card Pool")]
public class CardPool : ScriptableObject
{
    public bool excludeFromPlayerDeck = true;
    public int cardSelectionSize = 3;

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
    public List<TarotCard> DrawSelection(PlayerDeck playerDeck)
    {
        List<TarotCard> eligible = new List<TarotCard>();

        foreach (TarotCard card in cards)
        {
            if (!excludeFromPlayerDeck || !playerDeck.Owns(card))
            {
                eligible.Add(card);
            }
        }

        int numCards = Mathf.Min(cardSelectionSize, eligible.Count);
        List<TarotCard> selection = new List<TarotCard>(numCards);

        for (int i = 0; i < numCards; i++)
        {
            int index = LootRng.Range(0, eligible.Count);
            selection.Add(eligible[index]);
            eligible.RemoveAt(index); // no duplicates
        }

        return selection;
    }
}