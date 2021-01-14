﻿using System.Collections;
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
    private Unit _unit = null;
    public Unit Unit => _unit;

    private class IdleState : CustomFSMStateBase
    {
        public IdleState(CustomFSMSystem system) : base(system)
        {
        }

        public override void EndState()
        {
            Debug.Log("IdleState End");
        }

        public override void StartState()
        {
            if (SystemMgr.Unit == null)
                return;

            SystemMgr.Unit.StopMove();

            Debug.Log("IdleState Start");
        }

        public override void Update()
        {
            //if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            //    SystemMgr.ChangeState(CustomFSMState.Move);

            SystemMgr.Unit.Progress();
            SystemMgr.Unit.Idle();

            if (Input.GetAxisRaw("Horizontal") != 0)
                SystemMgr.ChangeState(CustomFSMState.Move);
        }
    }

    private class MoveState : CustomFSMStateBase
    {
        public MoveState(CustomFSMSystem system) : base(system) { }

        public override void EndState()
        {
            Debug.Log("MoveState End");
        }

        public override void StartState()
        {
            if (SystemMgr.Unit._StopMoveCoroutine != null)
                SystemMgr.Unit.StopCoroutine(SystemMgr.Unit._StopMoveCoroutine);

            Debug.Log("MoveState Start");
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            SystemMgr.Unit.Move(Input.GetAxisRaw("Horizontal"));

            if(Input.GetAxisRaw("Horizontal") == 0)
                SystemMgr.ChangeState(CustomFSMState.Idle);

            //if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            //    SystemMgr.ChangeState(CustomFSMState.Idle);
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

    public void SetUnit(Unit unit)
    {
        if (unit == null)
            return;

        _unit = unit;
    }
}
