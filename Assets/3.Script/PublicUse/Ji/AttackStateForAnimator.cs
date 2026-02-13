using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateForAnimator : StateMachineBehaviour
{
    private PlayerAction action;
    private WeaponAbstract weapon;

    private bool isLastAttack = false;
    [Range(0f, 1f)] public float unlockRotationTime = 0.6f;
    [Range(0f, 1f)] public float lastAttackRotationTime = 0.9f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (action == null) action = animator.GetComponentInParent<PlayerAction>();
        if (action != null)
        {

            weapon = action.Equipment.MainWeapon;

            if (weapon.weaponID != WeaponID.Hammer && weapon != null)
            {
                int comboIndex = animator.GetInteger("ComboState");
                int maxCombo = weapon.weaponData.comboCount;

                isLastAttack = comboIndex >= maxCombo - 1;

                action.forStopMove = true;
                action.GetComponent<PlayerMovement>().FocusOnMouse();
                action.forStopRotate = true;
            }
            else
            {
                action.forStopMove = true;
                action.forStopRotate = false;
            }
            //if (!action.Equipment.MainWeapon.CanAttack())
            //{
            //    if (weapon.weaponID == WeaponID.Hammer) action.forStopRotate = false;
            //    else
            //    {
            //        action.forStopRotate = true;
            //    }
            //}
            //else
            //{
            //    action.forStopRotate = true;
            //}
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (action == null) return;
        if (weapon.weaponID == WeaponID.Hammer) return;

        if (animator.IsInTransition(layerIndex))
        {
            if (animator.GetNextAnimatorStateInfo(layerIndex).IsTag("Attack"))
            {
                action.forStopRotate = true;
                return;
            }
        }

        if (isLastAttack && (stateInfo.normalizedTime >= lastAttackRotationTime)) return;

        if (stateInfo.normalizedTime >= unlockRotationTime)
        {
            action.forStopRotate = false;
        }
        //if (isLastAttack) return;
        //
        //if (stateInfo.normalizedTime >= unlockRotationTime && !animator.IsInTransition(layerIndex))
        //{
        //    action.forStopRotate = false;
        //}
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (action == null) return;

        AnimatorStateInfo nextInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (animator.IsInTransition(layerIndex))
        {
            nextInfo = animator.GetNextAnimatorStateInfo(layerIndex);
        }

        if (nextInfo.IsTag("Attack")) return;

        action.forStopMove = false;
        action.forStopRotate = false;
    }
}
