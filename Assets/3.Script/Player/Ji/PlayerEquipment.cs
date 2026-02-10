using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerEquipment
{
    //public WeaponAbstract MainWeapon { get; private set; }
    //public WeaponAbstract SubWeapon { get; private set; }
    public WeaponAbstract MainWeapon;
    public WeaponAbstract SubWeapon;

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
            if(SubWeapon.DualWeapon != null)
            {
                GameObject.Destroy(SubWeapon.DualWeapon);
            }
            GameObject.Destroy(SubWeapon.gameObject);
            
            SubWeapon = MainWeapon;
            SubWeapon.gameObject.SetActive(false);

            MainWeapon = newWeapon;
            MainWeapon.gameObject.SetActive(true);
        }
    }
}
