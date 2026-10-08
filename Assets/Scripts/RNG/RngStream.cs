using UnityEngine;
using System;

public class RngStream
{
    private System.Random _rng;

    public void Init(int seed)
    {
        _rng = new System.Random(seed);
    }

    public int NextInt(int minInclusive, int maxExclusive) => _rng.Next(minInclusive, maxExclusive);
    public float NextFloat() => (float)_rng.NextDouble();
    public bool NextBool(float chance) => _rng.NextDouble() < chance;
}
