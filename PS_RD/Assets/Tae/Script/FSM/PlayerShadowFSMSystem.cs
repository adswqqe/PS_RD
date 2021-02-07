using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerShadowFSMState
{
    Idle,
    OutOfControl,
    ControlRecovery,
    Destroy,
    Resurrection,
}

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

public class PlayerShadowFSMSystem : FSMSystem<PlayerShadowFSMState, PlayerShadowFSMStateBase>
{
    private PlayerShadowUnit _unit = null;
    public PlayerShadowUnit Unit => _unit;

    public event UnityAction OnAttackEndAniEvent = null;

    private void Start()
    {
        SetUnit(GetComponentInParent<PlayerShadowUnit>());
        Unit.OnLightDetectionAction += OnLightDetection;
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

    private class OutOfControlState : PlayerShadowFSMStateBase
    {
        public OutOfControlState(PlayerShadowFSMSystem system) : base(system)
        {
        }

        public override void EndState()
        {
        }

        public override void StartState()
        {
            SystemMgr.Unit.shadowAniState = ShadowAniState.OutOfControl;
            SystemMgr.Unit.OutOfControl();
        }

        public override void Update()
        {
        }
    }

    private class ControlRecoveryState : PlayerShadowFSMStateBase
    {
        public ControlRecoveryState(PlayerShadowFSMSystem system) : base(system)
        {
        }

        public override void EndState()
        {
        }

        public override void StartState()
        {
            SystemMgr.Unit.shadowAniState = ShadowAniState.ControlRecovery;
            SystemMgr.Unit.ControlRecovery();
        }

        public override void Update()
        {
        }
    }

    private class DestroyState : PlayerShadowFSMStateBase
    {
        public DestroyState(PlayerShadowFSMSystem system) : base(system)
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
        }
    }

    private class ResurrectionState : PlayerShadowFSMStateBase
    {
        public ResurrectionState(PlayerShadowFSMSystem system) : base(system)
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
        }
    }

    protected override void RegisterState()
    {
        AddState(PlayerShadowFSMState.Idle, new IdleState(this));
        AddState(PlayerShadowFSMState.OutOfControl, new OutOfControlState(this));
        AddState(PlayerShadowFSMState.ControlRecovery, new ControlRecoveryState(this));
        AddState(PlayerShadowFSMState.Destroy, new DestroyState(this));
        AddState(PlayerShadowFSMState.Resurrection, new ResurrectionState(this));
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

    private void OnLightDetection()
    {
        ChangeState(PlayerShadowFSMState.OutOfControl);
    }
}
