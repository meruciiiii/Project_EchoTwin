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
        if (MainWeapon == null)
        {
            MainWeapon = newWeapon;
            MainWeapon.gameObject.SetActive(true);
            MainWeapon.SetDualWeaponActive(true); 
            MainWeapon.ResetResonance();           
        }
        else if (SubWeapon == null)
        {
            SubWeapon = MainWeapon;
            SubWeapon.gameObject.SetActive(false);
            SubWeapon.SetDualWeaponActive(false);
            SubWeapon.ResetResonance();          

            MainWeapon = newWeapon;
            MainWeapon.gameObject.SetActive(true);
            MainWeapon.SetDualWeaponActive(true); 
            MainWeapon.ResetResonance();         
        }
        else
        {
            SubWeapon.SetDualWeaponActive(false); 
            SubWeapon.gameObject.SetActive(false);

            SubWeapon = MainWeapon;
            SubWeapon.gameObject.SetActive(false);
            SubWeapon.SetDualWeaponActive(false); 
            SubWeapon.ResetResonance();           

            MainWeapon = newWeapon;
            MainWeapon.gameObject.SetActive(true);
            MainWeapon.SetDualWeaponActive(true);  
            MainWeapon.ResetResonance();            
        }
    }
}
