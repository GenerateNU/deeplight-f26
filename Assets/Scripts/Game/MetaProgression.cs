using UnityEngine;

/// <summary>
/// A class that keeps track of meta progression for our game.
/// Tracks information on the total runs played, won, lost, and the total enemies killed for the player's interest.
/// </summary>
public class MetaProgression : MonoBehaviour
{
    // TODO: Maybe we change this into a listener... might be better for making sure the data is secure
    public int totalRunsPlayed { get; private set; }
    public int totalRunsWon { get; private set; }
    public int totalRunsLost { get; private set; }
    public int totalEnemiesKilled { get; private set; }  // would be cool to keep track of

    /// <summary>
    /// Called by GameManager when the player wins a run.
    /// </summary>
    public void RecordWin()
    {
        totalRunsPlayed++;
        totalRunsWon++;
    }

    /// <summary>
    /// Called by GameManager when the player loses a run.
    /// </summary>
    public void RecordLoss()
    {
        totalRunsPlayed++;
        totalRunsLost++;
    }

    /// <summary>
    /// Called by GameManager when an enemy is killed.
    /// </summary>
    public void RecordEnemyKill()
    {
        totalEnemiesKilled++;
    }

    // TODO: MetaProgression data needs to be saved in an external file
    // When the game closes and opens, variables saved will be lost..
}
