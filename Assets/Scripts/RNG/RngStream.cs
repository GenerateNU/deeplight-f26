using UnityEngine;
using System;

/// <summary>
/// Contains logic for initializing an independent stream and generating random numbers
/// </summary>
public class RngStream
{
    private System.Random _rng;

    /// <summary>
    /// Initializes a rng stream using a given seed. Every stream with the same seed produces  
    /// the same output
    /// </summary>
    public void Init(int seed)
    {
        _rng = new System.Random(seed);
    }

    /// <summary>
    /// Generates a whole number from minInclusive to maxExclusive, not including
    /// maxExclusive
    /// </summary>
    public int Range(int minInclusive, int maxExclusive) => _rng.Next(minInclusive, maxExclusive);
    
    /// <summary>
    /// Generates a decimal from 0 to 1
    /// </summary>
    public float Value() => (float)_rng.NextDouble();

    /// <summary>
    /// Generates the output "true" with the probability of chance
    /// </summary>
    public bool Chance(float chance) => _rng.NextDouble() < chance;
}
