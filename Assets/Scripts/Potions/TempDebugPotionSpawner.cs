using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A Temporary Debug script. can be deleted once breakables / real drops exist.
/// Spawns a random potion pickup near the player whenever the debug key (Q by default) is pressed.
/// Only exists to test PotionDropper without needing breakables.
/// </summary>
public class TempDebugPotionSpawner : MonoBehaviour
{
    [Header("debug testing")]
    [SerializeField] private PotionDropper dropper;
    [SerializeField] private Transform player;
    [SerializeField] private Key debugKey = Key.Q;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0.5f, 2f);

    /// <summary>
    /// Checks for the debug key each frame
    /// </summary>
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current[debugKey].wasPressedThisFrame)
        {
            SpawnNearPlayer();
        }
    }

    /// <summary>
    /// Moves the dropper in front of the player and calls Drop(), which skips the drop chance roll
    /// </summary>
    private void SpawnNearPlayer()
    {
        if (dropper == null || player == null)
        {
            Debug.LogWarning("TempDebugPotionSpawner needs a PotionDropper and a player Transform.", this);
            return;
        }

        dropper.transform.position = player.position + player.TransformDirection(spawnOffset);
        dropper.Drop();
    }
}
