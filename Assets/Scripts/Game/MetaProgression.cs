using System;
using System.IO;
using UnityEngine;

/// <summary>
/// A class that keeps track of meta progression for our game.
/// Tracks information on the total runs played, won, lost, and the total enemies killed for the
/// player's interest. Persists to a JSON file in Application.persistentDataPath.
/// </summary>
public class MetaProgression : MonoBehaviour
{
    public int totalRunsPlayed { get; private set; }
    public int totalRunsWon { get; private set; }
    public int totalRunsLost { get; private set; }
    public int totalEnemiesKilled { get; private set; }

    /// <summary>
    /// Plain serialisable mirror of the stats. JsonUtility cannot serialise properties, so the
    /// values are copied into this on save and back out on load.
    /// </summary>
    [Serializable]
    private class SaveData
    {
        public int totalRunsPlayed;
        public int totalRunsWon;
        public int totalRunsLost;
        public int totalEnemiesKilled;
    }

    private const string FileName = "meta_progression.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, FileName);

    private bool isDirty;

    /// <Summary>
    /// Runs on awake
    /// </Summary>
    private void Awake()
    {
        Load();
    }

    /// <Summary>
    /// Runs on enable
    /// </Summary>
    private void OnEnable()
    {
        GameManager.OnRunEnded += HandleRunEnded;
        GameManager.OnEnemyKilled += HandleEnemyKilled;
    }

    /// <Summary>
    /// Runs on disable
    /// </Summary>
    private void OnDisable()
    {
        GameManager.OnRunEnded -= HandleRunEnded;
        GameManager.OnEnemyKilled -= HandleEnemyKilled;
    }

    /// <Summary>
    /// Increments total runs played, won and lost, then saves it
    /// </Summary>
    /// <param name="won"> whether the player won the run or not
    private void HandleRunEnded(bool won)
    {
        totalRunsPlayed++;
        if (won) totalRunsWon++;
        else totalRunsLost++;

        // End of run is also the natural flush point for the kill count accumulated below.
        Save();
    }

    private void HandleEnemyKilled()
    {
        totalEnemiesKilled++;
        isDirty = true;
    }

    /// <summary>
    /// Writes the current stats to disk.
    /// </summary>
    public void Save()
    {
        SaveData data = new SaveData
        {
            totalRunsPlayed = totalRunsPlayed,
            totalRunsWon = totalRunsWon,
            totalRunsLost = totalRunsLost,
            totalEnemiesKilled = totalEnemiesKilled,
        };

        try
        {
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            isDirty = false;
        }
        catch (Exception e)
        {
            Debug.LogError($"[MetaProgression] Could not save to {SavePath}: {e.Message}", this);
        }
    }

    /// <summary>
    /// Reads the stats back from disk. A missing or corrupt file just leaves everything at zero,
    /// which is the correct state for a first launch anyway.
    /// </summary>
    private void Load()
    {
        if (!File.Exists(SavePath)) return;

        try
        {
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            if (data == null) return;

            totalRunsPlayed = data.totalRunsPlayed;
            totalRunsWon = data.totalRunsWon;
            totalRunsLost = data.totalRunsLost;
            totalEnemiesKilled = data.totalEnemiesKilled;
        }
        catch (Exception e)
        {
            Debug.LogError($"[MetaProgression] Could not load from {SavePath}: {e.Message}", this);
        }
    }

    // OnApplicationQuit does not fire reliably when a build is force-closed, which
    // is why the win/loss records save immediately. These are the safety net for kill counts.
    private void OnApplicationQuit()
    {
        if (isDirty) Save();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused && isDirty) Save();
    }
}
