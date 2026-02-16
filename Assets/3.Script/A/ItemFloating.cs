using UnityEngine;

public class ItemFloating : MonoBehaviour
{
    [Header("회전 속도")]
    [SerializeField] private float rotateSpeed = 50f;

    [Header("위아래 이동 높이")]
    [SerializeField] private float floatHeight = 0.5f;

    [Header("위아래 이동 속도")]
    [SerializeField] private float floatSpeed = 2f;

    // 아이템의 처음 시작 위치를 기억합니다.
    private Vector3 startPos;

    private void Start()
    {
        // 게임이 시작될 때 현재 위치를 저장해둡니다.
        startPos = transform.position;
    }

    private void Update()
    {
        // 1. 빙글빙글 돌리기
        // 매 프레임마다 Y축(세로축)을 기준으로 조금씩 회전시킵니다.
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        // 2. 둥둥 뜨게 만들기
        // Sin 함수를 사용하면 값이 1에서 -1 사이로 부드럽게 왔다 갔다 합니다.
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // 처음 시작 위치에서 Sin 값만큼 Y축 위치를 더해줍니다.
        transform.position = new Vector3(startPos.x, startPos.y + newY, startPos.z);
    }
}