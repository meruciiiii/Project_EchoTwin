using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    [SerializeField] private List<Button> buttons;

    private void Awake()
    {
        if(stats == null) stats = FindAnyObjectByType<PlayerStats>();
    }

    private void OnEnable()
    {
        if (stats == null) return;

        stats.onMaxHpChanged += whenMaxHPChanged;
        stats.onHpChanged += whenHPChanged;
    }

    private void OnDisable()
    {
        if (stats == null) return;

        stats.onMaxHpChanged -= whenMaxHPChanged;
        stats.onHpChanged -= whenHPChanged;
    }

    private void whenMaxHPChanged(int maxHP)
    {
        for(int i=0;i<buttons.Count;i++)
        {
            bool isItActive = (i < maxHP);
            buttons[i].gameObject.SetActive(isItActive);
        }
    }

    private void whenHPChanged(int currentHP, int maxHP)
    {
        for(int i=0;i<buttons.Count;i++)
        {
            if (i > maxHP) continue;
            bool isItFull = (i < currentHP);
            buttons[i].interactable = isItFull;
        }
    }
}
