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
    private PlayerAction action;
    private WeaponAbstract weapon;

    private Vector3 mousePosition;
    public float rotationSpeed = 10f;

    [SerializeField] private float moveSpeed = 5f;
    //[SerializeField] private float distance = 10f;
    private Coroutine coroutine;

    private void Awake()
    {
        TryGetComponent(out Input);
        TryGetComponent(out rb);
        TryGetComponent(out stats);
        TryGetComponent(out action);
        animator = GetComponentInChildren<Animator>();
        weapon = GetComponentInChildren<WeaponAbstract>(true);
    }

    private void OnEnable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.whenGoNextMap += movePlayerOverBridge;
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.whenGoNextMap -= movePlayerOverBridge;
        }
    }

    private void FixedUpdate()
    {
        // 1. GameManager의 isStop 상태 체크
        if (GameManager.instance != null && GameManager.instance.isStop)
        {
            // [추가] 물리적인 속도를 완전히 0으로 만들어야 미끄러지지 않습니다.
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // 멈췄을 때 애니메이션 파라미터를 즉시 0으로 (댐핑 인자 제거)
            if (coroutine == null)
            {
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveZ", 0);
            }
            return; // 이후 Move(), FocusOnMouse() 실행 안 함
        }

        // 대쉬 중에는 일반 이동/회전 로직 건너뜀

        if (stats.isDash) return;
        if (action.isKnockback) return;
        FocusOnMouse();
        Move();
    }

    public void Move()
    {
        if (stats.isDash) return;
        if (action.forStopMove) return;

        Vector2 moveInput = Input.MoveValue;

        // 입력이 없을 때도 애니메이션을 서서히(0.1f) Idle로 돌림
        if (moveInput.magnitude <= 0.1f)
        {
            animator.SetFloat("MoveX", 0, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveZ", 0, 0.1f, Time.deltaTime);
            return;
        }

        Vector3 movePos = new Vector3(moveInput.x, 0, moveInput.y) * stats.MoveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movePos);

        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        Vector3 localMoveDir = transform.InverseTransformDirection(moveDir);

        // 이동 중일 때는 부드럽게 애니메이션 적용
        animator.SetFloat("MoveX", localMoveDir.x, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveZ", localMoveDir.z, 0.1f, Time.deltaTime);
    }

    public IEnumerator Dash()
    {
        stats.isDash = true;
        // 1. 구르기 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("Roll");
        }

        Vector2 moveInput = Input.MoveValue;
        Vector3 dashDir;

        // 2. 방향 결정: 이동 키를 누르고 있다면 그 방향으로, 아니면 정면으로
        if (moveInput.magnitude > 0.1f)
        {
            dashDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        }
        else
        {
            dashDir = transform.forward;
        }


        Vector3 startPos = transform.position;
        Vector3 destPos = startPos + (dashDir * stats.DashLength);

        float timer = 0;
        while (timer < 1f)
        {
            timer += stats.DashSpeed * Time.deltaTime;

            transform.position = Vector3.Lerp(startPos, destPos, timer);
            //transform.LookAt(destPos);

            yield return null;
        }
        transform.position = destPos;

        stats.isDash = false;
        yield return new WaitForSeconds(stats.DashDelay);
        Input.coroutine = null;
    }

    public void FocusOnMouse()
    {
        if (stats.isDash) return;
        if (action.forStopRotate) return;

        mousePosition = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.MousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            //if(hit.transform.CompareTag("Ground") || hit.transform.CompareTag("Rava"))
            mousePosition = hit.point;
            mousePosition.y = transform.position.y;

            Vector3 dir = (mousePosition - transform.position).normalized;

            if (dir != Vector3.zero)
            {
                Quaternion mouseRotation = Quaternion.LookRotation(dir);

                transform.rotation = Quaternion.Slerp(transform.rotation, mouseRotation, Time.deltaTime * rotationSpeed * weapon.weaponData.attackSpeed);
            }
        }
    }

    private void movePlayerOverBridge(Vector3 destPos, Vector2Int dicKey)
    {
        if (coroutine != null) return;

        //coroutine = StartCoroutine(movePlayer_Co(destDir, distance, moveSpeed));
        coroutine = StartCoroutine(movePlayer_Co(destPos, moveSpeed, dicKey));
    }

    private IEnumerator movePlayer_Co(Vector3 destPos, float speed, Vector2Int dicKey)
    {
        //dir.y = 0;
        //dir = dir.normalized;

        Vector3 startPos = transform.position;
        //Vector3 destPos = startPos + dir * distance;

        action.forStopMove = true;
        action.forStopRotate = true;

        while ((rb.position - destPos).sqrMagnitude > 0.01f)
        {
            Vector3 before = transform.position;
            Vector3 after = Vector3.MoveTowards(before, destPos, speed * Time.deltaTime);

            rb.MovePosition(after);

            Vector3 delta = after - before;
            Vector3 moveDir = delta.sqrMagnitude > 0.01f ? delta.normalized : Vector3.zero;

            if (moveDir != Vector3.zero)
            {
                Vector3 localMoveDir = transform.InverseTransformDirection(moveDir);
                animator.SetFloat("MoveX", localMoveDir.x, 0.1f, Time.deltaTime);
                animator.SetFloat("MoveZ", localMoveDir.z, 0.1f, Time.deltaTime);

            }
            else
            {
                animator.SetFloat("MoveX", 0, 0.1f, Time.deltaTime);
                animator.SetFloat("MoveZ", 0, 0.1f, Time.deltaTime);
            }
            yield return null;
        }
        rb.MovePosition(destPos);

        animator.SetFloat("MoveX", 0);
        animator.SetFloat("MoveZ", 0);

        action.forStopMove = false;
        action.forStopRotate = false;

        GameManager.instance.whenPlayerArrived(dicKey);
        coroutine = null;
    }
}
