using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChest : MonoBehaviour
{
    [Header("상자 설정")]
    [SerializeField] private Transform chestLid;
    [SerializeField] private ParticleSystem coinEffect;
    [SerializeField] private float openSpeed = 2f;

    [Header("드롭 아이템")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private int coinCount = 5;
    [SerializeField] private int heartCount = 1;
    [SerializeField] private float jumpForce = 7f;

    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private bool isOpened = false;

    // 생성된 모든 아이템의 콜라이더를 담아둘 리스트
    private List<Collider> spawnedColliders = new List<Collider>();

    private void Awake()
    {
        if (chestLid != null)
        {
            closedRotation = chestLid.localRotation;
            openedRotation = Quaternion.Euler(-130f, 0f, 0f);
        }
    }

    private void Start()
    {
        OnPlayerEnterRoom();
    }

    public void OnPlayerEnterRoom()
    {
        if (!isOpened)
        {
            StartCoroutine(OpenChestRoutine());
        }
    }

    public void ResetChest()
    {
        isOpened = false;
        if (chestLid != null) chestLid.localRotation = closedRotation;
    }

    private IEnumerator OpenChestRoutine()
    {
        isOpened = true;

        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * openSpeed;
            if (chestLid != null)
                chestLid.localRotation = Quaternion.Slerp(closedRotation, openedRotation, timer);
            yield return null;
        }

        if (coinEffect != null) coinEffect.Play();

        // 새로 뿌릴 때 리스트 초기화
        spawnedColliders.Clear();

        DropItems(coinPrefab, coinCount);
        DropItems(heartPrefab, heartCount);
    }

    private void DropItems(GameObject prefab, int count)
    {
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            // 스폰 위치 분산
            Vector3 randomOffset = new Vector3(Random.Range(-0.4f, 0.4f), 0, Random.Range(-0.4f, 0.4f));
            Vector3 spawnPos = transform.position + Vector3.up * 1.8f + randomOffset;

            GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);

            // [핵심] 새로 생성된 아이템과 기존 아이템들 간의 충돌 무시
            Collider currentCollider = item.GetComponent<Collider>();
            if (currentCollider != null)
            {
                foreach (Collider other in spawnedColliders)
                {
                    if (other != null) Physics.IgnoreCollision(currentCollider, other);
                }
                spawnedColliders.Add(currentCollider);
            }

            if (item.TryGetComponent(out ItemFloating floating))
            {
                floating.enabled = false;
            }

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 공중에서 멈추지 않도록 무작위 힘을 강하게 줌
                Vector3 jumpDir = new Vector3(
                    Random.Range(-1f, 1f),
                    1.5f,
                    Random.Range(-1f, 1f)
                ).normalized;
                rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);
            }

            StartCoroutine(EnableFloatingAfterLand(item));
        }
    }

    private IEnumerator EnableFloatingAfterLand(GameObject item)
    {
        // 바닥에 떨어질 충분한 시간 대기
        yield return new WaitForSeconds(1.2f);

        if (item != null)
        {
            if (item.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true;
            }

            // ItemFloating 스크립트가 있다면 활성화
            if (item.TryGetComponent(out ItemFloating floating))
            {
                // [중요] 바닥에 닿은 현재 위치를 플로팅의 시작 위치로 다시 설정
                // ItemFloating 스크립트 내부에 startPos를 갱신하는 public 메서드가 있으면 좋습니다.
                // 없다면 지금 위치에서 둥둥 뜨기 시작합니다.
                floating.enabled = true;
            }
        }
    }
}