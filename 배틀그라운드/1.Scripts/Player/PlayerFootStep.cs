using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStep : MonoBehaviour
{
    private Animator anim;

    public SoundList[] stepSounds;
    private int index;

    private Transform leftFoot, rightFoot;
    private float distance;
    private bool isGrounded; 
    private int isGroundedHash, isCoverHash, isAimHash, isCrouchHash;

    public enum EFoot { LEFT, RIGHT, }
    private EFoot step = EFoot.LEFT;
    private float oldDist, maxDist = 0f;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);

        isGroundedHash = Animator.StringToHash(FC.AnimatorKey.Grounded);
        isCoverHash = Animator.StringToHash(FC.AnimatorKey.Cover);
        isAimHash = Animator.StringToHash(FC.AnimatorKey.Aim);
        isCrouchHash = Animator.StringToHash(FC.AnimatorKey.Crouch);
    }

    private void Update()
    {
        if (!isGrounded && anim.GetBool(isGroundedHash)) PlayFootStep();
        
        isGrounded = anim.GetBool(isGroundedHash);

        float t_factor = 0.15f;
        if (isGrounded && anim.velocity.magnitude > 1.6f)
        {
            oldDist = maxDist;
            switch (step)
            {
                case EFoot.LEFT:
                    distance = leftFoot.position.y - transform.position.y;
                    maxDist = distance > maxDist ? distance : maxDist;
                    if (distance <= t_factor)
                    {
                        PlayFootStep();
                        step = EFoot.RIGHT;
                    }
                    break;

                case EFoot.RIGHT:
                    distance = rightFoot.position.y - transform.position.y;
                    maxDist = distance > maxDist ? distance : maxDist;
                    if (distance <= t_factor)
                    {
                        PlayFootStep();
                        step = EFoot.LEFT;
                    }
                    break;
            }
        }
    }

    private void PlayFootStep()
    {
        if (oldDist < maxDist) return;

        oldDist = maxDist = 0;
        
        int t_oldIndex = index;
        while (t_oldIndex == index) index = Random.Range(0, stepSounds.Length - 1);
        SoundManager.Instance.PlayOneShotEffect((int)stepSounds[index], transform.position, 0.2f);
    }
}
