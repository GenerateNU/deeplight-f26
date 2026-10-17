using UnityEngine;

public class RngTestScript : MonoBehaviour
{
    [SerializeField] private RngStreamsConfig rngConfig;

    void Start()
    {
        Debug.Log("Random number: " + rngConfig.Get(RngStreamsConfig.RngStreamType.Combat).Range(0, 100));

        for (int i = 0; i < 1000; i++) rngConfig.Get(RngStreamsConfig.RngStreamType.Combat).Value();
        Debug.Log("Loot value: " + rngConfig.Get(RngStreamsConfig.RngStreamType.Loot).Value());
    }
}
