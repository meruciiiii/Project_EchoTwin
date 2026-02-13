using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateForAnimator : StateMachineBehaviour
{
    private PlayerAction action;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (action == null) action = animator.GetComponentInParent<PlayerAction>();
        if (action != null)
        {
            Debug.Log($"{action.gameObject.name}");
            action.forStopMove = true;
            if(action.Equipment.MainWeapon.weaponID != WeaponID.Hammer) action.forStopRotate = true;
            else
            {
                action.forStopRotate = false;
            }
        }
        else Debug.Log($"action is null in animator");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (action == null) return;

        AnimatorStateInfo nextInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (nextInfo.IsTag("Attack")) return;

        action.forStopMove = false;
        action.forStopRotate = false;
    }
}
