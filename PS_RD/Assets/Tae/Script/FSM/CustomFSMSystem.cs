using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomFSMState
{
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

    }
}
