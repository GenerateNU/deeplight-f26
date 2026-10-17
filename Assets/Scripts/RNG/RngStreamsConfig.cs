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

  /// <summary>
  /// All the systems that need independent RNG streams. Add new systems to the
  /// end of the enum.
  /// </summary>
  public enum RngStreamType
  {
    Combat,
    Loot,
    LevelGen,
    Cosmetic
  }

  [NonSerialized] private RngStream[] _streams;

  /// <summary>
  /// Creates a new baseseed, then initializes each stream with the baseseed.
  /// </summary>
  public void InitAll()
  {
    hasSetBaseSeed = false;
    _streams = new RngStream[Enum.GetNames(typeof(RngStreamType)).Length];
    SetBaseSeed();

    foreach (RngStreamType type in Enum.GetValues(typeof(RngStreamType)))
    {
      Get(type);
    }
  }

  /// <summary>
  /// Returns the stream for the given type, creating it if it hasn't been created yet
  /// </summary>
  public RngStream Get(RngStreamType type)
  {
    if (_streams == null)
    {
      _streams = new RngStream[Enum.GetNames(typeof(RngStreamType)).Length];
    }
    int i = (int)(type);

    if (_streams[i] != null)
    {
      return _streams[i];
    }
    SetBaseSeed();
    _streams[i] = new RngStream();
    _streams[i].Init(DeriveSeed(baseSeed, i));

    return _streams[i];
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