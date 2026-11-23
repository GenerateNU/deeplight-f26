using UnityEngine;

/// <summary>
/// The Pierce Effect Tarot Card which is in the Swords (speed and precision) suit under the Attack category.
/// When equipped, this Tarot Card does the following behavior: 
/// - Projectiles do not despawn when hitting an enemy.
/// - Instead they go through an enemy, dealing one hit of damage and keep traveling until they hit a wall or a non-enemy game object.
/// The goal of this tarot card is to have one projectile hit multiple enemies, aiding the player's gameplay
/// </summary>
[CreateAssetMenu(fileName = "ThePierceEffect", menuName = "Tarot/The Pierce Effect")]
public class PierceEffectTarotCard : TarotCard
{
    public override string CardName
    {
        get
        {
            Debug.Log("The Pierce Effect");
            return "The Pierce Effect";
        }
    }

    public override string CardDescription
    {
        get
        {
            Debug.Log("Player's projectiles can pierce through multiple enemies, dealing damage to all enemies it touches.");
            return "Player's projectiles can pierce through multiple enemies, dealing damage to all enemies it touches.";

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

    [SerializeField] private float _pierceCount = 99f;
    private TarotContext _context;

    /// <summary>
    /// To apply tarot card, adds the new pierce modifier to the player's stats.
    /// It takes in TarotContext which holds objects needed for each tarot card to apply their changes.
    /// </summary>
    /// <param name="context">A class that holds objects needed for tarot cards.</param>
    public override void ApplyTarotCard(TarotContext context)
    {
        _context = context;
        _context.StatsHandler.RemoveModifier(this); // in case the pierce modifier was already added. 
        _context.StatsHandler.AddModifier(Stat.ProjectilePierce, Operation.Add, _pierceCount, this);
        Debug.Log("Tarot Activated!");
    }

    /// <summary>
    /// To remove tarot card, removes the new pierce modifier from the player's stats.
    /// </summary>
    public override void RemoveTarotCard()
    {
        if (_context == null)
        {
            return;
        }

        _context.StatsHandler.RemoveModifier(this);
    }
}
