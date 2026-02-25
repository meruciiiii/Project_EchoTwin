using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    //[SerializeField] private WeaponAbstract weapon;
    [SerializeField] private WeaponID weaponID;
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
        Vector3 worldPos = this.transform.position + cam.transform.up * height;
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

        player.OnWeaponAcquire(weaponID);

        gameObject.SetActive(false);

        //AttachToPlayer(weapon, player);
        //CleanupItemComponents();
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

    //private void AttachToPlayer(WeaponAbstract weapon, PlayerAction player)
    //{
    //    Transform rightHand = player.RightHand;
    //    Transform leftHand = player.LeftHand;

    //    if (weapon.weaponType == WeaponType.onehand || weapon.weaponType == WeaponType.twohand)
    //    {
    //        weapon.transform.SetParent(rightHand);
    //        weapon.transform.localPosition = Vector3.zero;
    //        weapon.transform.localRotation = Quaternion.identity;

    //        weapon.GetComponent<Collider>().enabled = false;
    //    }
    //    else if (weapon.weaponType == WeaponType.dual)
    //    {
    //        weapon.transform.SetParent(rightHand);
    //        weapon.transform.localPosition = Vector3.zero;
    //        weapon.transform.localRotation = Quaternion.identity;

    //        if (weapon.DualWeapon == null)
    //        {
    //            weapon.DualWeapon = Instantiate(weapon.gameObject, leftHand);
    //            weapon.DualWeapon.transform.localPosition = Vector3.zero;
    //            weapon.DualWeapon.transform.localRotation = Quaternion.identity;

    //            if (weapon.DualWeapon.TryGetComponent<ItemPickup>(out ItemPickup item))
    //            {
    //                Destroy(item);
    //            }
    //            if (weapon.DualWeapon.TryGetComponent<Collider>(out Collider col))
    //            {
    //                col.enabled = false;
    //            }
    //            if (weapon.DualWeapon.TryGetComponent<WeaponAbstract>(out WeaponAbstract WA))
    //            {
    //                WA.enabled = false;
    //            }
    //        }
    //    }
    //}
}
