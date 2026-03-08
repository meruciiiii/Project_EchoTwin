using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CristalUI : MonoBehaviour
{
    private PlayerStats stats;

    [SerializeField] private TMP_Text cristalText;

    private int beforeCristal = 0;
    private int afterCristal = 0;

    [SerializeField] private float duration = 1f;

    private Coroutine coroutine;

    private void Awake()
    {
        stats = FindAnyObjectByType<PlayerStats>();
        cristalText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        if (stats == null) return;

        stats.onCristalChanged += whenGetCristal;
        whenGetCristal(stats.Cristal);
    }

    private void OnDisable()
    {
        if (stats == null) return;

        stats.onCristalChanged -= whenGetCristal;
    }

    private void whenGetCristal(int after)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            if (cristalText != null) int.TryParse(cristalText.text.Replace(",", ""), out beforeCristal);
        }

        afterCristal = after; // [CHANGED]
        coroutine = StartCoroutine(cristalAnimate_Co(beforeCristal, afterCristal));
    }

    private IEnumerator cristalAnimate_Co(int start, int last)
    {
        float elapsed = 0f;

        if (cristalText != null) cristalText.text = $"{start:N0}";

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            int currentDisplay = (int)Mathf.Lerp(start, last, t);

            if (cristalText != null)
                cristalText.text = $"{currentDisplay:N0}";

            yield return null;
        }

        if (cristalText != null) cristalText.text = $"{last:N0}";
        beforeCristal = last;
        coroutine = null;
    }
}

