using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningGizmo : MonoBehaviour
{
    [SerializeField] private Transform baseQuadTransform;
    [SerializeField] private MeshRenderer baseQuadRenderer;

    [SerializeField] private Transform fillQuadTransform;
    [SerializeField] private MeshRenderer fillQuadRenderer;

    [SerializeField] private float yOffset = 0.6f;

    [SerializeField] private string warningColorProperty = "_WarningColor";
    [SerializeField] private string angleProperty = "_Angle";
    [SerializeField] private string ratioProperty = "_Ratio";

    [SerializeField] private Color baseColor = new Color(1f, 0f, 0f, 0.15f);
    [SerializeField] private Color fillColor = new Color(1f, 0f, 0f, 0.4f);

    private MaterialPropertyBlock basePropertyBlock;
    private MaterialPropertyBlock fillPropertyBlock;

    private Coroutine coroutine;
    private Action<WarningGizmo> returnAction;

    private int warningColorID;
    private int angleID;
    private int ratioID;

    private bool isReturn = true;

    private void Awake()
    {
        if (baseQuadTransform == null) baseQuadTransform = transform.GetChild(0);
        if (fillQuadTransform == null) fillQuadTransform = transform.GetChild(1);

        if (baseQuadRenderer == null && baseQuadTransform != null)
        {
            baseQuadRenderer = baseQuadTransform.GetComponent<MeshRenderer>();
        }
        if (fillQuadRenderer == null && fillQuadTransform != null)
        {
            fillQuadRenderer = fillQuadTransform.GetComponent<MeshRenderer>();
        }

        basePropertyBlock = new MaterialPropertyBlock();
        fillPropertyBlock = new MaterialPropertyBlock();

        warningColorID = Shader.PropertyToID(warningColorProperty);
        angleID = Shader.PropertyToID(angleProperty);
        ratioID = Shader.PropertyToID(ratioProperty);

        ApplyBaseProperties(baseColor, 360f, 1f);
        ApplyFillProperties(fillColor, 360f, 0f);
    }

    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public void init(Action<WarningGizmo> returnAction)
    {
        this.returnAction = returnAction;
        isReturn = false;
    }

    public void playCircle(Vector3 worldPos, float attackRadius, float duration, Color warningColor)
    {
        playInternal(worldPos, Quaternion.identity, attackRadius, 360f, duration, warningColor);
    }

    public void showCircle(Vector3 worldPos, float attackRadius, Color warningColor)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        gameObject.SetActive(true);

        transform.position = new Vector3(worldPos.x, worldPos.y + yOffset, worldPos.z);
        transform.rotation = Quaternion.identity;

        setRadius(attackRadius);

        ApplyBaseProperties(setAlpha(warningColor, warningColor.a * 0.4f), 360f, 1f);
        ApplyFillProperties(warningColor, 360f, 1f);
    }

    public void playSector(Vector3 worldPos, Vector3 forward, float attackRadius, float angle, float duration, Color warningColor)
    {
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;

        Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
        playInternal(worldPos, rotation, attackRadius, angle, duration, warningColor);
    }

    public void showSector(Vector3 worldPos, Vector3 forward, float attackRadius, float angle, Color warningColor)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;

        gameObject.SetActive(true);

        transform.position = new Vector3(worldPos.x, worldPos.y + yOffset, worldPos.z);
        transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        setRadius(attackRadius);

        ApplyBaseProperties(setAlpha(warningColor, warningColor.a * 0.4f), angle, 1f);
        ApplyFillProperties(warningColor, angle, 1f);
    }

    private void playInternal(Vector3 worldPos, Quaternion rotation, float attackRadius, float angle, float duration, Color warningColor)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        gameObject.SetActive(true);

        transform.position = new Vector3(worldPos.x, worldPos.y + yOffset, worldPos.z);
        transform.rotation = rotation;

        setRadius(attackRadius);

        ApplyBaseProperties(setAlpha(warningColor, warningColor.a * 0.4f), angle, 1f);
        ApplyFillProperties(warningColor, angle, 0f);

        coroutine = StartCoroutine(showWarning_Co(duration, warningColor, angle));
    }

    private IEnumerator showWarning_Co(float duration, Color warningColor, float angle)
    {
        if (duration <= 0)
        {
            ApplyBaseProperties(setAlpha(warningColor, warningColor.a * 0.4f), angle, 1f);
            ApplyFillProperties(warningColor, angle, 1f);
            coroutine = null;
            yield break;
        }

        float elased = 0f;

        while (elased < duration)
        {
            elased += Time.deltaTime;
            float ratio = Mathf.Clamp01(elased / duration);

            ApplyBaseProperties(setAlpha(warningColor, warningColor.a * 0.4f), angle, 1f);
            ApplyFillProperties(warningColor, angle, ratio);

            yield return null;
        }

        ApplyBaseProperties(setAlpha(warningColor, warningColor.a * 0.4f), angle, 1f);
        ApplyFillProperties(warningColor, angle, 1f);
        coroutine = null;
    }

    public void Hide()
    {
        if (isReturn) return;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        ApplyBaseProperties(baseColor, 360f, 1f);
        ApplyFillProperties(fillColor, 360f, 0f);

        isReturn = true;
        gameObject.SetActive(false);

        if (returnAction != null)
        {
            returnAction(this);
        }
    }

    public void setRatio(float ratio)
    {
        if (fillQuadRenderer == null) return;

        fillQuadRenderer.GetPropertyBlock(fillPropertyBlock);
        fillPropertyBlock.SetFloat(ratioID, Mathf.Clamp01(ratio));
        fillQuadRenderer.SetPropertyBlock(fillPropertyBlock);
    }

    public void setAngle(float angle)
    {
        float clampedAngle = Mathf.Clamp(angle, 0f, 360f);

        if (baseQuadRenderer != null)
        {
            baseQuadRenderer.GetPropertyBlock(basePropertyBlock);
            basePropertyBlock.SetFloat(angleID, clampedAngle);
            baseQuadRenderer.SetPropertyBlock(basePropertyBlock);
        }

        if (fillQuadRenderer != null)
        {
            fillQuadRenderer.GetPropertyBlock(fillPropertyBlock);
            fillPropertyBlock.SetFloat(angleID, clampedAngle);
            fillQuadRenderer.SetPropertyBlock(fillPropertyBlock);
        }
    }

    public void setWarningColor(Color warningColor)
    {
        ApplyBaseProperties(setAlpha(warningColor, warningColor.a * 0.4f), 360f, 1f);
        ApplyFillProperties(warningColor, 360f, 1f);
    }

    private void setRadius(float attackRadius)
    {
        float diameter = attackRadius * 2f;

        if (baseQuadTransform != null)
        {
            Vector3 localScale = baseQuadTransform.localScale;
            localScale.x = diameter;
            localScale.y = diameter;
            baseQuadTransform.localScale = localScale;
        }

        if (fillQuadTransform != null)
        {
            Vector3 localScale = fillQuadTransform.localScale;
            localScale.x = diameter;
            localScale.y = diameter;
            fillQuadTransform.localScale = localScale;
        }
    }

    private void ApplyBaseProperties(Color warningColor, float angle, float ratio)
    {
        if (baseQuadRenderer == null) return;

        baseQuadRenderer.GetPropertyBlock(basePropertyBlock);
        basePropertyBlock.SetColor(warningColorID, warningColor);
        basePropertyBlock.SetFloat(angleID, Mathf.Clamp(angle, 0f, 360f));
        basePropertyBlock.SetFloat(ratioID, Mathf.Clamp01(ratio));
        baseQuadRenderer.SetPropertyBlock(basePropertyBlock);
    }

    private void ApplyFillProperties(Color warningColor, float angle, float ratio)
    {
        if (fillQuadRenderer == null) return;

        fillQuadRenderer.GetPropertyBlock(fillPropertyBlock);
        fillPropertyBlock.SetColor(warningColorID, warningColor);
        fillPropertyBlock.SetFloat(angleID, Mathf.Clamp(angle, 0f, 360f));
        fillPropertyBlock.SetFloat(ratioID, Mathf.Clamp01(ratio));
        fillQuadRenderer.SetPropertyBlock(fillPropertyBlock);
    }

    private Color setAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}
