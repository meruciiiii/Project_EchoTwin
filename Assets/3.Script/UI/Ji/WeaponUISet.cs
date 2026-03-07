using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUISet : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Image subBGColor;

    [Header("Main Weapon UI")]
    [SerializeField] private GameObject mainSword;
    [SerializeField] private GameObject mainHammer;
    [SerializeField] private GameObject mainDagger;
    [SerializeField] private GameObject mainSpear;
    [SerializeField] private GameObject mainAxe;

    [Header("Sub Weapon UI")]
    [SerializeField] private GameObject subSword;
    [SerializeField] private GameObject subHammer;
    [SerializeField] private GameObject subDagger;
    [SerializeField] private GameObject subSpear;
    [SerializeField] private GameObject subAxe;

    private Color originColor;

    private void Awake()
    {
        if (subBGColor != null)
        {
            originColor = subBGColor.color;
        }
    }

    private void Start()
    {
        if (GameManager.instance == null) return;

        GameManager.instance.turnWeaponUI += setWeaponUI;
        GameManager.instance.setResonanceUI += setSlider;
        setWeaponUI(GameManager.instance.mainWeapon, GameManager.instance.subWeapon);
    }

    private void OnDestroy()
    {
        if (GameManager.instance == null) return;

        GameManager.instance.turnWeaponUI -= setWeaponUI;
        GameManager.instance.setResonanceUI -= setSlider;
    }

    private void setWeaponUI(WeaponAbstract mainWeapon, WeaponAbstract subWeapon)
    {
        setMainWeapon(mainWeapon);
        setSubWeapon(subWeapon);
        setSlider();
        setBGColor();
    }

    private void setMainWeapon(WeaponAbstract weapon)
    {
        if (mainSword != null) mainSword.SetActive(false);
        if (mainHammer != null) mainHammer.SetActive(false);
        if (mainDagger != null) mainDagger.SetActive(false);
        if (mainSpear != null) mainSpear.SetActive(false);
        if (mainAxe != null) mainAxe.SetActive(false);

        if (weapon == null) return;

        switch (weapon.weaponID)
        {
            case WeaponID.Sword:
                if (mainSword != null) mainSword.SetActive(true);
                break;
            case WeaponID.Hammer:
                if (mainHammer != null) mainHammer.SetActive(true);
                break;
            case WeaponID.Dagger:
                if (mainDagger != null) mainDagger.SetActive(true);
                break;
            case WeaponID.Spear:
                if (mainSpear != null) mainSpear.SetActive(true);
                break;
            case WeaponID.Axe:
                if (mainAxe != null) mainAxe.SetActive(true);
                break;
        }
    }

    private void setSubWeapon(WeaponAbstract weapon)
    {
        if (subSword != null) subSword.SetActive(false);
        if (subHammer != null) subHammer.SetActive(false);
        if (subDagger != null) subDagger.SetActive(false);
        if (subSpear != null) subSpear.SetActive(false);
        if (subAxe != null) subAxe.SetActive(false);

        if (weapon == null) return;

        switch (weapon.weaponID)
        {
            case WeaponID.Sword:
                if (subSword != null) subSword.SetActive(true);
                break;
            case WeaponID.Hammer:
                if (subHammer != null) subHammer.SetActive(true);
                break;
            case WeaponID.Dagger:
                if (subDagger != null) subDagger.SetActive(true);
                break;
            case WeaponID.Spear:
                if (subSpear != null) subSpear.SetActive(true);
                break;
            case WeaponID.Axe:
                if (subAxe != null) subAxe.SetActive(true);
                break;
        }
    }

    private void setSlider()
    {
        if (mainSlider == null) return;
        if (GameManager.instance == null) return;

        if (GameManager.instance.subWeapon == null)
        {
            mainSlider.maxValue = 1f;
            mainSlider.value = 0f;
            return;
        }

        mainSlider.maxValue = GameManager.instance.subWeapon.weaponData.resonanceCount;
        mainSlider.value = GameManager.instance.subWeapon.resonanceCount;
    }

    private void setBGColor()
    {
        if (subBGColor == null) return;
        if (GameManager.instance == null) return;

        if(GameManager.instance.subWeapon == null)
        {
            subBGColor.color = Color.gray;
        }
        else
        {
            subBGColor.color = originColor;
        }
    }
}
