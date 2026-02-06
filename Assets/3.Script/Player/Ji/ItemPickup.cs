using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private WeaponAbstract weapon;
    [SerializeField] private Transform attachPos;
    [SerializeField] private Image image;
    [SerializeField] private float height = 5f;

    [SerializeField] private bool isPickedUp = false;

    private PlayerAction player;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (image == null || cam == null) return;

        //Vector3 worldPos = transform.position + Vector3.up * height;
        Vector3 worldPos = weapon.transform.position + cam.transform.up * height;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        image.rectTransform.position = screenPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (image == null) return;
            Color c = image.color;
            c.a = 1f;
            image.color = c;

            player = other.GetComponent<PlayerAction>();
            player.onInteraction.AddListener(GetNewWeapon);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (image == null) return;
            Color c = image.color;
            c.a = 0f;
            image.color = c;

            player.onInteraction.RemoveListener(GetNewWeapon);
            player = null;
        }
    }

    public void GetNewWeapon()
    {
        if (player == null) return;
        if (isPickedUp) return;

        isPickedUp = true;

        player.OnWeaponAcquire(weapon);
        AttachToPlayer(player.transform, attachPos);

        CleanupItemComponents();
    }

    private void CleanupItemComponents()
    {
        if (image != null)
        {
            image.gameObject.SetActive(false);
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        this.enabled = false;
    }

    private void AttachToPlayer(Transform player, Transform attachPos)
    {
        transform.SetParent(player);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.position = attachPos.position;
        transform.rotation = attachPos.rotation;
    }
}
