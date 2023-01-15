using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reload Action", menuName = "Pluggable AI/Actions/Reload")]
public class ReloadAction : Action
{
    public override void Act(StateController p_controller)
    {
        if (!p_controller.isReloading && p_controller.bullets <= 0)
        {
            p_controller.enemyAnim.anim.SetTrigger(FC.AnimatorKey.Reload);
            p_controller.isReloading = true;
            SoundManager.Instance.PlayOneShotEffect((int)SoundList.reloadWeapon, p_controller.enemyAnim.muzzleTr.position, 2f);
        }
    }
}
