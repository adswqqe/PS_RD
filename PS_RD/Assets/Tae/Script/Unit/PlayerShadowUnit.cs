﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShadowUnit : UnitBase
{
    private Unit _playerUnit;

    // Compoenent
    private BoxCollider2D boxCollider2D;
    public SpriteRenderer _spriteRenderer;

    // Events
    public event UnityAction OnLightDetectionAction;
    public event UnityAction OnResurrectionAction;
    public event UnityAction OnIdleAction;
    public event UnityAction OnSkill1StartStateAction;

    public ShadowAniState shadowAniState;

    [HideInInspector]
    public bool isControlAble = false;

    private void Start()
    {
        _playerUnit = GetComponentInParent<Unit>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        OutOfControl();
    }

    public override void Progress()
    {

    }

    public override void Attack()
    {

    }

    public override void Idle()
    {
        AniCtrl.PlayAni(_playerUnit.CurAniState);
        isControlAble = true;
    }

    public override void Dash()
    {

    }

    public override void Jump(float height)
    {

    }

    public override void JumpAttack()
    {

    }

    public override void Move(float deltaX)
    {

    }

    public void Skill1()
    {
        OnSkill1StartStateAction?.Invoke();
        AniCtrl.PlayAni(shadowAniState);
        isControlAble = false;
    }

    public void SKill1AniEnd()
    {
        _spriteRenderer.enabled = false;
    }

    public void Skill1End()
    {
        _spriteRenderer.enabled = true;
        OnResurrectionAction?.Invoke();
        isControlAble = true;
    }

    public void OutOfControl()
    {
        Debug.Log("ASDASDSA");
        AniCtrl.PlayAni(shadowAniState);
        isControlAble = false;
    }

    public void LightDetection()
    {
        OnLightDetectionAction?.Invoke();
    }

    public void ControlRecovery()
    {
        AniCtrl.PlayAni(shadowAniState);
        isControlAble = false;
    }

    public void DestroyState()
    {
        AniCtrl.PlayAni(shadowAniState);
        isControlAble = false;
    }

    public void Resurrection()
    {
        AniCtrl.PlayAni(shadowAniState);
        isControlAble = false;
    }

    public override void Hit(float damage)
    {
        // OnCollison으로 변경할 지 고민 해야함.
        OnLightDetectionAction?.Invoke();
    }

    public void ResurrectionAniEnd()
    {
        OnIdleAction?.Invoke();
    }


}
