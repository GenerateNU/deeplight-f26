using System;
using UnityEngine;
/// <summary>
/// The Heal On Dash Tarot Card which is under the Passive category.
/// When equipped, this Tarot Card does the following behavior: 
/// - Dashing through enemy bullets restores 5 HP
/// - Cooldown for 4 seconds
/// The goal of this tarot card is to heal the player when they dash through enemy projectiles.
/// </summary>
[CreateAssetMenu(fileName = "TheHealOnDash", menuName = "Tarot/The Heal On Dash")]
public class HealOnDashTarotCard : TarotCard
{
    public override string CardName
    {
        get
        {
            Debug.Log("The Heal On Dash");
            return "The Heal On Dash";
        }
    }

    public override string CardDescription
    {
        get
        {
            Debug.Log("When the player dashes through an enemy projectile, they restore 5 HP with a cooldown of 4 seconds");
            return "When the player dashes through an enemy projectile, they restore 5 HP with a cooldown of 4 seconds";

        }
    }

    public override CardRarity Rarity
    {
        get
        {
            Debug.Log(CardRarity.Rare);
            return CardRarity.Rare;
        }
    }

    [SerializeField] private float _cooldownDuration = 4f;
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
