using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerMovement Player;
    private ItemPickup pickup;

    private Vector2 moveValue;
    public Vector2 MoveValue => moveValue;

    private Vector2 mousePos;
    public Vector2 MousePos => mousePos;

    private void Awake()
    {
        TryGetComponent(out Player);
    }
    public void Event_Move(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
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
            StartCoroutine(Player.Dash());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {

        }
    }

    public void Event_Attack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {

        }
        else if (context.phase == InputActionPhase.Canceled)
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
        mousePos = context.ReadValue<Vector2>();
    }

    public void Event_GetItem(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            pickup.GetNewWeapon();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {

        }
    }
}
