using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private ItemPickup[] items = new ItemPickup[5];
    public void ActivateRandomItem()
    {
        if (items == null || items.Length == 0) return;
        int index = Random.Range(0, items.Length);
        ItemPickup selectedItem = items[index];
        if (selectedItem == null) return;
        // 선택된 아이템 활성화
        selectedItem.gameObject.SetActive(true);
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
}