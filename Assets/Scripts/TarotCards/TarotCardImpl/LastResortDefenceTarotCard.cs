using UnityEngine;

/// <summary>
/// The Last Resort Defence Tarot Card which is under the Passive category.
/// When equipped, this Tarot Card does the following behavior: 
/// - On reaching critical health (below 10 HP), take 50% less damage for 8 seconds.
/// - Only applies once per room.
/// The goal of this tarot card is to heal the player on every 3rd projectile that hits an enemy.
/// </summary>
[CreateAssetMenu(fileName = "TheLastResortDefence", menuName = "Tarot/The Last Resort Defence")]
public class LastResortDefenceTarotCard : TarotCard
{
    public override string CardName
    {
        get
        {
            Debug.Log("The Last Resort Defence");
            return "The Last Resort Defence";
        }
    }

    public override string CardDescription
    {
        get
        {
            Debug.Log("When the player reaches critical health, they take 50% less damage for 8 seconds. This only applies once per room.");
            return "When the player reaches critical health, they take 50% less damage for 8 seconds. This only applies once per room.";

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

    [SerializeField] private float _cooldownDuration = 5f;
    [SerializeField] private float _damageDecreasePercent = 0.5f;
    [SerializeField] private float _duration = 8f;
    private bool _isSubscribed; // TODO decide whether to make it a listener or for it to constantly check the players HP

    /// <summary>
    /// To apply tarot card, ...
    /// It takes in TarotContext which holds objects needed for each tarot card to apply their changes.
    /// </summary>
    /// <param name="context">A class that holds objects needed for tarot cards.</param>
    public override void ApplyTarotCard(TarotContext context)
    {
        if (!_isSubscribed)
        {
            _isSubscribed = true;
        }
    }

    /// <summary>
    /// To remove tarot card, ....
    /// </summary>
    public override void RemoveTarotCard()
    {
        if (_isSubscribed)
        {
            _isSubscribed = false;
        }
    }

    /// <summary>
    /// A method that ....
    /// </summary>
    private void HandleBlank()
    {

    }

    /// <summary>
    /// A method that resets the _isSubscribed field upon start.
    /// Since TarotCard's are scriptable objects, this field will be carried over.
    /// </summary>
    private void OnEnable()
    {
        _isSubscribed = false;
    }
}
