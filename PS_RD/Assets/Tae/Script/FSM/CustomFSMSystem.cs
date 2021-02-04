﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CustomFSMState
{
    Idle = 0,
    Move,
    Jump,
    Attack,
    Dash,
    Hit,
    Fall,
    Contact,
    DoubleJump,
    JumpAttack,
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

    public event UnityAction OnAttackEndAniEvent = null;

    private class IdleState : CustomFSMStateBase
    {
        public IdleState(CustomFSMSystem system) : base(system)
        {
        }

        public override void EndState()
        {
            //Debug.Log("IdleState End");
        }

        public override void StartState()
        {
            if (SystemMgr.Unit == null)
                return;

            SystemMgr.Unit.StopMove();
            SystemMgr.Unit.CurAniState = AniState.Idle;
            //Debug.Log("IdleState Start");
        }

        public override void Update()
        {
            //if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            //    SystemMgr.ChangeState(CustomFSMState.Move);

            SystemMgr.Unit.Progress();
            SystemMgr.Unit.Idle();

            if (Input.GetAxisRaw("Horizontal") != 0)
                SystemMgr.ChangeState(CustomFSMState.Move);
            else if (Input.GetKeyDown(KeyCode.C))
                SystemMgr.ChangeState(CustomFSMState.Jump);
            else if (Input.GetKeyDown(KeyCode.X))
                SystemMgr.ChangeState(CustomFSMState.Attack);
            else if (Input.GetKeyDown(KeyCode.Space))
                SystemMgr.ChangeState(CustomFSMState.Dash);
        }
    }

    private class MoveState : CustomFSMStateBase
    {
        float _changeStateTimer = 0.25f;
        bool _isStartStateTimer = false;

        public MoveState(CustomFSMSystem system) : base(system) { }

        public override void EndState()
        {
            //Debug.Log("MoveState End");
        }

        public override void StartState()
        {
            if (SystemMgr.Unit._StopMoveCoroutine != null)
                SystemMgr.Unit.StopCoroutine(SystemMgr.Unit._StopMoveCoroutine);

            _changeStateTimer = 0.25f;
            _isStartStateTimer = false;
            SystemMgr.Unit.CurAniState = AniState.Move;
            //Debug.Log("MoveState Start");
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            SystemMgr.Unit.Move(Input.GetAxisRaw("Horizontal"));
            SystemMgr.Unit.CheckMovementDir();
            

            if (Input.GetKeyDown(KeyCode.C))
                SystemMgr.ChangeState(CustomFSMState.Jump);
            else if (Input.GetKeyDown(KeyCode.X))
                SystemMgr.ChangeState(CustomFSMState.Attack);
            else if (Input.GetKeyDown(KeyCode.Space))
                SystemMgr.ChangeState(CustomFSMState.Dash);

            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                _isStartStateTimer = true;
            }

            if (_isStartStateTimer) // 키보드를 좌우 연타 했을 때 idle로 넘어가는 시간에 대한 유예 값.
            {
                _changeStateTimer -= Time.deltaTime;

                if (_changeStateTimer >= 0.001f)
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        _changeStateTimer = 0.25f;
                        _isStartStateTimer = false;
                        return;
                    }
                }
                else
                    SystemMgr.ChangeState(CustomFSMState.Idle);
            }
        }

        private bool StateChangeCheck() // idle <-> Move 간 딜레이없는 빠른 교체를 막기 위한 함수
        {
            if (_changeStateTimer >= 0.01f)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    _changeStateTimer = 0.15f;
                    return true;
                }
            }
            else
                return false;

            return true;
        }
    }

    private class AttackState : CustomFSMStateBase
    {
        int _attackIndex = 0;
        int _nextAttackIndex = 0;
        int _attackMaxIndex = 2;
        float _attackInputTime = 0.0f;
        float _attackBeInputTime = 0.0f;
        float _attackTime = 0.5f;
        AniState[] _attackAniIndex = new AniState[] { AniState.Attack, AniState.Attack2, AniState.Attack3 };

        public AttackState(CustomFSMSystem system) : base(system)
        {
            system.OnAttackEndAniEvent += EndOrNextCheck;
        }

        public override void EndState()
        {
            _attackIndex = 0;
            SystemMgr.Unit.curAttackIndex = _attackIndex;
            _nextAttackIndex = 0;
            _attackInputTime = 0;
            _attackBeInputTime = 0;
        }

        public override void StartState()
        {
            SystemMgr.Unit.StopMove();
            _attackBeInputTime = Time.time;
            SystemMgr.Unit.CurAniState = AniState.Attack;
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            SystemMgr.Unit.Move(Input.GetAxisRaw("Horizontal"));

            if (Input.GetKeyDown(KeyCode.X))
            {
                _attackInputTime = Time.time;

                if(_attackInputTime - _attackBeInputTime <= _attackTime)
                {
                    // 1타 > 2타 > 3타. 현재 애니메이션은 끊기면 안됨.
                    _nextAttackIndex = _attackIndex + 1;
                }

                _attackBeInputTime = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
                SystemMgr.ChangeState(CustomFSMState.Dash);
        }

        private void EndOrNextCheck()
        {
            if (_attackIndex != _nextAttackIndex)
            {
                if (_nextAttackIndex > _attackMaxIndex)
                {
                    _attackIndex = 0;
                    _nextAttackIndex = 0;
                }
                else
                {
                    _attackIndex = _nextAttackIndex;
                }

                Debug.Log("_nextAttackIndex : " + _nextAttackIndex);
                Debug.Log("_attackIndex : " + _attackIndex);
                SystemMgr.Unit.curAttackIndex = _attackIndex;
                SystemMgr.Unit.CurAniState = _attackAniIndex[_attackIndex];
            }
            else
            {
                Debug.Log("Idle");
                SystemMgr.ChangeState(CustomFSMState.Idle);
            }
        }
    }

    private class JumpAttackState : CustomFSMStateBase
    {
        public JumpAttackState(CustomFSMSystem system) : base(system)
        {
        }

        public override void EndState()
        {
        }

        public override void StartState()
        {
            SystemMgr.Unit.CurAniState = AniState.JumpAttack;
            SystemMgr.Unit.JumpAttack();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            SystemMgr.Unit.Move(Input.GetAxisRaw("Horizontal"));

            if (SystemMgr.Unit.IsGround)
            {
                if (Input.GetAxisRaw("Horizontal") == 0)
                    SystemMgr.ChangeState(CustomFSMState.Idle);
                else if (Input.GetAxisRaw("Horizontal") != 0)
                    SystemMgr.ChangeState(CustomFSMState.Move);
            }
        }
    }


    private class JumpState : CustomFSMStateBase
    {
        float _addJumpPower = 3.0f;
        float _doubleJumpPower = 4.0f;
        float _jumpTimeCounter = 0.1f;
        int _jumpCount = 1;
        public JumpState(CustomFSMSystem system) : base(system)
        {
        }

        public override void EndState()
        {
            //Debug.Log("JumpState End");
        }

        public override void StartState()
        {
            _jumpCount = 1;

            if (SystemMgr.Unit.CoyoteTime >= 0.0f)
            {
                _addJumpPower = 0.2f;
                _jumpTimeCounter = 0.1f;
                SystemMgr.Unit.Jump(_addJumpPower);
                SystemMgr.Unit.CurAniState = AniState.Jump;
                //Debug.Log("JumpState Start");
            }
            else
            {
                if (_jumpCount >= 1)
                    DoubleJump();
                else
                    SystemMgr.ChangeState(CustomFSMState.Idle);
            }
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            SystemMgr.Unit.Move(Input.GetAxisRaw("Horizontal"));


            if (_jumpCount >= 1 && Input.GetKeyDown(KeyCode.C))
            {
                DoubleJump();
            }

            if (Input.GetKey(KeyCode.C))
            {
                if (_jumpTimeCounter > 0.0f)
                    SystemMgr.Unit.Jump(_addJumpPower);
                _jumpTimeCounter -= Time.deltaTime;
            }

            if (Input.GetKeyUp(KeyCode.C))
            {
                _jumpTimeCounter = 0;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                SystemMgr.ChangeState(CustomFSMState.JumpAttack);
            }

            if (SystemMgr.Unit.IsGround)
            {
                if (Input.GetAxisRaw("Horizontal") == 0)
                    SystemMgr.ChangeState(CustomFSMState.Idle);
                else if (Input.GetAxisRaw("Horizontal") != 0)
                    SystemMgr.ChangeState(CustomFSMState.Move);
            }
        }

        private void DoubleJump()
        {
            SystemMgr.Unit.Jump(_doubleJumpPower);
            SystemMgr.Unit.CurAniState = AniState.DoubleJump;
            _jumpCount--;
        }
    }

    private class DashState : CustomFSMStateBase
    {
        public DashState(CustomFSMSystem system) : base(system)
        {
        }

        public override void EndState()
        {
        }

        public override void StartState()
        {
            SystemMgr.Unit.CurAniState = AniState.Dash;
            SystemMgr.Unit.Dash();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();

            if (Input.GetKeyDown(KeyCode.Z))
                SystemMgr.ChangeState(CustomFSMState.Idle);
        }
    }


    protected override void RegisterState()
    {
        AddState(CustomFSMState.Idle, new IdleState(this));
        AddState(CustomFSMState.Move, new MoveState(this));
        AddState(CustomFSMState.Jump, new JumpState(this));
        AddState(CustomFSMState.Attack, new AttackState(this));
        AddState(CustomFSMState.Dash, new DashState(this));
        AddState(CustomFSMState.JumpAttack, new JumpAttackState(this));
    }

    public void SetUnit(Unit unit)
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
