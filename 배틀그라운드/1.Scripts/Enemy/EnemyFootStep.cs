using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFootStep : MonoBehaviour
{
    public SoundList[] stepSoundLists;
    private int index;
    private Animator anim;
    private bool isLeftFootAhead;
    private bool isPlayedLeftFoot;
    private bool isPlayedRightFoot;
    private Vector3 leftFootIKPos;
    private Vector3 rightFootIKPos;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float t_factor = 0.15f;
        if(anim.velocity.magnitude > 1.4f)
        {
            if (Vector3.Distance(leftFootIKPos, anim.pivotPosition) <= t_factor && !isPlayedLeftFoot)
            {
                PlayFootStep();
                isPlayedLeftFoot = true;
                isPlayedRightFoot = false;
            }

            if (Vector3.Distance(rightFootIKPos, anim.pivotPosition) <= t_factor && !isPlayedRightFoot)
            {
                PlayFootStep();
                isPlayedLeftFoot = false;
                isPlayedRightFoot = true;
            }
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        leftFootIKPos = anim.GetIKPosition(AvatarIKGoal.LeftFoot);
        rightFootIKPos = anim.GetIKPosition(AvatarIKGoal.RightFoot);
    }

    private void PlayFootStep()
    {
        int t_oldIndex = index;
        while (t_oldIndex == index)
        {
            index = Random.Range(0, stepSoundLists.Length);
        }
        SoundManager.Instance.PlayOneShotEffect((int)stepSoundLists[index], transform.position, 1f);
    }
}
