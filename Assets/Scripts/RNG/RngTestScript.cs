using UnityEngine;

public class RngTestScript : MonoBehaviour
{
    [SerializeField] private RngStreamsConfig rngConfig;

    void Start()
    {
        Debug.Log("Random number: " + rngConfig.Combat.Range(0, 100));

        for (int i = 0; i < 1000; i++) rngConfig.Combat.Value();
        Debug.Log("Loot value: " + rngConfig.Loot.Value());
    }
}
