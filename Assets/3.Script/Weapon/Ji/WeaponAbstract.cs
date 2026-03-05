using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    onehand,
    twohand,
    dual,
}

public enum WeaponID
{
    Sword,
    Hammer,
    Dagger,
    Spear,
    Axe,
}

public abstract class WeaponAbstract : MonoBehaviour
{
    [SerializeField] public WeaponData weaponData;
    //[SerializeField] protected CharacterData characterData;
    [SerializeField] protected PlayerStats stats;
    [SerializeField] protected Animator animator;
    //[SerializeField] protected PlayerEquipment equipment;
    [SerializeField] protected InputManager input;

    [SerializeField] protected float attackAngle = 90f;

    [Serializable]
    public class AttackEffectData
    {
        public GameObject prefab; 
        public float forwardOffset = 1.0f; 
        public float upOffset = 1.0f;     
        public float scale = 1.0f;         
    }
    [Header("Effects Settings")]
    [SerializeField] protected AttackEffectData[] attackEffects;

    protected PlayerAction action;

    public WeaponType weaponType;
    public WeaponID weaponID;
    public GameObject DualWeapon;
    [SerializeField] public AnimatorOverrideController overrideController;

    public int resonanceCount = 0;

    protected float lastAttackTime;
    protected int comboCount = 0;
    protected bool isComboCooltime = false;
    protected bool isAttackReserved = false;

    protected float comboExpireTime;
    protected bool isCancelled = false;
    protected bool isCharging = false;
    public bool IsCharging => isCharging;


    protected AttackDebugInfo lastAttackInfo;
    protected bool hasDebugInfo;
    protected List<AttackDebugInfo> echoAttackInfos = new List<AttackDebugInfo>();

    public AttackDebugInfo DebugInfo => lastAttackInfo;
    public bool HasDebugInfo => hasDebugInfo;
    public IReadOnlyList<AttackDebugInfo> EchoAttackInfos => echoAttackInfos;

    private void Awake()
    {
        action = stats.GetComponent<PlayerAction>();
        SetResonance(10);
        SetAttackTime();
    }

    public void Initialize(Animator playerAni)
    {
        this.animator = playerAni;

        float groupValue = (float)weaponType;
        animator.SetFloat("WeaponGroup", groupValue);

        animator.SetInteger("WeaponType", weaponData.ID);
        animator.SetFloat("AttackSpeed", weaponData.attackSpeed);
    }

    #region Combo관련
    public bool CanAttack()
    {
        if (isComboCooltime) return false;
        if (animator.IsInTransition(0)) return false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //Debug.Log(animator.IsInTransition(0) + "animator");
        if (stateInfo.IsTag("Attack"))
        {
            if (stateInfo.normalizedTime < 0.65f)// || stateInfo.normalizedTime > 0.9f)
            {
                return false;
            }
        }

        return true;
    }

    //public virtual bool CanRotate()
    //{
    //    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

    //    if (stateInfo.IsTag("Attack"))
    //    {
    //        if (stateInfo.normalizedTime < 0.65f)
    //        {
    //            return false;
    //        }
    //    }

    //    return true;
    //}

    private void SetAttackTime()
    {
        lastAttackTime = Time.time;
    }

    protected void AttackTimeChecker()
    {
        if (Time.time > lastAttackTime + 2f / weaponData.attackSpeed)
        {
            comboCount = 0;
        }
        lastAttackTime = Time.time;
    }

    protected virtual IEnumerator ComboCooltime_Co()
    {
        if (isCancelled) yield break;
        isComboCooltime = true;
        yield return new WaitForSeconds(weaponData.comboCooltime);
        isComboCooltime = false;
    }

    protected void SetAnimator()
    {
        //animator.SetInteger("ComboState", comboCount);
        //if (comboCount == 0)
        //{
        //    animator.SetTrigger("Attack");
        //}

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsTag("Attack"))
        {
            comboCount = 0;
        }

        animator.SetInteger("ComboState", comboCount);

        if (comboCount == 0)
        {
            animator.SetTrigger("Attack");
        }

        comboCount++;

        if (comboCount >= weaponData.comboCount)
        {
            comboCount = 0;
            StartCoroutine(ComboCooltime_Co());
        }
    }
    #endregion

    #region Echo관련
    public void SetResonance(int count)
    {
        resonanceCount = count;
    }

    public bool canEcho()
    {
        return resonanceCount > 0;
    }

    public void ConsumeResonance()
    {
        resonanceCount--;
    }
    #endregion

    protected virtual void enemyKnockback(Collider target)
    {
        if (target.TryGetComponent<Iknockback>(out Iknockback kb))
        {
            if (target.gameObject.layer == 9) return;
            Vector3 dir = (target.transform.position - transform.position).normalized;
            kb.applyKnockback(dir, weaponData.knockback);
        }
    }

    protected virtual float calcDamage()
    {
        return weaponData.baseDamage + stats.PlayerDMG;// + characterData.valuePerLv 이 부분 정리
    }

    public void AnimationEventEffect(int index)
    {
        if (index >= 100)
        {
            PlayEffect(index - 100, true);
        }
        else
        {
            // 100 미만이면: 정방향으로 실행
            PlayEffect(index, false);
        }
    }

    protected void PlayEffect(int index, bool isFlip)
    {
        if (attackEffects == null || index >= attackEffects.Length || attackEffects[index].prefab == null) return;

        AttackEffectData data = attackEffects[index];

        Vector3 pos = stats.transform.position + (stats.transform.forward * data.forwardOffset) + (Vector3.up * data.upOffset);
        GameObject effect = Instantiate(data.prefab, pos, stats.transform.rotation);

        Vector3 finalScale = Vector3.one * data.scale;

        if (isFlip)
        {
            if (Mathf.Abs(stats.transform.forward.z) > Mathf.Abs(stats.transform.forward.x))
            {
                finalScale.x *= -1;
            }
            else
            {
                finalScale.z *= -1;
            }
        }

        effect.transform.localScale = finalScale;
    }

    public abstract void Attack(AttackContext context);

    public abstract void ChargingAttack();

    public abstract void OnEcho(AttackContext context);
}
