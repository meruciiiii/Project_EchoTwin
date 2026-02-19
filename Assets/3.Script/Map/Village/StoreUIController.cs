using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StoreUIController : MonoBehaviour
{
    public enum UpgradeType
    {
        MaxHP,
        BaseDamage,
        MoveSpeed,
        EchoDamage,
        AttackRange,
        AttackSpeed,
        Count
    }
    GameObject storeSpace;
    private Transform[] upgradeSpace;
    Dictionary<UpgradeType, UpgradeUISet> uiSet;


    private void Awake()
    {
        storeSpace = GameObject.FindGameObjectWithTag("Store");
        if (storeSpace == null)
        {
            Debug.LogError("storeSpace object not found");
            return;
        }

        uiSet = new Dictionary<UpgradeType, UpgradeUISet>();

        Transform[] children = storeSpace.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (!child.CompareTag("Upgrade")) continue;

            string upgradeName = child.name.Replace("space", "");
            if (!Enum.TryParse(upgradeName, out UpgradeType type))
            {
                Debug.LogWarning($"UpgradeType not found for {child.name}");
                continue;
            }

            UpgradeUISet set = new UpgradeUISet();

            set.slider = child.Find("Slider")?.GetComponent<Slider>();
            set.value = child.Find("CurrentValue")?.GetComponent<Text>();
            set.cost = child.Find("Cost")?.GetComponent<Text>();
            set.button = child.Find("Button")?.GetComponent<Button>();

            if (set.slider == null || set.value == null || set.cost == null || set.button == null)
            {
                Debug.LogError($"UI missing in {child.name}");
                continue;
            }

            uiSet[type] = set;
        }
        //upgradeSliders[type].maxValue = maxUpgrade; 12200
    }
    public void SetSliderValue(UpgradeType type, float grade)
    {
        uiSet[type].slider.value = grade;
    }
    public void StoreOpen()
    {
        storeSpace.SetActive(true);
        // 상점 UI 켜기
    }
    public void RefreshUI(Dictionary<UpgradeType, UpgradeState> states)
    {
        foreach (var pair in states)
        {
            UpgradeType type = pair.Key;
            UpgradeState state = pair.Value;

            if (!uiSet.TryGetValue(type, out UpgradeUISet ui))
                continue;

            // Slider
            ui.slider.maxValue = state.maxLevel;
            ui.slider.value = state.currentLevel;
                
            // Text
            ui.value.text = state.currentValue.ToString();
            ui.cost.text = state.currentLevel >= state.maxLevel
                ? "MAX"
                : state.nextCost.ToString();

            // Button
            ui.button.interactable = state.canUpgrade;
        }
    }
    public void StoreClose()
    {
        storeSpace.SetActive(false);
        // 상점 UI 끄기
    }
}
public class UpgradeState
{
    public int currentLevel;
    public int maxLevel;
    public int nextCost;
    public float currentValue;
    public bool canUpgrade;
}
class UpgradeUISet
{
    public Slider slider;
    public Text value;
    public Text cost;
    public Button button;
}
