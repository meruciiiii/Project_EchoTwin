using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SacrificeCamp : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float height = 5f;

    private PlayerAction player;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;

        setImageAlpha(0f);
    }

    private void LateUpdate()
    {
        if (image == null || cam == null) return;

        //Vector3 worldPos = transform.position + Vector3.up * height;
        Vector3 worldPos = this.transform.position + cam.transform.up * height;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        image.rectTransform.position = screenPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            setImageAlpha(1f);

            player = other.GetComponent<PlayerAction>();
            player.onInteraction.AddListener(sacrifice);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            setImageAlpha(0f);

            player.onInteraction.RemoveListener(sacrifice);
            player = null;
        }
    }

    private void sacrifice()
    {
        if (player == null) return;

        player.takeDamage(1,transform.position,0);

        ItemDataBase db = ItemDataBase.Instance;
        Vector3 floorPos = transform.position;

        if (db == null)
        {
            Debug.Log("db null");
            return;
        }
        if (db.cristalPrefab != null)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), 0.8f, Random.Range(-2f, 2f));
                Vector3 spawnPos = floorPos + randomOffset;

                GameObject cristalObj = Instantiate(db.cristalPrefab, spawnPos, Quaternion.identity, GameManager.instance.transform);
                GetCurrency cristal = cristalObj.GetComponent<GetCurrency>();
                cristal.amount = 3;
                cristal.isOnGround = true;

                if (GameManager.instance != null)
                {
                    GameManager.instance.AddItemToList(cristalObj);
                }
            }
        }
    }

    private void setImageAlpha(float alpha)
    {
        if (image == null) return;

        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
}
