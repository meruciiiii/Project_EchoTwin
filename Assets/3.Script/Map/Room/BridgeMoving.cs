
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeMoving : MonoBehaviour
{
    [SerializeField] private float moveDistance = 12f;
    [SerializeField] private float moveDuration = 1.5f;

    private bool isUp = true;
    private Vector3 startPos;
    private Vector3 targetPos;
    public void EnterDoor(bool doorState)
    {
        if (doorState == isUp) return;
        Vector3 offset = (isUp ? Vector3.down : Vector3.up) * moveDistance;
        transform.position += offset;
        isUp = doorState;
    }
    public void SetState(bool doorState)
    {
        if (!doorState) return;
        //Debug.Log(this.name +" is bridge move");
        StartCoroutine(MoveBridge());
    }
    private IEnumerator MoveBridge()
    {
        isUp = !isUp;
        startPos = transform.position;
        targetPos = startPos + (isUp ? Vector3.up : Vector3.down) * moveDistance;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / moveDuration);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        transform.position = targetPos;
    }
    public void ResetBridge()
    {
        if (isUp) return;
        Vector3 offset = Vector3.up * moveDistance;
        transform.position += offset;
        isUp = true;
    }
}
