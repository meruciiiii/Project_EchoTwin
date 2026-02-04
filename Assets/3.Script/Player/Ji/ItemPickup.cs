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

    private PlayerAction player;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Color c = image.color;
            c.a = 1f;
            image.color = c;

            player = other.GetComponent<PlayerAction>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Color c = image.color;
            c.a = 0f;
            image.color = c;

            player = null;
        }
    }

    public void GetNewWeapon()
    {
        if (player == null) return;

        player.OnWeaponAcquire(weapon);
        AttachToPlayer(player.transform, attachPos);

        CleanupItemComponents();
    }

    private void CleanupItemComponents()
    {
        if(image != null)
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

    private void AttachToPlayer(Transform player,Transform attachPos)
    {
        transform.SetParent(player);
        transform.localPosition = attachPos.position;
        transform.localRotation = Quaternion.identity;
    }
}
