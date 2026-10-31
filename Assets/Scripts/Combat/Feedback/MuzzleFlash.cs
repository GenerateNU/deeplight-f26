using UnityEngine;

/// <summary>
/// A muzzle flash for when the player shoots a projectile
/// </summary>
public class MuzzleFlash : ShotFeedback
{
    [SerializeField] GameObject flash;
    [SerializeField] float duration = .05f;
    private float timer;

    /// <summary>
    /// applies the flash when the player shoots.
    /// </summary>
    override protected void HandleShoot()
    {
        flash.SetActive(true);
        timer = duration;
    }

    /// <summary>
    /// counts down the flash timer and hides the flash when it runs out.
    /// </summary>
    private void Update()
    {
        if (timer <= 0f)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            flash.SetActive(false);
        }
    }
}
