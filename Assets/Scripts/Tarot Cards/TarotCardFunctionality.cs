using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TarotCardFunctionality : MonoBehaviour
{
    [Header ("Tarot Card Data File")]
    [SerializeField] private TarotData cardDataFile;

    [Header("CardEffect")]
    [SerializeField] private float effectPotency = 0f;

    /*
    void Start()
    {
        InitializeEffectPotency(cardDataFile.GetRarity(), cardDataFile.GetType(), cardDataFile.GetSuit());
    }

    //initializes a card's effectPotency (the value that the card displays as an increase (example: common card has a damage increase of 5% whereas a mythic card
    //has a damage increase of 27%)). Effect potency ranges depend on the ActionType and the Suit of the card (example: a damage buff and a dash distance buff will
    //likely have to be different percentage ranges, despite being of equal card rarity. Likewise, an attack damage buff (wands suit) and an attack lifesteal buff (spell suit)
    //would need different ranges of values for the same type of card). This function should exist because we don't want preset values specific to cards because then
    //we'd need to make hundreds of iterations of cards for each run to be unique, rather than just having the value of a card be randomized based on its rarity.
    void InitializeEffectPotency(Rarity rarity, ActionType type, Suit suit)
    {

    }
    */
}
