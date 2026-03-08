using UnityEngine;
using System;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("Cloud RectTransforms")]
    public RectTransform rightCloud;
    public RectTransform leftCloud;

    [Header("Settings")]
    public float duration = 1.0f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // ������ ���� ��ǥ
    private const float RightClosedX = 725f;
    private const float RightOpenX = 2630f;

    // ���� ���� ��ǥ
    private const float LeftClosedX = -783f;
    private const float LeftOpenX = -2590f;

    private Coroutine transitionCoroutine;

    private void Start()
    {
        if (rightCloud == null || leftCloud == null)
        {
            AssignClouds();
        }

        Open();
        SoundManager.SendEvent(SoundType.SFX_UI_Cloud);

    }

    private void AssignClouds()
    {
        if (transform.childCount >= 2)
        {
            transform.GetChild(0).TryGetComponent(out rightCloud);
            transform.GetChild(1).TryGetComponent(out leftCloud);
        }
    }

    public void Close()
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(Transition(true));
    }

    public void Open()
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(Transition(false));
    }



    public void PlayFullTransition(Action mapChangeAction)
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(FullTransition_Co(mapChangeAction));
    }

    private IEnumerator FullTransition_Co(Action mapChangeAction)
    {
        yield return StartCoroutine(Transition(true));

        mapChangeAction?.Invoke();

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(Transition(false));
    }

    private IEnumerator Transition(bool isClosing)
    {
        if (rightCloud == null || leftCloud == null) yield break;

        float elapsed = 0f;
        Vector2 rStart = rightCloud.anchoredPosition;
        Vector2 lStart = leftCloud.anchoredPosition;

        float rTargetX = isClosing ? RightClosedX : RightOpenX;
        float lTargetX = isClosing ? LeftClosedX : LeftOpenX;

        Vector2 rTarget = new Vector2(rTargetX, rStart.y);
        Vector2 lTarget = new Vector2(lTargetX, lStart.y);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            rightCloud.anchoredPosition = Vector2.Lerp(rStart, rTarget, t);
            leftCloud.anchoredPosition = Vector2.Lerp(lStart, lTarget, t);

            yield return null;
        }

        rightCloud.anchoredPosition = rTarget;
        leftCloud.anchoredPosition = lTarget;
    }
}