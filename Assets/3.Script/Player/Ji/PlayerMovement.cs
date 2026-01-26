using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    [Space(5f)]
    [SerializeField] private int playerHP = 6; //반칸이 체력 1로 기준을 설정
    [SerializeField] private float playerDMG = 1f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashLength = 1f;
    [SerializeField] private float dashSpeed = 1f;
    [SerializeField] private float dashDelay = 1f;

    public int PlayerHP => playerHP;
    public float PlayerDMG => playerDMG;
    public float MoveSpeed => moveSpeed;
    public float DashLength => dashLength;

    private bool isMain = false;
    private bool isSub = false;

    private bool isDash = false;

    private InputManager Input;
    private Rigidbody rb;

    //[Space(5f)]
    //[Header("Weapon & Magic")]
    //[Space(5f)]
    //[SerializeField] private GameObject weapon;//gameobject 말고 scriptable object 파일 받아오기
    //[SerializeField] private GameObject magic;//이하 동문

    private void Awake()
    {
        TryGetComponent(out Input);
        TryGetComponent(out rb);
    }

    private void FixedUpdate()
    {
        Move();
        FocusOnMouse();
    }

    private void getEquipment(GameObject target)//UI에서 상호작용 키 눌렀을 때 실행되는 method
    {
        //get component script 후에 data file을 갖고와 무기인지 스킬인지 판별
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Equipment"))
        {
            //pick UI popup
        }
    }

    #region Movement

    public void Move()
    {
        Vector2 moveInput = Input.MoveValue;
        Vector3 movePos = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.deltaTime * GameManager.instance.timeScale;

        rb.MovePosition(transform.position + movePos);
    }

    public IEnumerator Dash()
    {
        isDash = true;
        Vector2 dashPos = Input.MoveValue * dashLength;
        Vector3 destPos = new Vector3(transform.position.x + dashPos.x, 0, transform.position.z + dashPos.y);

        float timer = 0;
        while (timer < 1f)
        {
            timer += dashSpeed * Time.deltaTime;

            transform.position = Vector3.Lerp(transform.position, destPos, timer);

            yield return null;
        }
        transform.position = destPos;

        yield return new WaitForSeconds(dashDelay);
        isDash = false;
    }

    private void FocusOnMouse()
    {
        Vector3 mousePos = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.MousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            mousePos = hit.point;
        }
        mousePos.y = transform.position.y;
        transform.LookAt(mousePos);
    }

    public void Attack()
    {

    }

    public void ChargingAttack()
    {

    }

    public void Curse()
    {

    }

    #endregion
}
