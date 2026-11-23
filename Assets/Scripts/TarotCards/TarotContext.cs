using UnityEngine;

/// <summary>
/// Holds the game objects and components that tarot cards need to apply their abilities.
///
/// One TarotContext is built and passed to TarotCard.ApplyTarotCard, so each card can
/// reach things like the player or their stats without looking them up itself.
/// When a new card needs access to something else, add it here as a field.
/// </summary>
public class TarotContext
{
    public GameObject Player;
    public StatHandler StatsHandler;
}
