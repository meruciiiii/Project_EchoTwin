using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item
{
    gold,
    cristal,
    heart,
}
public class GetCurrency : MonoBehaviour
{
    [SerializeField] private Item item;
    private Collider col;
    private PlayerStats player;

    public int amount = 0;
    private int getheringTime = 5;
    private int duration = 1;

    private int groundLayer = 8;
    private int structure = 9;
    [SerializeField] private bool isOnGround = false;

    private ItemFloating floating;
    private Rigidbody rb;

    private void Awake()
    {
        TryGetComponent(out col);
        col.isTrigger = true;
        player = FindAnyObjectByType<PlayerStats>();
        TryGetComponent(out floating);
        floating.enabled = false;
        TryGetComponent(out rb);
    }

    private void OnEnable()
    {
        StartCoroutine(getAllCurrency_Co());
    }

    private IEnumerator getAllCurrency_Co()
    {
        yield return new WaitForSeconds(getheringTime);
        isOnGround = true;
        if (item != Item.heart)
        {
            Vector3 startPos = transform.position;
            float timer = 0;
            floating.enabled = false;

            while (true)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                transform.position = Vector3.Lerp(startPos, player.transform.position, t);

                yield return null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == groundLayer)
        {
            rb.isKinematic = true;
            floating.enabled = true;
            isOnGround = true;
        }

        if (other.CompareTag("Player"))
        {
            if (!isOnGround) return;
            if (item == Item.gold)
            {
                player.getGold(amount);
                Destroy(gameObject);
            }
            if (item == Item.cristal)
            {
                player.getCristal(amount);
                Destroy(gameObject);
            }
            if (player.CurrentHP == player.MaxHP) return;
            if (item == Item.heart)
            {
                player.getHeart();
                Destroy(gameObject);
            }
        }
    }
}
