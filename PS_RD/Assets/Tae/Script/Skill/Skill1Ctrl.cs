﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Skill1Ctrl : UnitBase
{
    public Animator _animator;
    public SkillAttack skillAttack;
    public SkillAttack skillExAttack;
    Transform _playerTr;
    Vector3 spawnPlayerPos;
    public Rigidbody2D _rigid2D;
    Vector2 spawnPos;
    float moveX = 4.0f;

    [SerializeField]
    float moveSpped = 5.0f;
    float dir = 0;
    public bool isTransformB = false;

    float _xVelocity = 0.0f;

    public bool isInit = false;
    bool isAnimationEnd = false;

    private PlayerShadowUnit playerShadow;
    public BoxCollider2D aBoxCollider2D;
    public BoxCollider2D bBoxCollider2D;

    bool _isOverCome = false;
    [SerializeField]
    float _damage = 10f;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(Transform playerTr, float dir, PlayerShadowUnit playerShadow)
    {
        _rigid2D = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        bBoxCollider2D.enabled = false;
        _playerTr = playerTr;
        spawnPlayerPos = _playerTr.position;
        spawnPos = transform.position;
        this.dir = dir;
        Debug.Log("dir : " + dir);
        if(dir <= 0)
            transform.Rotate(0.0f, 180.0f, 0.0f);

        this.playerShadow = playerShadow;
        isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInit == false)
            return;

        //if (_isOverCome == true)
        //    return;

        if (isTransformB == false)
        {
            if (Mathf.Abs(spawnPos.x - transform.position.x) >= moveX)
            {
                isTransformB = true;
                InitTransformB();
            }
            else
            {
                _rigid2D.velocity = new Vector2(moveSpped * dir, _rigid2D.velocity.y);
            }
        }
        else
        {
            if (isAnimationEnd == true)
            {
                float posX = Mathf.SmoothDamp(transform.position.x, spawnPlayerPos.x + (0.5f * dir), ref _xVelocity, 0.2f);
                _rigid2D.MovePosition(new Vector2(posX, transform.position.y));
                if (Mathf.Abs(spawnPos.x - transform.position.x) <= 0.03f)
                    DestroySkill();
            }
        }
    }

    private void DestroySkill()
    {
        playerShadow.Skill1End();
        Destroy(this.gameObject);
    }

    private void InitTransformB()
    {
        _animator.SetInteger("State", 1);
        dir *= -1;
        _rigid2D.velocity = Vector2.zero;
        aBoxCollider2D.enabled = false;
        bBoxCollider2D.enabled = true;
    }

    public void SetSkill1Ex()
    {
        _animator.SetInteger("State", 2);
        _playerTr.GetComponent<Unit>().Skill1Ex();
        _isOverCome = true;
        _rigid2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransformB == false)
        {
            if (collision.gameObject.tag == "Player")
                return;

            if(collision.gameObject.tag == "Enemy")
            {
                isTransformB = true;
                collision.GetComponent<UnitBase>().Hit(_damage);
                transform.position = new Vector3(collision.transform.position.x + (2 * dir), collision.transform.position.y, collision.transform.position.z);

                InitTransformB();
            }
        }
        else if (isAnimationEnd == true)
        {
            if(collision.gameObject.tag == "Player")
            {
                if (_isOverCome == true)
                    return;
                collision.gameObject.GetComponent<Unit>().Hit(150);
                collision.gameObject.GetComponent<Unit>().SlowStateStart();
            }
        }
    }

    public void AniEnd()
    {
        isAnimationEnd = true;
    }

    public void Skill1ExAttackTrigger()
    {
        Skill1ExAttack();
    }

    public void Skill1ExAiEnd()
    {
        playerShadow.Skill1End();
        Destroy(this.gameObject);
    }

    public override void Progress()
    {
    }

    public override void Idle()
    {
    }

    public override void Move(float deltaX)
    {
    }

    public override void Attack()   // 애니메이션이 호출해주는거
    {
        skillAttack.SetDamage(0, 10);
        skillAttack.gameObject.SetActive(true);
    }

    public void Skill1ExAttack()
    {
        skillExAttack.SetDamage(0, 10);
        skillExAttack.gameObject.SetActive(true);
    }

    public override void Jump(float height)
    {
    }

    public override void Dash()
    {
    }

    public override void JumpAttack()
    {

    }

    public override void Hit(float damage)
    {

    }
}
