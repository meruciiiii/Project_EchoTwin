using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeData
{
    [Header("ID")]
    public int id;
    public string name;
    public string targetStat;

    [Header("Amount")]
    public int maxLv;
    public int baseCost;
    public float costIncrease;
    public float valuePerLv;

    [Header("Info")]
    public string description;
}
