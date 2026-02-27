using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldUI: MonoBehaviour
{
    private PlayerStats stats;

    [SerializeField] private TMP_Text goldText;

    private int beforeGold = 0;
    private int afterGold = 0;


    [SerializeField] private float duration = 1f;

    private Coroutine coroutine;

    private void Awake()
    {
        stats = FindAnyObjectByType<PlayerStats>();
        goldText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        stats.onCoinChanged += whenGetGold;
    }

    private void OnDisable()
    {
        stats.onCoinChanged -= whenGetGold;
    }

    private void whenGetGold(int after)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            if (goldText != null) int.TryParse(goldText.text.Replace(",", ""), out beforeGold);
        }
        afterGold = after;
        coroutine = StartCoroutine(goldAnimate_Co(beforeGold, afterGold));
    }

    private IEnumerator goldAnimate_Co(int start, int last)
    {
        float elapsed = 0f;

        if (goldText != null) goldText.text = $"{start:N0}";

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // EaseOut

            int currentDisplay = (int)Mathf.Lerp(start, last, t);

            if (goldText != null)
                goldText.text = $"{currentDisplay:N0}";

            yield return null;
        }

        if (goldText != null) goldText.text = $"{last:N0}";
        beforeGold = last;
        coroutine = null;
    }
}
