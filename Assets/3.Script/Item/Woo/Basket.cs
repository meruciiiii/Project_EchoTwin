using System;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private ItemPickup[] items = new ItemPickup[5];
    private int index;
    public Action OnAnyItemPicked;
    public void ActivateRandomItem()
    {
        if (items == null || items.Length == 0) return;
        index = UnityEngine.Random.Range(0, items.Length);
        ItemPickup selectedItem = items[index];
        if (selectedItem == null) return;
        // 선택된 아이템 활성화
        selectedItem.gameObject.SetActive(true);
        selectedItem.OnAnyItemPicked += OnItemPicked;
    }
    public WeaponID GetSelectedWeaponID()
    {
        ItemPickup activeItem = GetActiveItem();
        if (activeItem == null) return default;

        return activeItem.GetSelectedWeaponID();
    }

    private ItemPickup GetActiveItem()
    {
        foreach (var item in items)
        {
            if (item != null && item.gameObject.activeSelf)
                return item;
        }
        return null;
    }
    public void DisableBasket()
    {
        items[index].OnAnyItemPicked -= OnItemPicked;
        items[index].gameObject.SetActive(false);
    }
    private void OnItemPicked()
    {
        OnAnyItemPicked?.Invoke();
    }
}