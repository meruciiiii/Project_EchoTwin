using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerMovement Player;
    private PlayerAction Action;
    private PlayerStats stats;
    public Coroutine coroutine;

    private Vector2 moveValue;
    public Vector2 MoveValue => moveValue;

    private Vector2 mousePos;
    public Vector2 MousePos => mousePos;

    public bool isAttackPressed { get; private set; }

    private void Awake()
    {
        TryGetComponent(out Player);
        TryGetComponent(out Action);
        TryGetComponent(out stats);
    }
    public void Event_Move(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (GameManager.instance.isStop) return;
            if (Action.isPlayingAani) return;
            moveValue = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            moveValue = Vector2.zero;
        }
    }

    public void Event_Dash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (GameManager.instance.isStop) return;
            if (Action.isPlayingAani) return;
            if (coroutine != null) return;
            coroutine = StartCoroutine(Player.Dash());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {

        }
    }

    public void Event_Attack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isAttackPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isAttackPressed = false;
        }
    }

    public void Event_ChargingAttack(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {

        }
        else if(context.phase == InputActionPhase.Canceled)
        {

        }
    }

    public void Event_Curse(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {

        }
        else if (context.phase == InputActionPhase.Canceled)
        {

        }
    }

    public void Event_MousPos(InputAction.CallbackContext context)
    {
        if (GameManager.instance.isStop) return;
        mousePos = context.ReadValue<Vector2>();
    }

    public void Event_Interact(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            if (GameManager.instance.isStop) return;

            Action.onInteraction?.Invoke();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {

        }
    }
}
