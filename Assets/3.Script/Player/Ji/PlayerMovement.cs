using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;
    private InputManager Input;
    private Rigidbody rb;
    private Animator animator; // 애니메이터 추가

    private Vector3 mousePos;

    private void Awake()
    {
        TryGetComponent(out Input);
        TryGetComponent(out rb);
        TryGetComponent(out stats);
        animator = GetComponentInChildren<Animator>();
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

        // 2. 중요: moveDir를 만들 때 Vector2의 y를 Vector3의 z에 넣어줘야 합니다!
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        if (moveDir.magnitude>0.1f)
        {
            Vector3 localMoveDir = transform.InverseTransformDirection(moveDir);

            //animator localMoveDir.x -> 좌우 움직임 애니메이터
            //animator localMoveDir.z -> 앞뒤 움직임 애니매이터

            // 0.1f는 부드러운 애니메이션 전환을 위한 댐핑 값입니다.
            animator.SetFloat("MoveX", localMoveDir.x, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveZ", localMoveDir.z, 0.1f, Time.deltaTime);

        }
        else
        {
            // 멈췄을 때 애니메이션도 0으로 부드럽게 복귀
            animator.SetFloat("MoveX", 0, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveZ", 0, 0.1f, Time.deltaTime);
        }
    }

    public IEnumerator Dash()
    {
        stats.isDash = true;
        Vector2 dashPos = Input.MoveValue.normalized * stats.DashLength;
        Vector3 destPos = new Vector3(transform.position.x + dashPos.x, transform.position.y, transform.position.z + dashPos.y);
        Vector3 startPos = transform.position;
        float timer = 0;
        while (timer < 1f)
        {
            timer += stats.DashSpeed* Time.deltaTime;

            transform.position = Vector3.Lerp(startPos, destPos, timer);

            yield return null;
        }
        transform.position = destPos;

        stats.isDash = false;
        yield return new WaitForSeconds(stats.DashDelay);
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
