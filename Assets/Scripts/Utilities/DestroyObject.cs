using UnityEngine;

/// <summary>
/// This script holds the standard logic for destroying gameObjects after a time limit.
/// </summary>
public class DestroyObject : MonoBehaviour
{
    [SerializeField] private float duration = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, duration);
    }
}
