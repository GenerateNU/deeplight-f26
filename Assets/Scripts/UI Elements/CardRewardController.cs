using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CardRewardController : MonoBehaviour
{
    [SerializeField] private Canvas cardRewardScreen;
    [SerializeField] private PlayerDeck playerDeck;
    [SerializeField] private GameObject[] cardSlots;
    private List<TarotCard> currentSelection;
    private bool isEnabled = false;

    private void Awake()
    {
        currentSelection = new List<TarotCard>();
        cardRewardScreen = GetComponent<Canvas>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Deactivate();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isEnabled)
            {
                Deactivate();
            }
            else
            {
                Activate();
            }
        }
    }

    public void Activate()
    {
        cardRewardScreen.enabled = true;
        isEnabled = true;

        // select cards
        currentSelection = CardPool.Instance.DrawSelection(playerDeck);
        PopulateCards();
    }

    public void Deactivate()
    {
        cardRewardScreen.enabled = false;
        isEnabled = false;
    }

    private void PopulateCards()
    {
        if (currentSelection.Count < 3)
        {
            return; // we want 3 cards for now
        }

        // TODO: actually implement cards visually
        // placeholder: text of card name
        String cardName1 = currentSelection[0].CardName;
        String cardName2 = currentSelection[1].CardName;
        String cardName3 = currentSelection[2].CardName;

        cardSlots[0].GetComponentInChildren<TMP_Text>().SetText(cardName1);
        cardSlots[1].GetComponentInChildren<TMP_Text>().SetText(cardName2);
        cardSlots[2].GetComponentInChildren<TMP_Text>().SetText(cardName3);
    }
}
