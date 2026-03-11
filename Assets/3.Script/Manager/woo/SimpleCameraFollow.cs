
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float normalFov = 60f;   // 평상시 시야
    [SerializeField] private float bossSkillFov = 80f; // 스킬 사용 시 (Zoom Out - 더 넓게)
    [SerializeField] private float zoomDuration = 0.2f;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private Coroutine zoomCoroutine;
    private void Start()
    {
        if (_camera == null)
            _camera = Camera.main;
        transform.rotation = Quaternion.Euler(45f, 0f, 0f);
        //GameManager.instance.bossSkillStart += StartBossSkill;
        //GameManager.instance.bossSkillEnd += EndBossSkill;
    }
    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;
    }
    private void StartBossSkill()
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ChangeFovWithCurve(_camera.fieldOfView, bossSkillFov));
    }
    private void EndBossSkill()
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(ChangeFovWithCurve(_camera.fieldOfView, normalFov));
    }
    private IEnumerator ChangeFovWithCurve(float fromFov, float toFov)
    {
        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / zoomDuration;
            float curveValue = zoomCurve.Evaluate(normalizedTime);
            _camera.fieldOfView = Mathf.Lerp(fromFov, toFov, curveValue);
            yield return null;
        }
        _camera.fieldOfView = toFov;
        zoomCoroutine = null;
    }
}
