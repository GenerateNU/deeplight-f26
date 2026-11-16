using System;
using UnityEngine;
/// <summary>
/// The Decoy Dash Tarot Card which is in the Pentacles (defense) suit under the Dash category.
/// When equipped, this Tarot Card does the following behavior: 
/// - Dashing leaves a decoy at the starting position.
/// - Enemies target the decoy instead of the player for 2 seconds.
/// - Only one decoy can exist at a time.
/// The goal of this tarot card is to have a temporary defensive clone the enemies will shoot when the player dashes.
/// </summary>
[CreateAssetMenu(fileName = "TheDecoyDash", menuName = "Tarot/The Decoy Dash")]
public class DecoyDashTarotCard : TarotCard
{
    public override string CardName
    {
        get
        {
            Debug.Log("The Decoy Dash");
            return "The Decoy Dash";
        }
    }

    public override string CardDescription
    {
        get
        {
            Debug.Log("When the player dashes, a clone of the player will appear, distracting enemies for 2 seconds. Only one decoy can exist at a time.");
            return "When the player dashes, a clone of the player will appear, distracting enemies for 2 seconds. Only one decoy can exist at a time.";

        }
    }

    public override CardRarity Rarity
    {
        get
        {
            Debug.Log(CardRarity.Epic);
            return CardRarity.Epic;
        }
    }

    [SerializeField] private GameObject _decoyPlayerPrefab;
    [SerializeField] private float _decoyDuration = 2f;
    public static GameObject ActivePlayerDecoy { get; private set; }
    private bool _isSubscribed;

    /// <summary>
    /// An abstract method that each tarot card will implement for their special abilities
    /// </summary>
    public override void ApplyTarotCard(TarotContext context)
    {
        if (!_isSubscribed)
        {
            ThirdPersonController.OnDashStarted += HandleDash;
            _isSubscribed = true;
        }
    }

    /// <summary>
    /// An abstract method that each tarot card will implement to remove their special abilites when a run ends.
    /// </summary>
    public override void RemoveTarotCard()
    {
        if (_isSubscribed)
        {
            ThirdPersonController.OnDashStarted -= HandleDash;
            _isSubscribed = false;
        }

        if (ActivePlayerDecoy != null)
        {
            Destroy(ActivePlayerDecoy);
        }
    }

    private void HandleDash(Vector3 startPosition, Quaternion rotation)
    {
        if (ActivePlayerDecoy != null)
        {
            return;
        }

        ActivePlayerDecoy = Instantiate(_decoyPlayerPrefab, startPosition, rotation);
        Destroy(ActivePlayerDecoy, _decoyDuration);
    }
}
