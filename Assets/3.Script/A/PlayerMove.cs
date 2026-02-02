using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float decelerationRate = 10f;

    private Rigidbody rb;
    private Vector3 movementInput;
    private Vector3 currentVelocity;
    private Animator animator;

    // 추가: 이미지 반전을 위해 SpriteRenderer를 가져옵니다.
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // 캐릭터 본인 또는 자식 오브젝트에 있는 SpriteRenderer를 찾습니다.
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 3D 환경에서 캐릭터가 넘어지지 않게 회전 고정
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // [통과 방지 팁] Rigidbody의 충돌 감지 모드를 연속(Continuous)으로 설정
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.z = Input.GetAxisRaw("Vertical");
        movementInput.Normalize();

        // [방향 전환 로직 변경] Scale 대신 FlipX 사용
        if (movementInput.x < 0)
        {
            // 왼쪽 이동: 이미지를 왼쪽으로 반전 (이미지가 원래 오른쪽을 본다면 true)
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }
        else if (movementInput.x > 0)
        {
            // 오른쪽 이동: 이미지 반전 해제
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }
        // *참고: 이미지가 원래 왼쪽을 보고 있다면 true/false를 반대로 적어주세요.

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
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero,
                                                  decelerationRate * Time.fixedDeltaTime);
        }

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }
}