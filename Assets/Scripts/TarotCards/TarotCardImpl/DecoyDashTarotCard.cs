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
    /// To apply tarot card, subscribes to the OnDashStarted event.
    /// It takes in TarotContext which holds objects needed for each tarot card to apply their changes.
    /// </summary>
    /// <param name="context">A class that holds objects needed for tarot cards.</param>
    public override void ApplyTarotCard(TarotContext context)
    {
        if (!_isSubscribed)
        {
            ThirdPersonController.OnDashStarted += HandleDash;
            _isSubscribed = true;
        }
    }

    /// <summary>
    /// To remove tarot card, unsubscribes from the OnDashStarted event and destroys any active decoy.
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

    /// <summary>
    /// A method that gets called after this class subscribed to 3DPlayerController's OnDashStarted event. 
    /// Creates a decoy enemy prefab in the scene and destroys it after a set amount of time.
    /// </summary>
    /// <param name="startPosition">Position of the player.</param>
    /// <param name="rotation">Rotation of the player.</param>
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