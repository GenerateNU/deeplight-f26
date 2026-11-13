using UnityEngine;
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

    private float _damageBoostMultiplier = 1.5f;

    /// <summary>
    /// An abstract method that each tarot card will implement for their special abilities
    /// </summary>
    public override void ApplyTarotCard(TarotContext ctx)
    {
        Debug.Log("Tarot Activated!");
        ctx.Stats.AddModifier(Stat.AttackDamage, Operation.Multiply, _damageBoostMultiplier, this);
    }
}
