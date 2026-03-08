using System.Collections.Generic;
using UnityEngine;
public class StartRoom : MonoBehaviour
{
    [SerializeField] private Basket[] items = new Basket[2];
    public void SetStartRoom()
    {
        HashSet<WeaponID> usedWeapons = new HashSet<WeaponID>();
        foreach (var basket in items)
        {
            if (basket == null) continue;
            int safety = 20; // 무한루프 방지
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
}