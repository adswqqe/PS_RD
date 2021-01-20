using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField, ReadOnly]
    private bool _isAttackCancleAble = false;

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

        public AttackState(CustomFSMSystem system) : base(system)
        {
        }

        public override void EndState()
        {
            _attackIndex = 0;
        }

        public override void StartState()
        {
            Debug.Log("Attack State : " + _attackIndex);
            SystemMgr._isAttackCancleAble = false;
            SystemMgr.Unit.CurAniState = AniState.Attack;
            SystemMgr._unit.Attack();
            _attackIndex++;
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();

            if (Input.GetKeyDown(KeyCode.X))
            {
                if(SystemMgr._isAttackCancleAble)
                {
                    if (_attackIndex == 1)
                    {
                        SystemMgr.Unit.CurAniState = AniState.Attack2;
                        _attackIndex++;

                    }
                    else if (_attackIndex == 2)
                    {
                        SystemMgr.Unit.CurAniState = AniState.Attack3;
                        _attackIndex++;
                    }

                    SystemMgr._isAttackCancleAble = false;

                }
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

            if (SystemMgr.Unit.IsGround)
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

    public void EndAttack()
    {
        ChangeState(CustomFSMState.Idle);
    }

    public void AttackCancleAble()
    {
        Debug.Log("asdasd");
        _isAttackCancleAble = true;
    }
}
