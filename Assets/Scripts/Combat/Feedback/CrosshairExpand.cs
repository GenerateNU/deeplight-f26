using UnityEngine;
/// <summary>
/// Expands the crosshair when the player shoots a projectile
/// </summary>
public class CrosshairExpand : ShotFeedback
{
    [SerializeField] private float restScale = 1f;
    [SerializeField] private float shotBump = 1.7f;
    [SerializeField] private float maxScale = 2f;
    [SerializeField] private float returnSpeed = 12f;
    private float currentScale;
    private Vector3 baseScale;


    /// <summary>
    /// Sets the baseScale to the scale of the crosshair and initializes currentScale.
    /// </summary>
    private void Start()
    {
        baseScale = transform.localScale;
        currentScale = restScale;
    }

    /// <summary>
    /// Expands the crosshair when the player shoots, capped at the max scale.
    /// </summary>
    override protected void HandleShoot()
    {
        currentScale = Mathf.Min(currentScale + shotBump, maxScale);
    }

    /// <summary>
    /// resets the crosshair to its restScale.
    /// </summary>
    void Update()
    {
        currentScale = Mathf.MoveTowards(currentScale, restScale, returnSpeed * Time.deltaTime);
        transform.localScale = baseScale * currentScale;
    }
}
