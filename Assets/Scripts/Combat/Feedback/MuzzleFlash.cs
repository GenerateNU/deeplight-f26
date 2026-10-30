using UnityEngine;

public class MuzzleFlash : ShotFeedback
{
    [SerializeField] GameObject flash;
    [SerializeField] float duration = .05;
    override protected void HandleShoot()
    {
        ///flash.enable();
        ///start timer
    }

}
