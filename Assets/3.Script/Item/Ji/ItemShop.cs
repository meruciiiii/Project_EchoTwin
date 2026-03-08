using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum shopItem
{
    Heart,
    Cristal,
    refillEcho,
}

public class ItemShop : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float height = 5f;

    [SerializeField] private shopItem itemType;
    [SerializeField] private int price = 1;
    [SerializeField] private int value = 1;

    [SerializeField] private bool isPickedUp = false;

    private PlayerAction player;
    private Camera cam;

    private Vector3 uiPos;

    private void Awake()
    {
        cam = Camera.main;

        setImageAlpha(0f);
    }

    private void LateUpdate()
    {
        if (image == null || cam == null) return;

        Vector3 worldPos = uiPos + cam.transform.up * height;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        image.rectTransform.position = screenPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;
        if (other.CompareTag("Player"))
        {
            setImageAlpha(1f);

            player = other.GetComponent<PlayerAction>();
            player.onInteraction.RemoveListener(buyItem);
            player.onInteraction.AddListener(buyItem);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            setImageAlpha(0f);

            if (player != null)
            {
                player.onInteraction.RemoveListener(buyItem);
                player = null;
            }
        }
    }
    private void Start()
    {
        // OnEnable 외에 Start에서도 다시 한번 체크하여 인스턴스가 생성된 후 확실히 구독함
        if (GameManager.instance != null)
        {
            GameManager.instance.whenNodeClear -= ResetItem; // 중복 방지
            GameManager.instance.whenNodeClear += ResetItem;
        }
    }
    private void OnEnable()
    {
        uiPos = transform.position;

        if (GameManager.instance != null)
        {
            GameManager.instance.AddItemToList(gameObject);
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RemoveItemFromList(gameObject);
        }

        if (player != null)
        {
            player.onInteraction.RemoveListener(buyItem);
            player = null;
        }

        setImageAlpha(0f);
    }

    private void buyItem()
    {
        if (player == null) return;

        if (!player.TryBuyShopItem(itemType, price, value)) return;

        isPickedUp = true;

        player.onInteraction.RemoveListener(buyItem);
        player = null;

        setImageAlpha(0f);
        SetChildrenActive(false);
    }

    private void setImageAlpha(float alpha)
    {
        if (image == null) return;

        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
    public void ResetItem()
    {
        if (isPickedUp)
        {
                isPickedUp = false;
                setImageAlpha(1f);
                SetChildrenActive(true);
        }
    }
    private void SetChildrenActive(bool active)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }
}
