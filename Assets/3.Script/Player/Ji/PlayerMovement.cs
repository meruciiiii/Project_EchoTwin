using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;
    private InputManager Input;
    private Rigidbody rb;

    private bool isDash = false;

    private Vector3 mousePos;

    private void Awake()
    {
        TryGetComponent(out Input);
        TryGetComponent(out rb);
        TryGetComponent(out stats);
    }

    private void FixedUpdate()
    {
        Move();
        FocusOnMouse();
    }

    public void Move()
    {
        Vector2 moveInput = Input.MoveValue;
        Vector3 movePos = new Vector3(moveInput.x, 0, moveInput.y) * stats.MoveSpeed * Time.deltaTime * GameManager.instance.timeScale;

        rb.MovePosition(transform.position + movePos);
    }

    public IEnumerator Dash()
    {
        isDash = true;
        Vector2 dashPos = Input.MoveValue * stats.DashLength;
        Vector3 destPos = new Vector3(transform.position.x + dashPos.x, 0, transform.position.z + dashPos.y);

        float timer = 0;
        while (timer < 1f)
        {
            timer += stats.DashSpeed* Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, destPos, timer);

            yield return null;
        }
        transform.position = destPos;

        yield return new WaitForSeconds(stats.DashDelay);
        isDash = false;
    }

    private void FocusOnMouse()
    {
        mousePos = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.MousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            mousePos = hit.point * GameManager.instance.timeScale;
        }
        mousePos.y = transform.position.y;
        transform.LookAt(mousePos);
    }
}
