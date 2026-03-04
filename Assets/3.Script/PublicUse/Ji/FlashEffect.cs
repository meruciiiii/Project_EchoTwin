using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    private Coroutine flashCoroutine;
    private Coroutine chargeCoroutine;

    private Renderer targetRenderer;
    private MaterialPropertyBlock mpb;

    private static readonly int emissionColorID = Shader.PropertyToID("_EmissionColor");

    private Color originalColor;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        
        if(targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        mpb = new MaterialPropertyBlock();

        if(targetRenderer.sharedMaterial != null && targetRenderer.sharedMaterial.HasProperty(emissionColorID))
        {
            originalColor = targetRenderer.sharedMaterial.GetColor(emissionColorID);
        }
        else
        {
            originalColor = Color.black;
        }

        targetRenderer.GetPropertyBlock(mpb);
        Color blockColor = mpb.GetColor(emissionColorID);
        if(blockColor.r != 0f || blockColor.g != 0f || blockColor.b != 0f || blockColor.a != 0f)
        {
            originalColor = blockColor;
        }

        mpb.Clear();

        targetRenderer.SetPropertyBlock(mpb);
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
            
        if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);

        chargeCoroutine = StartCoroutine(FadeIn_Co(duration));
    }

    private void SetColor(Color color)
    {
        if (targetRenderer == null) return;

        targetRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(emissionColorID, color);
        targetRenderer.SetPropertyBlock(mpb);
    }

    private IEnumerator Blink_Co(int count, float duration)
    {
        float flashDuration = duration / (count * 2f);
        WaitForSeconds wfs = new WaitForSeconds(flashDuration);

        for (int i = 0; i < count; i++)
        {
            SetColor(Color.white);
            yield return wfs;

            SetColor(originalColor);
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

            SetColor(Color.Lerp(originalColor, Color.white, timer / duration));
            yield return null;
        }

        SetColor(originalColor);
        chargeCoroutine = null;
    }
}
