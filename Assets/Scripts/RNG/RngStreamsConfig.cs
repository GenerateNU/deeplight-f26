using System;
using UnityEngine;

/// <summary>
/// Creates independent rng streams for combat, loot, levelgen and cosmetic
/// </summary>
[CreateAssetMenu(fileName = "RngStreams", menuName = "Config/RngStreams")]
public class RngStreamsConfig : ScriptableObject
{
  [SerializeField] private int masterSeed = 0;
  [SerializeField] private bool useFixedSeed = false;
  [NonSerialized] private int baseSeed;
  [NonSerialized] private bool hasSetBaseSeed = false;

  [NonSerialized] private RngStream _combat;
  [NonSerialized] private RngStream _loot;
  [NonSerialized] private RngStream _levelGen;
  [NonSerialized] private RngStream _cosmetic;

  /// <summary>
  /// Creates a new baseseed, then initializes each stream with the baseseed.
  /// </summary>
  public void InitAll()
  {
    hasSetBaseSeed = false;
    _combat = null;
    _loot = null;
    _levelGen = null;
    _cosmetic = null;
    SetBaseSeed();

    _ = Combat;
    _ = Loot;
    _ = LevelGen;
    _ = Cosmetic;
  }

  /// <summary>
  /// Returns the Combat stream, creating it if it hasn't been created yet
  /// </summary>
  public RngStream Combat
  {
    get
    {
      if (_combat == null)
      {
        _combat = new RngStream();
        SetBaseSeed();
        _combat.Init(DeriveSeed(baseSeed, 0));
      }
      return _combat;
    }
  }

  /// <summary>
  /// Returns the Loot stream, creating it if it hasn't been created yet
  /// </summary>
  public RngStream Loot
  {
    get
    {
      if (_loot == null)
      {
        _loot = new RngStream();
        SetBaseSeed();
        _loot.Init(DeriveSeed(baseSeed, 1));
      }
      return _loot;
    }
  }

  /// <summary>
  /// Returns the LevelGen stream, creating it if it hasn't been created yet
  /// </summary>
  public RngStream LevelGen
  {
    get
    {
      if (_levelGen == null)
      {
        _levelGen = new RngStream();
        SetBaseSeed();
        _levelGen.Init(DeriveSeed(baseSeed, 2));
      }
      return _levelGen;
    }
  }

  /// <summary>
  /// Returns the Cosmetic stream, creating it if it hasn't been created yet
  /// </summary>
  public RngStream Cosmetic
  {
    get
    {
      if (_cosmetic == null)
      {
        _cosmetic = new RngStream();
        SetBaseSeed();
        _cosmetic.Init(DeriveSeed(baseSeed, 3));
      }
      return _cosmetic;
    }
  }

  /// <summary>
  /// creates a stream's seed from the baseseed using SplitMix64 hashing, streamIndex is unique
  /// for each independent stream.
  /// </summary>
  private static int DeriveSeed(int baseSeed, int streamIndex)
  {
    // Pack both inputs into one 64-bit value
    ulong x = ((ulong)(uint)baseSeed << 32) | (uint)streamIndex;

    // SplitMix64: scrambles the bits
    x += 0x9E3779B97F4A7C15UL;
    x = (x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL;
    x = (x ^ (x >> 27)) * 0x94D049BB133111EBUL;
    x ^= x >> 31;

    // only keep the top 32 bits
    return (int)(x >> 32);
  }

  /// <summary>
  /// If a base seed hasn't been set, it sets the base seed to the master seed if useFixedSeed  
  /// is true or creates a time-based base seed otherwise
  /// </summary>
  private void SetBaseSeed()
  {
    if (!hasSetBaseSeed)
    {
      if (useFixedSeed)
        baseSeed = masterSeed;
      else
        baseSeed = Environment.TickCount;
      hasSetBaseSeed = true;
      Debug.Log("Current base seed: " + baseSeed);
    }
  }
}