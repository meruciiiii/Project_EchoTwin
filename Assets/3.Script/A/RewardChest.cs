using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChest : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField] private Transform chestLid;
    [SerializeField] private ParticleSystem coinEffect;
    [SerializeField] private float openSpeed = 2f;

    [Header("��� ������")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject crystalPrefab; 
    [SerializeField] private int coinCount = 5;
    [SerializeField] private int heartCount = 1;
    [SerializeField] private int crystalCount = 3;     
    [SerializeField] private float jumpForce = 7f;

    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private bool isOpened = false;

    private List<Collider> spawnedColliders = new List<Collider>();
    private List<GameObject> spawnedItems = new List<GameObject>();
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
        StartCoroutine(SubscribeEvent_Co());
    }
    private IEnumerator SubscribeEvent_Co()
    {
        while (GameManager.instance == null) yield return null;
        
        GameManager.instance.whenNodeClear -= ResetChest;
        GameManager.instance.whenNodeClear += ResetChest;
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.whenNodeClear -= ResetChest;
        }
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
        if (isOpened)
        {
            isOpened = false;
            if (chestLid != null) chestLid.localRotation = closedRotation;

            foreach (GameObject item in spawnedItems)
            {
                if (item != null) Destroy(item);
            }
            spawnedItems.Clear();

        }
    }
    private void ClearSpawnedItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        spawnedItems.Clear(); // 리스트 비우기
    }
    private IEnumerator OpenChestRoutine()
    {
        isOpened = true;
        SoundManager.SendEvent(SoundType.SFX_Chest);

        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * openSpeed;
            if (chestLid != null)
                chestLid.localRotation = Quaternion.Slerp(closedRotation, openedRotation, timer);
            yield return null;
        }

        if (coinEffect != null) coinEffect.Play();

        spawnedColliders.Clear();

        DropItems(coinPrefab, coinCount);
        DropItems(heartPrefab, heartCount);
        DropItems(crystalPrefab, crystalCount); 
    }

        private void DropItems(GameObject prefab, int count)
    {
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-0.4f, 0.4f), 0, Random.Range(-0.4f, 0.4f));
            Vector3 spawnPos = transform.position + Vector3.up * 1.8f + randomOffset;

            GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);
            spawnedItems.Add(item);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 jumpDir = new Vector3(
                    Random.Range(-1f, 1f),
                    1.5f,
                    Random.Range(-1f, 1f)
                ).normalized;
                rb.AddForce(jumpDir * jumpForce, ForceMode.Impulse);
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOpened) return;

        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.gamestate == GameManager.GameState.Playing)
            {
                OnPlayerEnterRoom();
            }
        }
    }


}