using UnityEngine;


/// <summary>
/// Test class for sample enemy
/// </summary>
public class TestEnemy : EnemyBase
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start(); // keeps starting navmesh setup
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); // keeps state machine logic
    }
}
