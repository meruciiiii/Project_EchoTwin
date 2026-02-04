using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment
{
    public WeaponAbstract MainWeapon { get; private set; }
    public WeaponAbstract SubWeapon { get; private set; }

    public void EquipWeapon(WeaponAbstract newWeapon)
    {
        if(MainWeapon == null)
        {
            MainWeapon = newWeapon;
            MainWeapon.gameObject.SetActive(true);
        }
        else if(SubWeapon == null)
        {
            SubWeapon = MainWeapon;
            //SubWeapon.SetResonance(SubWeapon.GetComponent<WeaponData>().echoAmount);
            SubWeapon.gameObject.SetActive(false);

            MainWeapon = newWeapon;
            MainWeapon.gameObject.SetActive(true);
        }
        else
        {
            GameObject.Destroy(SubWeapon.gameObject);
            SubWeapon = MainWeapon;
            SubWeapon.gameObject.SetActive(false);

            MainWeapon = newWeapon;
            MainWeapon.gameObject.SetActive(true);
        }
    }
}
