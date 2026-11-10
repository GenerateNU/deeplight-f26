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

    /// <summary>
    /// An abstract method that each tarot card will implement for their special abilities
    /// </summary>
    public override void ApplyTarotCard()
    {
        Debug.Log("Tarot Activated!");
    }
}
