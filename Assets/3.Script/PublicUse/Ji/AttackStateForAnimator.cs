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
        if (action != null) action.isAttack = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (action != null) action.isAttack = false;
    }
}
