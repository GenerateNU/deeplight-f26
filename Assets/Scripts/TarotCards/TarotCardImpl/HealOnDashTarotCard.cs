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
    [SerializeField] private int _hpGain = 5;

    private bool _isSubscribed;
    private HealthComponent _playerHealth;
    private TarotContext _context;
    private bool _isImmune;
    private float _cooldownEndTime;

    /// <summary>
    /// To apply tarot card, gets the player's health, and subscribes to dash and projectile events.
    /// It takes in TarotContext which holds objects needed for each tarot card to apply their changes.
    /// </summary>
    /// <param name="context">A class that holds objects needed for tarot cards.</param>
    public override void ApplyTarotCard(TarotContext context)
    {
        if (context.Player == null || !context.Player.TryGetComponent(out _playerHealth))
        {
            Debug.LogWarning("From HealOnDashTarotCard: player has no HealthComponent!");
            return;
        }

        _context = context;
        _isImmune = false;
        _cooldownEndTime = 0f;

        if (!_isSubscribed)
        {
            ThirdPersonController.OnDashStarted += HandleDashStarted;
            ThirdPersonController.OnDashEnded += HandleDashEnded;
            BaseProjectileBehavior.OnEnemyProjectileHitPlayer += HandleEnemyProjectileHitPlayer;
            _isSubscribed = true;
        }
    }

    /// <summary>
    /// To remove tarot card, unsubscribes from dash and projectile events and removes the immune modifier.
    /// </summary>
    public override void RemoveTarotCard()
    {
        if (_isSubscribed)
        {
            ThirdPersonController.OnDashStarted -= HandleDashStarted;
            ThirdPersonController.OnDashEnded -= HandleDashEnded;
            BaseProjectileBehavior.OnEnemyProjectileHitPlayer -= HandleEnemyProjectileHitPlayer;
            _isSubscribed = false;
        }

        HandleDashEnded();
    }

    /// <summary>
    /// A method that is called when the player dash started to apply damage immunity when required.
    /// </summary>
    /// <param name="startPosition">Position of the player.</param>
    /// <param name="rotation">Rotation of the player.</param>
    private void HandleDashStarted(Vector3 startPosition, Quaternion rotation)
    {
        if (Time.time < _cooldownEndTime)
        {
            return;
        }

        _isImmune = true;

        // Allows player to take 0 damage: damageTaken * (1 + -1) = 0
        _context.StatsHandler.AddModifier(Stat.DamageTaken, Operation.Multiply, -1f, this);
    }

    /// <summary>
    /// A method that is called when the player dash ends to remove playe damage immunity.
    /// </summary>
    private void HandleDashEnded()
    {
        if (!_isImmune)
        {
            return;
        }

        _isImmune = false;
        _context.StatsHandler.RemoveModifier(this);
    }

    /// <summary>
    /// A method that is called when an enemy projectile hits a player. 
    /// Heals the player if they have immunity and if the cooldown is over.
    /// </summary>
    private void HandleEnemyProjectileHitPlayer()
    {
        if (!_isImmune || Time.time < _cooldownEndTime)
        {
            return;
        }

        _playerHealth.Heal(_hpGain);
        _cooldownEndTime = Time.time + _cooldownDuration;
    }

    /// <summary>
    /// A method that resets the _isSubscribed field upon start.
    /// Since TarotCard's are scriptable objects, this field will be carried over.
    /// </summary>
    private void OnEnable()
    {
        _isSubscribed = false;
        _isImmune = false;
        _cooldownEndTime = 0f;
    }
}
