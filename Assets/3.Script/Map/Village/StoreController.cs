using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StoreController : MonoBehaviour
{
    private Dictionary<int, UpgradeData> upgradeTable;
    private Dictionary<int, int> playerUpgradesState;

    private int cost;
    //private float playerValue = 0;
    //private int playerMaterial = 0;
    private UpgradeData data;
    private int state;
    private UpgradeState upgradeState;

    private PlayerStats stats;

    // CostMultiplier 업그레이드 비용 상승률
    private void Awake()
    {
        upgradeTable = UpgradeTableLoader.Load();
        playerUpgradesState = new Dictionary<int, int>();
        foreach (var id in upgradeTable.Keys)
            playerUpgradesState[id] = 0;
    }

    public void setPlayerStats(PlayerStats stats)
    {
        this.stats = stats;
    }

    public void clearPlayerStats()
    {
        stats = null;
    }

    private void applyUpgradePlayer(int id)
    {
        switch ((StoreUIController.UpgradeType)id)
        {
            case StoreUIController.UpgradeType.MaxHP:
                stats.getMaxHP((int)data.valuePerLv);
                break;

            case StoreUIController.UpgradeType.BaseDamage:
                stats.getPlayerDMG(data.valuePerLv);
                break;

            case StoreUIController.UpgradeType.MoveSpeed:
                stats.getMoveSpeed(data.valuePerLv);
                break;

            case StoreUIController.UpgradeType.EchoDamageRatio:
                stats.getEchoDamage(data.valuePerLv);
                break;

            case StoreUIController.UpgradeType.AttackRange:
                stats.getAttackRange(data.valuePerLv);
                break;

            case StoreUIController.UpgradeType.AttackSpeed:
                stats.getAttackSpeed(data.valuePerLv);
                break;
        }
    }

    private float getCurrentValue(int id)
    {
        if (stats == null) return 0f;

        switch ((StoreUIController.UpgradeType)id)
        {
            case StoreUIController.UpgradeType.MaxHP:
                return stats.MaxHP;

            case StoreUIController.UpgradeType.BaseDamage:
                return stats.PlayerDMG;

            case StoreUIController.UpgradeType.MoveSpeed:
                return stats.MoveSpeed;

            case StoreUIController.UpgradeType.EchoDamageRatio:
                return stats.EchoDamage;

            case StoreUIController.UpgradeType.AttackRange:
                return stats.AttackRange;

            case StoreUIController.UpgradeType.AttackSpeed:
                return stats.AttackSpeed;
        }

        return 0f;
    }

    public Dictionary<StoreUIController.UpgradeType, UpgradeState> getAllUpgradeState()
    {
        Dictionary<StoreUIController.UpgradeType, UpgradeState> states =
            new Dictionary<StoreUIController.UpgradeType, UpgradeState>();

        foreach (var pair in upgradeTable)
        {
            if (!Enum.IsDefined(typeof(StoreUIController.UpgradeType), pair.Key))
                continue;

            states[(StoreUIController.UpgradeType)pair.Key] = GetUpgradeState(pair.Key);
        }

        return states;
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
        StoreStateLoad(id);
        PlayerStateLoad(id);
        cost = CalculateCost();

        upgradeState = new UpgradeState();
        upgradeState.currentLevel = playerUpgradesState[id];
        upgradeState.maxLevel = upgradeTable[id].maxLv;
        upgradeState.nextCost = cost;
        upgradeState.currentValue = getCurrentValue(id);
        upgradeState.canUpgrade = CanUpgrade();

        return upgradeState;
    }
    private int CalculateCost()
    {
        return Mathf.RoundToInt(data.baseCost * Mathf.Pow(data.costIncrease, state));
    }
    //public void PlayerStateUpgrade(int id)
    //{
    //    // Player 업그레이드 상태 가져오기
    //    state = playerUpgradesState[id];
    //}
    // control upgrade state
    public bool TryUpgrade(int id)
    {
        if (stats == null) return false;

        // 1st. id use
        // Dictionary 에서 일치하는 key 값 찾기 Dictionary.key(id) value 값으로 값 참조
        // 2nd. value 값으로 maxLevel에 도달했는지 찾기
        StoreStateLoad(id);
        PlayerStateLoad(id);
        cost = CalculateCost();
        if (!CanUpgrade())
            return false;
        if(stats.TryUseCristal(cost))
            return false;
        //playerMaterial -= cost;
        state++;
        //playerValue += data.valuePerLv;
        //playerUpgradesState[id] = state;
        applyUpgradePlayer(id);
        return true;
    }
    private bool CanUpgrade()
    {
        //if (state.Equals(data.maxLv)) return false;                              // 이미 MaxLevel이면 반환
        //if (playerMaterial < cost) return false;                                       // 소지한 재화 부족
        if (stats == null) return false;
        if (state >= data.maxLv) return false;
        if (stats.Cristal < cost) return false;
        return true;
    }
    public void PlayerControl()
    {
        // Player 움직임 제한 
    }
}

