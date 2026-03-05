using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningGizmo : MonoBehaviour
{
    [SerializeField] private Transform quadTransform;
    [SerializeField] private MeshRenderer quadRenderer;
    [SerializeField] private float yOffset = 0.6f;

    [SerializeField] private string warningColorProperty = "_WarningColor";
    [SerializeField] private string angleProperty = "_Angle";
    [SerializeField] private string ratioProperty = "_Ratio";

    private MaterialPropertyBlock propertyBlock;
    private Coroutine coroutine;
    private Action<WarningGizmo> returnAction;

    private int warningColorID;
    private int angleID;
    private int ratioID;

    private bool isReturn = true;

    private void Awake()
    {
        if(quadRenderer == null)
        {
            quadRenderer = GetComponentInChildren<MeshRenderer>();
        }

        if (quadTransform == null)
        {
            quadTransform = quadRenderer != null ? quadRenderer.transform : transform;
        }

        propertyBlock = new MaterialPropertyBlock();

        warningColorID = Shader.PropertyToID(warningColorProperty);
        angleID = Shader.PropertyToID(angleProperty);
        ratioID = Shader.PropertyToID(ratioProperty);

        ApplyProperties(new Color(1f, 0f, 0f, 0.4f), 360f, 0f);
    }

    private void OnDisable()
    {
        if(coroutine != null)
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
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        gameObject.SetActive(true);

        transform.position = new Vector3(worldPos.x, worldPos.y + yOffset, worldPos.z);
        transform.rotation = Quaternion.identity;

        setRadius(attackRadius);
        ApplyProperties(warningColor, 360f, 0);
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
        transform.rotation = Quaternion.LookRotation(forward,Vector3.up);

        setRadius(attackRadius);
        ApplyProperties(warningColor, angle, 1f);
    }

    private void playInternal(Vector3 worldPos, Quaternion rotation, float attackRadius, float angle, float duration, Color warningColor)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        gameObject.SetActive(true);

        transform.position = new Vector3(worldPos.x, worldPos.y + yOffset, worldPos.z);
        transform.rotation = rotation;

        setRadius(attackRadius);
        ApplyProperties(warningColor, angle, 0f);

        coroutine = StartCoroutine(showWarning_Co(duration, warningColor, angle));
    }

    private IEnumerator showWarning_Co(float duration, Color warningColor, float angle)
    {
        if(duration <= 0)
        {
            ApplyProperties(warningColor, angle, 1f);
            coroutine = null;
            yield break;
        }

        float elased = 0f;

        while(elased < duration)
        {
            elased += Time.deltaTime;
            float ratio = Mathf.Clamp01(elased / duration);

            ApplyProperties(warningColor, angle, ratio);

            yield return null;
        }

        ApplyProperties(warningColor, angle, 1f);
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

        ApplyProperties(new Color(1f, 0f, 0f, 0.4f), 360f, 0f);
        isReturn = true;
        gameObject.SetActive(false);

        if(returnAction != null)
        {
            returnAction(this);
        }
    }

    public void setRatio(float ratio)
    {
        if (quadRenderer == null) return;

        quadRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(ratioID, Mathf.Clamp01(ratio));
        quadRenderer.SetPropertyBlock(propertyBlock);
    }    

    public void setAngle(float angle)
    {
        if (quadRenderer == null) return;

        quadRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(angleID, Mathf.Clamp(angle, 0f, 360f));
        quadRenderer.SetPropertyBlock(propertyBlock);
    }

    public void setWarningColor(Color warningColor)
    {
        if (quadRenderer == null) return;

        quadRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(warningColorID,warningColor);
        quadRenderer.SetPropertyBlock(propertyBlock);
    }

    private void setRadius(float attackRadius)
    {
        float diameter = attackRadius * 2f;

        Vector3 localScale = quadTransform.localScale;
        localScale.x = diameter;
        localScale.y = diameter;
        quadTransform.localScale = localScale;
    }

    private void ApplyProperties(Color warningColor, float angle, float ratio)
    {
        if (quadRenderer == null) return;

        quadRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(warningColorID, warningColor);
        propertyBlock.SetFloat(angleID, Mathf.Clamp(angle, 0f, 360f));
        propertyBlock.SetFloat(ratioID, Mathf.Clamp01(ratio));
        quadRenderer.SetPropertyBlock(propertyBlock);
    }
}
