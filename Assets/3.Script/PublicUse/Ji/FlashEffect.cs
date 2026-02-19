using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    private Coroutine flashCoroutine;

    private Color originalColor;
    private Renderer targetRenderer;

    private void Awake()
    {
        TryGetComponent(out targetRenderer);
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        if (targetRenderer != null)
        {
            originalColor = targetRenderer.material.color;
        }
    }

    public void Flash(int count, float duration)
    {
        if (targetRenderer == null) return;

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(Blink_Co(count, duration));
    }

    public void ChargeEffect(float duration)
    {
        if (targetRenderer == null) return;

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FadeIn_Co(duration));
    }

    private IEnumerator Blink_Co(int count, float duration)
    {
        float halfDuration = duration / 2f;
        WaitForSeconds wfs = new WaitForSeconds(halfDuration);

        for (int i = 0; i < count; i++)
        {
            targetRenderer.material.color = Color.white;

            yield return wfs;

            targetRenderer.material.color = originalColor;

            yield return wfs;
        }

        flashCoroutine = null;
    }

    private IEnumerator FadeIn_Co(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            targetRenderer.material.color = Color.Lerp(originalColor, Color.white, timer / duration);
            yield return null;
        }
        targetRenderer.material.color = originalColor;
    }

    public void RestColor()
    {
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        targetRenderer.material.color = originalColor;
        flashCoroutine = null;
    }
}
