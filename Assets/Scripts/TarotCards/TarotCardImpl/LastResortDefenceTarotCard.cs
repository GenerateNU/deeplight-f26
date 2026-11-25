using UnityEngine;
using System.Collections;

/// <summary>
/// The Last Resort Defence Tarot Card which is under the Passive category.
/// When equipped, this Tarot Card does the following behavior: 
/// - On reaching critical health (below 10 HP), take 50% less damage for 8 seconds.
/// - Only applies once per room.
/// The goal of this tarot card is to help keep the player alive when they are in critical health.
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

    [SerializeField] private float _damageDecreasePercent = 0.5f;
    [SerializeField] private float _duration = 8f;
    [SerializeField] private int _healthThreshold = 10;
    private bool _isSubscribed;
    private HealthComponent _playerHealth;
    private bool _usedThisRoom;
    private Coroutine _coroutine;
    private TarotContext _context;


    /// <summary>
    /// To apply tarot card, subscribes to the OnHealthChanged and OnRoomEntered events. 
    /// It takes in TarotContext which holds objects needed for each tarot card to apply their changes.
    /// </summary>
    /// <param name="context">A class that holds objects needed for tarot cards.</param>
    public override void ApplyTarotCard(TarotContext context)
    {
        if (context.Player == null || !context.Player.TryGetComponent(out _playerHealth))
        {
            Debug.LogWarning("From LastResortDefenceTarotCard: player has no HealthComponent!");
            return;
        }

        if (!_isSubscribed)
        {
            _playerHealth.OnHealthChanged += HandleHealthChanged;
            GameManager.OnRoomEntered += HandleRoomEntered;
            _isSubscribed = true;
        }

        _context = context;
        _usedThisRoom = false;
    }

    /// <summary>
    /// To remove tarot card, unsubscribes from the OnHealthChanged and OnRoomEntered, stops the 
    /// couroutine, and removes the modifier from the stats handler. 
    /// </summary>
    public override void RemoveTarotCard()
    {
        if (_isSubscribed)
        {
            _playerHealth.OnHealthChanged -= HandleHealthChanged;
            GameManager.OnRoomEntered -= HandleRoomEntered;
            _isSubscribed = false;
        }

        if (_context == null)
        {
            return;
        }

        if (_coroutine != null)
        {
            _context.StatsHandler.StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _context.StatsHandler.RemoveModifier(this);
    }

    /// <summary>
    /// A method that is called when the OnHealthChanged event occurs. 
    /// Decides whether the player should recieve reduced damage intake and starts the coroutine when needed.
    /// </summary>
    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        if (_usedThisRoom || _coroutine != null || currentHealth <= 0 || currentHealth >= _healthThreshold)
        {
            return;
        }

        _usedThisRoom = true;
        _coroutine = _context.StatsHandler.StartCoroutine(DamageReduction());
    }

    /// <summary>
    /// A coroutine that gives the player reduced damage intake for a limited time. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DamageReduction()
    {
        _context.StatsHandler.AddModifier(Stat.DamageTaken, Operation.Multiply, -_damageDecreasePercent, this);
        yield return new WaitForSeconds(_duration);
        _context.StatsHandler.RemoveModifier(this);

        _coroutine = null;
    }

    /// <summary>
    /// A method that is called when the OnRoomEntered event happens. 
    /// </summary>
    private void HandleRoomEntered()
    {
        _usedThisRoom = false;
    }

    /// <summary>
    /// A method that resets the _isSubscribed field upon start.
    /// Since TarotCard's are scriptable objects, this field will be carried over.
    /// </summary>
    private void OnEnable()
    {
        _isSubscribed = false;
        _usedThisRoom = false;
    }
}
