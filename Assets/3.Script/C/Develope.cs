
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Develope : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject player;
    public void Developer()
    {
        playerStats.Developer();
    }
    public void Telepoter()
    {
        player.transform.position = new Vector3(94, 3, -190);
    }
}
