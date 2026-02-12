using System.Collections;
using UnityEngine;

public class RewardChest : MonoBehaviour
{
    [Header("상자 설정")]
    [SerializeField] private Transform chestLid;       // 상자 뚜껑 (X축 -130도 회전용)
    [SerializeField] private ParticleSystem coinEffect; // 동전 파티클
    [SerializeField] private float openSpeed = 2f;      // 뚜껑 열리는 속도

    [Header("드롭 아이템")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private int coinCount = 5;
    [SerializeField] private int heartCount = 1;
    [SerializeField] private float jumpForce = 5f;

    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private bool isOpened = false;

    private void Awake()
    {
        // 뚜껑 각도 설정
        if (chestLid != null)
        {
            closedRotation = chestLid.localRotation;
            openedRotation = Quaternion.Euler(-130f, 0f, 0f);
        }
    }

    private void Start()
    {
        // 테스트용: 게임 시작 시 상자 오픈
        OnPlayerEnterRoom();
    }

    // [구역: 유저가 방에 들어올 때 호출]
    public void OnPlayerEnterRoom()
    {
        if (!isOpened)
        {
            StartCoroutine(OpenChestRoutine());
        }
    }

    // [구역: 유저가 방을 나갈 때 초기화]
    public void ResetChest()
    {
        isOpened = false;
        if (chestLid != null)
        {
            chestLid.localRotation = closedRotation;
        }
    }

    private IEnumerator OpenChestRoutine()
    {
        isOpened = true;

        // 1. 뚜껑 부드럽게 열기
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * openSpeed;
            if (chestLid != null)
            {
                chestLid.localRotation = Quaternion.Slerp(closedRotation, openedRotation, timer);
            }
            yield return null;
        }

        // 2. 동전 파티클 실행
        if (coinEffect != null) coinEffect.Play();

        // 3. 아이템 생성 및 발사
        DropItems(coinPrefab, coinCount);
        DropItems(heartPrefab, heartCount);
    }

    private void DropItems(GameObject prefab, int count)
    {
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            // 1. 아이템끼리 겹치지 않게 미세한 랜덤 오프셋 추가
            Vector3 randomOffset = new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));
            Vector3 spawnPos = transform.position + Vector3.up * 1.5f + randomOffset;

            GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);

            // 2. 물리 발사 전 플로팅 끄기
            if (item.TryGetComponent(out ItemFloating floating))
            {
                floating.enabled = false;
            }

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 3. 발사 방향도 조금 더 다양하게 분산
                Vector3 jumpDir = new Vector3(
                    Random.Range(-1f, 1f),
                    2f, // 위로 더 확실히 띄움
                    Random.Range(-1f, 1f)
                ).normalized;

                rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);
            }

            StartCoroutine(EnableFloatingAfterLand(item));
        }
    }

    private IEnumerator EnableFloatingAfterLand(GameObject item)
    {
        yield return new WaitForSeconds(1f);

        if (item != null)
        {
            if (item.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true;
            }

            if (item.TryGetComponent(out ItemFloating floating))
            {
                floating.enabled = true;
            }
        }
    }
}