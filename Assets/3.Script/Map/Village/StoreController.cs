using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StoreController : MonoBehaviour
{
    private Dictionary<int, UpgradeData> upgradeTable;
    private Dictionary<int, int> playerUpgradesState;

    private int cost;
    private float playerValue = 0;
    private int playerMaterial = 0;
    private UpgradeData data;
    private int state;
    private UpgradeState upgradeState;
    // CostMultiplier 업그레이드 비용 상승률
    private void Awake()
    {
        upgradeTable = UpgradeTableLoader.Load();
        playerUpgradesState = new Dictionary<int, int>();
        foreach (var id in upgradeTable.Keys)
            playerUpgradesState[id] = 0;
    }
    public void StoreStateLoad(int id)
    {
        // 업그레이드 비용 가져오기
        data = upgradeTable[id];
    }
    public void PlayerStateLoad(int id)
    {
        // Player 업그레이드 상태 가져오기
        state = playerUpgradesState[id];
    }
    public UpgradeState GetUpgradeState(int id)
    {
        upgradeState = new UpgradeState();
        upgradeState.currentLevel = playerUpgradesState[id];
        upgradeState.maxLevel = upgradeTable[id].maxLv;
        upgradeState.nextCost = CalculateCost();
        upgradeState.currentValue = upgradeTable[id].valuePerLv;
        upgradeState.canUpgrade = CanUpgrade();
        return upgradeState;
    }
    private int CalculateCost()
    {
        return Mathf.RoundToInt(data.baseCost * Mathf.Pow(data.costIncrease, state));
    }
    public void PlayerStateUpgrade(int id)
    {
        // Player 업그레이드 상태 가져오기
        state = playerUpgradesState[id];
    }
    // control upgrade state
    public bool TryUpgrade(int id)
    {

        // 1st. id use
        // Dictionary 에서 일치하는 key 값 찾기 Dictionary.key(id) value 값으로 값 참조
        // 2nd. value 값으로 maxLevel에 도달했는지 찾기
        StoreStateLoad(id);
        PlayerStateLoad(id);
        cost = CalculateCost();
        if (!CanUpgrade())
            return false;
        playerMaterial -= cost;
        state++;
        playerValue += data.valuePerLv;
        playerUpgradesState[id] = state;
        return true;
    }
    private bool CanUpgrade()
    {
        if (state.Equals(data.maxLv)) return false;                              // 이미 MaxLevel이면 반환
        if (playerMaterial < cost) return false;                                       // 소지한 재화 부족
        return true;
    }
    public void PlayerControl()
    {
        // Player 움직임 제한 
    }
}

