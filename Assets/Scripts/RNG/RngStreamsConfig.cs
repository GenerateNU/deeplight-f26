using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RngStreams", menuName = "Config/RngStreams")]
public class RngStreamsConfig : ScriptableObject
{
    [SerializeField] private int masterSeed = 0;

    [NonSerialized] public RngStream Combat = new RngStream();
    [NonSerialized] public RngStream Loot = new RngStream();
    [NonSerialized] public RngStream LevelGen = new RngStream();
    [NonSerialized] public RngStream Cosmetic = new RngStream();

    public void InitAll()
    {
        int baseSeed = masterSeed != 0 ? masterSeed : Environment.TickCount;
        Combat.Init(baseSeed ^ 0x1111);
        Loot.Init(baseSeed ^ 0x2222);
        LevelGen.Init(baseSeed ^ 0x3333);
        Cosmetic.Init(baseSeed ^ 0x4444);
    }
}