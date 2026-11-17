using UnityEngine;

/// <summary>
/// The Lifesteal Shot Tarot Card which is under the Attack category.
/// When equipped, this Tarot Card does the following behavior: 
/// - Every third projectile that hits an enemy restores 5 HP.
/// - Cooldown for 5 seconds.
/// The goal of this tarot card is to heal the player on every 3rd projectile that hits an enemy.
/// </summary>
[CreateAssetMenu(fileName = "TheLifestealShot", menuName = "Tarot/The Lifesteal Shot")]
public class LifestealTarotCard : TarotCard
{
    public override string CardName
    {
        get
        {
            Debug.Log("The Lifesteal Shot");
            return "The Lifesteal Shot";
        }
    }

    public override string CardDescription
    {
        get
        {
            Debug.Log("For every third projectile that hits an enemy, the player restores 5 HP with a cooldown for 5 seconds.");
            return "For every third projectile that hits an enemy, the player restores 5 HP with a cooldown for 5 seconds.";

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

    [SerializeField] private float _cooldownDuration = 5f;
    [SerializeField] private float _hpGain = 5f;
    private bool _isSubscribed;

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
