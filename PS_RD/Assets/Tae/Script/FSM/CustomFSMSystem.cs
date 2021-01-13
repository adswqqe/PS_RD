using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomFSMState
{
    Idle,
    Move,
    Attack,
    Jump,
}

public abstract class CustomFSMStateBase : IFSMStateBase
{
    private CustomFSMSystem _systemMgr = null;
    public CustomFSMSystem SystemMgr => _systemMgr;

    public CustomFSMStateBase(CustomFSMSystem system)
    {
        _systemMgr = system;
    }

    public abstract void StartState();

    public abstract void Update();

    public abstract void EndState();
}

public class CustomFSMSystem : FSMSystem<CustomFSMState, CustomFSMStateBase>
{
    private UnitBase _unit = null;
    public UnitBase Unit => _unit;

    private class IdleState : CustomFSMStateBase
    {
        public IdleState(CustomFSMSystem system) : base(system)
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

    private class MoveState : CustomFSMStateBase
    {
        public MoveState(CustomFSMSystem system) : base(system) { }

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

    private class AttackState : CustomFSMStateBase
    {
        public AttackState(CustomFSMSystem system) : base(system)
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

    private class JumpState : CustomFSMStateBase
    {
        public JumpState(CustomFSMSystem system) : base(system)
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
        AddState(CustomFSMState.Idle, new IdleState(this));
        AddState(CustomFSMState.Move, new MoveState(this));
        AddState(CustomFSMState.Jump, new JumpState(this));
        AddState(CustomFSMState.Attack, new AttackState(this));
    }

    public void SetUnit(UnitBase unit)
    {
        if (unit == null)
            return;

        _unit = unit;
    }
}
