using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float decelerationRate = 10f; // 미끄러지는 거리

    private Rigidbody rb;
    private Vector3 movementInput;
    private Vector3 currentVelocity;
    private Animator animator;

    // 추가: 캐릭터의 원래 크기를 저장할 변수
    private Vector3 initialScale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // [중요] 시작할 때 현재 에디터에 설정된 캐릭터의 크기를 딱 저장해둡니다.
        initialScale = transform.localScale;

        // 3D 환경에서 캐릭터가 넘어지지 않게 회전 고정
        // [수정] X, Y, Z 모든 회전을 물리 엔진으로부터 보호합니다.
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.z = Input.GetAxisRaw("Vertical");
        movementInput.Normalize();

        // 방향 전환 로직: 기억해둔 initialScale을 활용합니다.
        if (movementInput.x < 0)
        {
            // 왼쪽 이동: X값만 원래 크기 그대로 (정방향)
            transform.localScale = new Vector3(initialScale.x, initialScale.y, initialScale.z);
        }
        else if (movementInput.x > 0)
        {
            // 오른쪽 이동: X값에만 마이너스(-)를 붙여서 반전
            transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
        }

        bool isMoving = movementInput.magnitude > 0;
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }

    void FixedUpdate()
    {
        if (movementInput.magnitude > 0)
        {
            currentVelocity = movementInput * moveSpeed;
        }
        else
        {
            // Vector3.MoveTowards를 사용하여 관성 구현
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero,
                                                  decelerationRate * Time.fixedDeltaTime);
        }

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }
}