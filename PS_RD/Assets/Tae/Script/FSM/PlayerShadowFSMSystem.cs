using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PlayerShadowFSMStateBase : IFSMStateBase
{
    private PlayerShadowFSMSystem _systemMgr = null;
    public PlayerShadowFSMSystem SystemMgr => _systemMgr;

    public PlayerShadowFSMStateBase(PlayerShadowFSMSystem system)
    {
        _systemMgr = system;
    }

    public abstract void StartState();

    public abstract void Update();

    public abstract void EndState();
}

public class PlayerShadowFSMSystem : FSMSystem<CustomFSMState, PlayerShadowFSMStateBase>
{
    private PlayerShadowUnit _unit = null;
    public PlayerShadowUnit Unit => _unit;

    public event UnityAction OnAttackEndAniEvent = null;

    private void Start()
    {
        SetUnit(GetComponentInParent<PlayerShadowUnit>());
    }

    private class IdleState : PlayerShadowFSMStateBase
    {
        public IdleState(PlayerShadowFSMSystem system) : base(system)
        {
        }

        public override void EndState()
        {

        }

        public override void StartState()
        {

        }

        public override void Update()
        {
            SystemMgr.Unit.Idle();
            SystemMgr.Unit.Progress();
        }
    }


    protected override void RegisterState()
    {
        AddState(CustomFSMState.Idle, new IdleState(this));
    }

    public void SetUnit(PlayerShadowUnit unit)
    {
        if (unit == null)
            return;

        _unit = unit;
    }

    public void EndAttack()
    {
        OnAttackEndAniEvent?.Invoke();
    }

    public void AttackCancleAble()
    {

    }
}
