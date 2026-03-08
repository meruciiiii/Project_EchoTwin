using System.Collections.Generic;
using UnityEngine;
public class StartRoom : MonoBehaviour
{
    [SerializeField] private Basket[] items = new Basket[2];
    private void OnEnable()
    {
        GameManager.instance.whenNodeClear += OnItemPicked;
        foreach (Basket basket in items)
        {
            basket.OnAnyItemPicked += OnItemPicked;
        }
    }
    public void SetStartRoom()
    {
        HashSet<WeaponID> usedWeapons = new HashSet<WeaponID>();
        foreach (Basket basket in items)
        {
            if (basket == null) continue;
            int safety = 100; // 무한루프 방지
            while (safety-- > 0)
            {
                basket.ActivateRandomItem();
                WeaponID id = basket.GetSelectedWeaponID();
                if (!usedWeapons.Contains(id))
                {
                    usedWeapons.Add(id);
                    break;
                }
            }
        }
    }
    private void OnItemPicked()
    {
        foreach (Basket basket in items)
        {
            if (basket == null) continue;
            basket.DisableBasket();
        }
    }
    private void OnDisable()
    {
        GameManager.instance.whenNodeClear -= OnItemPicked;
        foreach (Basket basket in items)
        {
            basket.OnAnyItemPicked -= OnItemPicked;
        }
    }
}