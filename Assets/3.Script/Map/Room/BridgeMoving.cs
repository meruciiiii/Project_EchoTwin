
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeMoving : MonoBehaviour
{
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float moveDuration = 1.5f;

    private bool isDropping = false;
    private Vector3 startPos;
    private Vector3 targetPos;

    public void StartMoving(GameObject door, bool doorState)
    {
        if (doorState)
        {
            if (!isDropping) return;
            startPos = door.transform.position;
            targetPos = startPos + Vector3.up * moveDistance;
            StartCoroutine(MoveBridge());
        }
        else
        {
            if (isDropping) return;
            startPos = door.transform.position;
            targetPos = startPos + Vector3.down * moveDistance;
            StartCoroutine(MoveBridge());
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (isDropping) return;

    //    if (other.CompareTag("Player"))
    //    {
    //        StartCoroutine(DropBridge());
    //    }
    //}
    private IEnumerator MoveBridge()
    {
        isDropping = !isDropping;
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
}
