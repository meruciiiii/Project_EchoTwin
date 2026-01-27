using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractUI : MonoBehaviour
{
    [SerializeField] private Image image;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Color c = image.color;
            c.a = 1f;
            image.color = c;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Color c = image.color;
            c.a = 0f;
            image.color = c;
        }
    }
}
