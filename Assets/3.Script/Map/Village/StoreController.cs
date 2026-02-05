
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreController : MonoBehaviour
{
    Dictionary<int, UpgradeData> upgradeTable;
    Dictionary<int, PlayerUpgradeState> playerUpgrades;

    int cost;
    int playerValue = 0;
    int playerMaterial = 0;
    UpgradeData data;
    PlayerUpgradeState state;
    // CostMultiplier 업그레이드 비용 상승률
    public void StoreStateLoad(int id)
    {
        // 업그레이드 비용 가져오기
        data = upgradeTable[id];
    }
    public void PlayerStateLoad(int id)
    {
        // Player 업그레이드 상태 가져오기
        state = playerUpgrades[id];
    }
    // control upgrade state
    public bool TryUpgrade(int id)
    {

        // 1st. id use
        // Dictionary 에서 일치하는 key 값 찾기 Dictionary.key(id) value 값으로 값 참조
        // 2nd. value 값으로 maxLevel에 도달했는지 찾기
        StoreStateLoad(id);
        PlayerStateLoad(id);
        cost = Mathf.RoundToInt(data.baseCost * Mathf.Pow(data.costMultiplier, state.currentLevel));
        if (!CanUpgrade())
            return false;
        playerMaterial -= cost;
        state.currentLevel++;
        playerValue += data.valuePerLevel;
        return true;
    }
    private bool CanUpgrade()
    {
        if (state.currentLevel.Equals(data.maxLevel)) return false;                              // 이미 MaxLevel이면 반환
        if (playerMaterial < cost) return false;                                       // 소지한 재화 부족
        return true;
    }
    public void PlayerControl()
    {
        // Player 움직임 제한 
    }
}
class UpgradeData
{
    public int id;
    public int maxLevel;
    public int baseCost;
    public float costMultiplier;
    public int valuePerLevel;
}

// 세이브 대상
class PlayerUpgradeState
{
    public int currentLevel;
}