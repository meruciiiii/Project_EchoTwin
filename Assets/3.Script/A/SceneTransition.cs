using UnityEngine;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("Cloud RectTransforms")]
    public RectTransform rightCloud;
    public RectTransform leftCloud;

    [Header("Settings")]
    public float duration = 1.0f;

    // 오른쪽 구름 좌표
    private const float RightClosedX = 725f;
    private const float RightOpenX = 2630f;

    // 왼쪽 구름 좌표
    private const float LeftClosedX = -783f;
    private const float LeftOpenX = -2590f;

    private Coroutine transitionCoroutine;

    private void Start()
    {
        if (rightCloud == null || leftCloud == null)
        {
            AssignClouds();
        }

        Close();
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

    private IEnumerator Transition(bool isClosing)
    {
        if (rightCloud == null || leftCloud == null) yield break;

        float elapsed = 0f;

        // 시작 위치 저장
        Vector2 rStart = rightCloud.anchoredPosition;
        Vector2 lStart = leftCloud.anchoredPosition;

        // 목표 위치 결정
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
        transitionCoroutine = null;
    }
}