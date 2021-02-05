using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadowUnit : UnitBase
{
    private Unit _playerUnit;

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

    
}
