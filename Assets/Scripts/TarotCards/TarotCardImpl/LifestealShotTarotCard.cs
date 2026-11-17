using UnityEngine;

/// <summary>
/// The Lifesteal Shot Tarot Card which is under the Attack category.
/// When equipped, this Tarot Card does the following behavior: 
/// - Every third projectile that hits an enemy restores 5 HP.
/// - Cooldown for 5 seconds.
/// The goal of this tarot card is to heal the player on every 3rd projectile that hits an enemy.
/// </summary>
[CreateAssetMenu(fileName = "TheLifestealShot", menuName = "Tarot/The Lifesteal Shot")]
public class LifestealShotTarotCard : TarotCard
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
    [SerializeField] private int _hpGain = 5;
    [SerializeField] private int _hitsToHeal = 3;
    private bool _isSubscribed;
    private HealthComponent _playerHealth;
    private int _projectileCounter = 0;
    private float _cooldownEndTime;

    /// <summary>
    /// To apply tarot card, subscribes to the OnPlayerProjectileHitEnemy event and get's the players HealthComponent.
    /// It takes in TarotContext which holds objects needed for each tarot card to apply their changes.
    /// </summary>
    /// <param name="context">A class that holds objects needed for tarot cards.</param>
    public override void ApplyTarotCard(TarotContext context)
    {
        if (context.Player == null || !context.Player.TryGetComponent(out _playerHealth))
        {
            Debug.LogWarning("From LifestealShotTarotCard: player has no HealthComponent!");
            return;
        }

        if (!_isSubscribed)
        {
            BaseProjectileBehavior.OnPlayerProjectileHitEnemy += HandlePlayerProjectileHitEnemy;
            _isSubscribed = true;
        }
    }

    /// <summary>
    /// To remove tarot card, unsubscribes from the OnPlayerProjectileHitEnemy event.
    /// </summary>
    public override void RemoveTarotCard()
    {
        if (_isSubscribed)
        {
            BaseProjectileBehavior.OnPlayerProjectileHitEnemy -= HandlePlayerProjectileHitEnemy;
            _isSubscribed = false;
        }
    }

    /// <summary>
    /// A method that gets called after this class subscribed to BaseProjectileBehavior's HandlePlayerProjectileHitEnemy event. 
    /// Adds health to the player for every 3 projectiles.
    /// </summary>
    private void HandlePlayerProjectileHitEnemy()
    {
        if (Time.time < _cooldownEndTime)
        {
            return;
        }

        _projectileCounter += 1;

        if (_projectileCounter >= _hitsToHeal)
        {
            _playerHealth.Heal(_hpGain);
            _projectileCounter = 0;
            _cooldownEndTime = Time.time + _cooldownDuration;
        }
    }

    /// <summary>
    /// A method that resets the _isSubscribe, _projectileCounter and _cooldownEndTime fields upon start.
    /// Since TarotCard's are scriptable objects, this field will be carried over.
    /// </summary>
    private void OnEnable()
    {
        _isSubscribed = false;
        _projectileCounter = 0;
        _cooldownEndTime = 0f;
    }
}
