using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(FlashEffect))]
public class PlayerAction : MonoBehaviour
{
    [SerializeField] public PlayerEquipment Equipment;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;
    public Transform RightHand => rightHand;
    public Transform LeftHand => leftHand;
    private InputManager inputManager;
    private IWeaponCommand command;
    private AttackContext context;
    private PlayerStats stats;
    private FlashEffect effect;
    private Rigidbody rb;
    private Animator ani;

    public bool isKnockback = false;
    public bool hasDamaged = false;
    public bool forStopMove = false;
    public bool forStopRotate = false;

    public AttackDebugGizmo gizmo;

    public UnityEvent onInteraction;

    [SerializeField] private GameObject dieUI;

    private Coroutine forSubscribe_Co;
    private bool isSubscribed = false;

    private void Awake()
    {
        if (Equipment == null)
        {
            Equipment = new PlayerEquipment();
        }
        TryGetComponent(out stats);
        TryGetComponent(out effect);
        TryGetComponent(out inputManager);
        TryGetComponent(out gizmo);
        TryGetComponent(out rb);
        ani = GetComponentInChildren<Animator>();

    }

    private void OnEnable()
    {
        if (forSubscribe_Co != null)
        {
            StopCoroutine(forSubscribe_Co);
            forSubscribe_Co = null;
        }
        forSubscribe_Co = StartCoroutine(subscribe_Co());
    }

    private void OnDisable()
    {
        if (forSubscribe_Co != null)
        {
            StopCoroutine(forSubscribe_Co);
            forSubscribe_Co = null;
        }

        if (isSubscribed && GameManager.instance != null)
        {
            GameManager.instance.playerDie -= onDie;
        }
        isSubscribed = false;
    }

    private IEnumerator subscribe_Co()
    {
        while (GameManager.instance == null)
        {
            yield return null;
        }

        if (!isSubscribed)
        {
            GameManager.instance.playerDie += onDie;
            isSubscribed = true;
        }

        forSubscribe_Co = null;
    }

    private void Update()
    {
        if (stats.isDash || Equipment.MainWeapon == null) return;
        if (GameManager.instance.isStop) return;

        if (inputManager.isAttackPressed)
        {
            if (Equipment.MainWeapon.CanAttack())
            {
                OnAttack();
            }
        }
    }

    public void checkWeapon()
    {
        if(Equipment.MainWeapon == null)
        {
            GameManager.instance.isGetWeapon = false;
        }
        else
        {
            GameManager.instance.isGetWeapon = true;
        }
    }

    public void OnAttack()
    {
        context = new AttackContext();
        RebuildAttackCmd();
        command?.execute();
    }



    public bool TryBuyShopItem(shopItem itemType, int price, int value)
    {
        if (stats == null) return false;
        if (stats.Gold < price) return false;

        switch (itemType)
        {
            case shopItem.Heart:
                if (stats.CurrentHP >= stats.MaxHP) return false;
                break;

            case shopItem.Cristal:
                break;

            case shopItem.refillEcho:
                if (Equipment == null) return false;
                if (Equipment.SubWeapon == null) return false;
                if (Equipment.SubWeapon.resonanceCount >= Equipment.SubWeapon.weaponData.resonanceCount) return false;
                break;
        }

        if (!stats.TryUseGold(price)) return false;

        switch (itemType)
        {
            case shopItem.Heart:
                stats.getHeart(value); 
                break;

            case shopItem.Cristal:
                stats.getCristal(value);
                break;

            case shopItem.refillEcho:
                int maxCount = Equipment.SubWeapon.weaponData.resonanceCount;
                int nextCount = Equipment.SubWeapon.resonanceCount + value;

                if (nextCount > maxCount)
                {
                    nextCount = maxCount;
                }

                Equipment.SubWeapon.SetResonance(nextCount);

                if (GameManager.instance != null)
                {
                    GameManager.instance.setResonance();
                }
                break;
        }

        return true;
    }

    private IEnumerator superArmor()
    {
        hasDamaged = true;
        yield return new WaitForSeconds(stats.invincibilityTime);
        hasDamaged = false;
    }

    public void takeDamage(int damage, Vector3 damagePos, float knockbackForce)
    {
        if (hasDamaged) return;
        if (stats.isDash) return;
        if (GameManager.instance.gamestate != GameManager.GameState.Playing) return;

        if (ani != null) ani.SetTrigger("TakeDamage");
        
        SoundManager.SendEvent(SoundType.SFX_PlayerHit);

        stats.takeDamage(damage);

        Vector3 dir = (damagePos - transform.position).normalized;
        knockback(dir, knockbackForce);

        StartCoroutine(superArmor());

        effect.Flash(stats.FlashAmount, stats.FlashDuration);

        if (stats.isDead)
        {
            GameManager.instance.ChangeState(GameManager.GameState.Die);
        }
    }

