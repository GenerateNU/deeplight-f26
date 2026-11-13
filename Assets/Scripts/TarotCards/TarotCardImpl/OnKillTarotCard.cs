using UnityEngine;
using System.Collections;

/// <summary>
/// The On Kill Tarot Card which is in the Wands (offense) suit under the Attack category.
/// When equipped, this Tarot Card does the following behavior: 
/// - When the player gets a kill, they gain a 1.5x damage boost for 3 seconds, with a 5 second cooldown.
/// The goal of this tarot card is to have increase the player's damage to enemies during specific time periods.
/// </summary>
[CreateAssetMenu(fileName = "TheOnKill", menuName = "Tarot/The On Kill")]
public class OnKillTarotCard : TarotCard
{
    public override string CardName
    {
        get
        {
            Debug.Log("The On Kill");
            return "The On Kill";
        }
    }

    public override string CardDescription
    {
        get
        {
            Debug.Log("When the player gets a kill, they gain a 1.5x damage boost for 3 seconds with a 5 second cooldown.");
            return "When the player gets a kill, they gain a 1.5x damage boost for 3 seconds with a 5 second cooldown.";

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

    [SerializeField] private float _damageBoostPercent = 0.5f;
    [SerializeField] private float _boostDuration = 3f;
    [SerializeField] private float _cooldownDuration = 5f;

    private TarotContext _context;
    private bool _buffActive;
    private float _cooldownEndTime;
    private Coroutine _damageBoostRoutine; // tracked so we can stop the coroutine when removing the tarot card.
    private bool _isSubscribed;

    /// <summary>
    /// An abstract method that each tarot card will implement for their special abilities.
    /// It takes in TarotContext which holds objects needed for each tarot card to apply their changes.
    /// </summary>
    /// <param name="context">A class that holds objects needed for tarot cards.</param>
    public override void ApplyTarotCard(TarotContext context)
    {
        _context = context;
        _buffActive = false;
        _cooldownEndTime = 0f;

        if (!_isSubscribed)
        {
            GameManager.OnEnemyKilled += HandleEnemyKilled; // add tarot card as listener for this method
            _isSubscribed = true;
        }
    }

    /// <summary>
    /// An abstract method that each tarot card will implement to remove their special abilites when a run ends.
    /// </summary>
    public override void RemoveTarotCard()
    {
        if (_context == null)
        { return; }

        if (_isSubscribed)
        {
            GameManager.OnEnemyKilled -= HandleEnemyKilled;
            _isSubscribed = false;
        }
        _context.StatsHandler.RemoveModifier(this);
        _buffActive = false;

        if (_damageBoostRoutine != null)
        {
            _context.StatsHandler.StopCoroutine(_damageBoostRoutine);
            _damageBoostRoutine = null;
        }
    }

    /// <summary>
    /// A method that gets called after this class subscribed to GameManager's OnEnemyKilled event. 
    /// Adds the modifier to StatsHandler.
    /// </summary>
    // TODO: This is public for TESTING purposes. When testing done, CHANGE TO PRIVATE.
    public void HandleEnemyKilled()
    {
        if (_buffActive || Time.time < _cooldownEndTime)
        {
            return;
        }

        /* NOTE: since TarotCards are scriptable objects, they can NOT start coroutines themselves.
           So here we are borrowing StatsHandler's coroutine to run this. 
           I decided to use Coroutines because tick based logic would need this class to have an update method
           which I felt would complicate things.   
        */
        _damageBoostRoutine = _context.StatsHandler.StartCoroutine(DamageBoost());
    }

    /// <summary>
    /// A coroutine that allows a damage boost for a time and adds a cooldown after.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DamageBoost()
    {
        _buffActive = true;
        _context.StatsHandler.AddModifier(Stat.AttackDamage, Operation.Multiply, _damageBoostPercent, this);
        Debug.Log("On Kill Tarot Card Applied");

        yield return new WaitForSeconds(_boostDuration);

        _context.StatsHandler.RemoveModifier(this);
        _buffActive = false;
        _cooldownEndTime = Time.time + _cooldownDuration;
        Debug.Log("On Kill Tarot Card not applied (cooling down)");
    }

    /// <summary>
    /// A method that resets the _isSubscribed field to false upon start.
    /// Since TarotCard's are scriptable objects, this field will be carried over.
    /// </summary>
    private void OnEnable()
    {
        _isSubscribed = false;
    }
}
