using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealCampController : MonoBehaviour
{
    [Header("생성 위치 (자식 1, 2, 3)")]
    [SerializeField] private Transform[] spawnPoints = new Transform[3];

    [Header("생성할 아이템 프리팹")]
    [SerializeField] private GameObject itemPrefab;

    private List<GameObject> spawnedItems = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SubscribeEvent_Co());
    }

    private IEnumerator SubscribeEvent_Co()
    {
        while (GameManager.instance == null) yield return null;
        
        GameManager.instance.whenNodeClear -= ResetHealCamp;
        GameManager.instance.whenNodeClear += ResetHealCamp;
        ResetHealCamp();
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
            GameManager.instance.whenNodeClear -= ResetHealCamp;
    }

    public void ResetHealCamp()
    {
        StartCoroutine(ResetAndSpawnRoutine());
    }
    private IEnumerator ResetAndSpawnRoutine()
    {

        foreach (GameObject item in spawnedItems)
            {
                if (item != null) Destroy(item);
            }
            spawnedItems.Clear();
            yield return null;
            if (itemPrefab == null) yield break;

            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    GameObject newItem = Instantiate(itemPrefab, point);
                    
                    newItem.transform.localPosition = Vector3.zero;
                    newItem.transform.localRotation = Quaternion.identity;
                    
                    newItem.name = itemPrefab.name + "_Attached";
                    spawnedItems.Add(newItem);
                }
            }
    }
}