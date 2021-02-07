using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShadowUnit : UnitBase
{
    private Unit _playerUnit;

    // Events
    public event UnityAction OnLightDetectionAction;

    private void Start()
    {
        _playerUnit = GetComponentInParent<Unit>();
    }

    public override void Progress()
    {

    }

    public override void Attack()
    {

    }

    public override void Idle()
    {
        Debug.Log(_playerUnit.CurAniState);
        AniCtrl.PlayAni(_playerUnit.CurAniState);
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

    public void OutOfControl()
    {

    }

    public void ControlRecovery()
    {

    }

    public void DestroyState()
    {

    }

    public void Resurrection()
    {

    }

    public override void Hit()
    {
        // OnCollison으로 변경할 지 고민 해야함.
        OnLightDetectionAction?.Invoke();
    }


}
