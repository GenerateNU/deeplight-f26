using System.Collections;
using UnityEngine;

/// <summary>
/// This script holds the standard logic for disabling gameObjects after a time limit.
/// </summary>
public class DisableObject : MonoBehaviour
{
    [SerializeField] private float duration = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        StartCoroutine(DisableAfterTime(duration));
    }

    /// <summary>
    /// This method disables a GameObject after a certain time limit.
    /// </summary>
    IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
