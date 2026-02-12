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
            if (MainWeapon.weaponID == WeaponID.Dagger) MainWeapon.DualWeapon.SetActive(true);
        }
        else if(SubWeapon == null)
        {
            SubWeapon = MainWeapon;
            //SubWeapon.SetResonance(SubWeapon.GetComponent<WeaponData>().echoAmount);
            SubWeapon.gameObject.SetActive(false);
            if (SubWeapon.weaponID == WeaponID.Dagger) SubWeapon.DualWeapon.SetActive(false);

            MainWeapon = newWeapon;
            MainWeapon.gameObject.SetActive(true);
        }
        else
        {
            SubWeapon.gameObject.SetActive(false);
            if (SubWeapon.weaponID == WeaponID.Dagger) SubWeapon.DualWeapon.SetActive(false);
            
            SubWeapon = MainWeapon;
            SubWeapon.gameObject.SetActive(false);

            MainWeapon = newWeapon;
            MainWeapon.gameObject.SetActive(true);
        }
    }
}