    private void onDie()
    {
        StartCoroutine(DieSequence_Co());
    }
    private IEnumerator DieSequence_Co()
    {

        if (ani != null)
        {
            ani.SetBool("Die", true);
        }

        yield return new WaitForSecondsRealtime(3f);

        if (dieUI != null)
        {
            if (!dieUI.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup = dieUI.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f; // 시작은 투명하게
            dieUI.SetActive(true);

            float fadeDuration = 1.5f; // 페이드 속도 (1.5초 동안 켜짐)
            float elapsed = 0f;
            SoundManager.SendEvent(SoundType.SFX_PlayerDie);

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime; // 게임 정지 대비 unscaled 사용
                canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f; // 확실하게 1로 고정
        }

        if (Equipment.SubWeapon != null) 
        {
            Equipment.SubWeapon.SetDualWeaponActive(false); 
            Equipment.SubWeapon.gameObject.SetActive(false);
            Equipment.SubWeapon = null; 
        }

        if (Equipment.MainWeapon != null) 
        {
            Equipment.MainWeapon.SetDualWeaponActive(false);
            Equipment.MainWeapon.gameObject.SetActive(false); 
            Equipment.MainWeapon = null; 
        }
        checkWeapon();
        if (GameManager.instance != null)
        {
            GameManager.instance.TurnWeaponUI(null, null);
            GameManager.instance.setResonance();
        }
        if (ani != null)
        {
            ani.SetInteger("WeaponGroup", 0);
        }
        stats.resetGold();
        stats.setCurrentHP();
    }
    public void CloseDieUI()
    {
        StartCoroutine(FadeOutDieUI_Co());
    }

    private IEnumerator FadeOutDieUI_Co()
    {
        if (dieUI != null && dieUI.TryGetComponent(out CanvasGroup canvasGroup))
        {
            float fadeDuration = 1.0f; // 페이드 아웃 속도 (1초 동안 사라짐)
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime; // 게임 정지 상태일 수 있으므로 unscaled 사용
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            
            dieUI.SetActive(false);
        }

        if (ani != null)
        {
            ani.SetBool("Die", false);
            ani.updateMode = AnimatorUpdateMode.Normal; // 다시 정상 속도로 복구
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.ChangeState(GameManager.GameState.Playing);
        }

    }
    private void knockback(Vector3 dir, float knockbackForce)
    {
        //if (Equipment.MainWeapon.IsCharging) return;

        if (isKnockback) return;
        isKnockback = true; 
        StartCoroutine(knockBack_Co(dir, knockbackForce));
    }

    private IEnumerator knockBack_Co(Vector3 dir, float knockbackForce)
    {
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;

        Vector3 finalDir = dir;
        finalDir.y = 0;

        rb.AddForce(finalDir * knockbackForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f); // �˹� �ð� (�ʿ�� ������ ��ü)

        isKnockback = false;
    }

    public void OnWeaponAcquire(WeaponID ID)
    {
        WeaponAbstract[] weapons = GetComponentsInChildren<WeaponAbstract>(true);

        WeaponAbstract target = null;

        foreach (WeaponAbstract weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
            if (weapon.weaponID == ID)
            {
                target = weapon;
                break;
            }
        }

        if (target == null)
        {
            return;
        }

        Equipment.EquipWeapon(target);
        Equipment.MainWeapon.Initialize(this.ani);
        checkWeapon();

        ani.runtimeAnimatorController = target.overrideController;

        if (gizmo.mainWeapon == null)
        {
            gizmo.mainWeapon = target;//gizmo
        }
        else
        {
            gizmo.subWeapon = gizmo.mainWeapon;
            gizmo.mainWeapon = target;
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.TurnWeaponUI(Equipment.MainWeapon, Equipment.SubWeapon);
        }
    }

    private void RebuildAttackCmd()
    {
        AttackCommand mainAttack = new AttackCommand(Equipment.MainWeapon, context);

        if (Equipment.SubWeapon == null)
        {
            command = mainAttack;
        }
        else
        {
            OnEchoCommand subEcho = new OnEchoCommand(Equipment.SubWeapon, context);
            command = new ComboAttackCommand(mainAttack, subEcho, this);
        }
    }
}
