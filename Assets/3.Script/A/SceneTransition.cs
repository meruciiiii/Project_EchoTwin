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

            // 진행도를 0~1 사이 값으로 정규화
            float normalizedTime = elapsed / duration;

            // AnimationCurve에서 해당 시점의 부드러운 t값을 가져옵니다.
            float t = transitionCurve.Evaluate(normalizedTime);

            // Lerp에 커브가 적용된 t값을 넣어서 이동합니다.
            rightCloud.anchoredPosition = Vector2.Lerp(rStart, rTarget, t);
            leftCloud.anchoredPosition = Vector2.Lerp(lStart, lTarget, t);

            yield return null;
        }

        // 마지막 위치 확정
        rightCloud.anchoredPosition = rTarget;
        leftCloud.anchoredPosition = lTarget;
        transitionCoroutine = null;
    }

    public void PlayFullTransition(Action mapChangeAction)
    {
        StartCoroutine(TransitionSequence(mapChangeAction));
    }

    private IEnumerator TransitionSequence(Action mapChangeAction)
    {
        // 1. 구름 닫기
        Close();
        // 구름이 닫히는 시간(duration) 동안 대기
        yield return new WaitForSeconds(duration);

        // 2. 구름이 다 닫히면 전달받은 맵 변경 로직 실행
        mapChangeAction?.Invoke();

        // 3. 민섭 님이 요청하신 0.2초 대기
        yield return new WaitForSeconds(0.2f);

        // 4. 구름 열기
        Open();
    }

}