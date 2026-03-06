using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    [SerializeField] private List<Button> buttons;

    private void Awake()
    {
        if(stats == null) stats = FindAnyObjectByType<PlayerStats>();

        buttons.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            buttons.Add(transform.GetChild(i).GetComponent<Button>());
        }
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

    private void Start()
    {
        whenMaxHPChanged(stats.MaxHP);
        whenHPChanged(stats.CurrentHP, stats.MaxHP);
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
        Debug.Log($"whenHPChanged È£ÃâµÊ / currentHP={currentHP}, maxHP={maxHP}, stats={stats.name}");
        for (int i=0;i<buttons.Count;i++)
        {
            if (i >= maxHP) continue;
            bool isItFull = (i < currentHP);
            Debug.Log($"i : {i}, isItFull : {isItFull}");
            buttons[i].interactable = isItFull;
        }
    }
}
