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
    public static List<GetCurrency> activeHeart = new List<GetCurrency>();

    [SerializeField] private Item item;
    private Collider col;
    private PlayerStats player;

    public int amount = 0;
    private int getheringTime = 5;
    private float duration = 1f;

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
        if(item == Item.heart) activeHeart.Add(this);
        StartCoroutine(getAllCurrency_Co());
    }

    private void OnDisable()
    {
        if (item == Item.heart) activeHeart.Remove(this);
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

            while (timer<duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                transform.position = Vector3.Lerp(startPos, player.transform.position, t);

                yield return null;
            }
        }
    }

    public static void destroyAllHeart()
    {
        for(int i=activeHeart.Count -1;i>=0;i--)
        {
            if(activeHeart[i] != null)
            {
                Destroy(activeHeart[i].gameObject);
            }
        }
        activeHeart.Clear();
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
                if (GameManager.instance != null)
                {
                    GameManager.instance.RemoveItemFromList(gameObject);
                }
                Destroy(gameObject);
            }
        }
    }
}
