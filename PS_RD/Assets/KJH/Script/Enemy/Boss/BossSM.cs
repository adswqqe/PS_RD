using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EnemyBoss
{
    public class BossSM : MonoBehaviour
    {
        public IdleState idleState = new IdleState();
        public ChaseState chaseState = new ChaseState();
        public static RightPunchState rightPunchState = new RightPunchState();
        public static LeftPunchState leftPunchState = new LeftPunchState();
        public static UppercutState uppercutState = new UppercutState();
        public static SeigeModeState seigeModeState = new SeigeModeState();
        public static TornadoState tornadoState = new TornadoState();
        public static JumpAttackState jumpAttackState = new JumpAttackState();
        public GroggyState groggyState = new GroggyState();
        public static DieState dieState = new DieState();

        // 나중에 푸시다운 오토마타로 바꿀 필요가 생길때를 대비해서 리마인딩용으로 남긴 것
        //private BaseState[] _stateStack = new BaseState[10];
        private BaseState _curState;

        [HideInInspector]
        public BossAnimCtrl anim;
        [HideInInspector]
        public Boss unit;
        private UnitBase targetUnit;

        private Vector3 _lookDir;

        private int _curPhase;
        public float Phase1to2Condition;
        public float Phase2to3Condition;
        public int CurPhase { get { return _curPhase; } }

        private bool _isAttacked = false;
        public bool IsAttacked
        {
            get { if (_isAttacked) { _isAttacked = false; return true; } else return false; }
            set { _isAttacked = value; }
        }
        private bool _isAttackEnd = false;
        public bool IsAttackEnd
        {
            get { if (_isAttackEnd) { _isAttackEnd = false; return true; } else return false; }
            set { _isAttackEnd = value; }
        }


        private void Update()
        {
            _curState.process(this);
            unit.Progress();

            float curHpRatio = unit.stat.nowHP / unit.stat.maxHP;
            if (curHpRatio < Phase2to3Condition)
                _curPhase = 2;
            else if (curHpRatio < Phase1to2Condition)
                _curPhase = 1;
        }

        private void Awake()
        {
            unit = GetComponent<Boss>();
            anim = GetComponentInChildren<BossAnimCtrl>();
            Assert.IsNotNull(unit);
            Assert.IsNotNull(anim);
        }

        private void Start()
        {
            _curState = idleState;
            ChangeState(idleState);
            _curPhase = 0;
        }

        public void ChangeState(BaseState state)
        {
            _curState.exit(this);
            _curState = state;
            _curState.enter(this);
        }

        public void DamageEvent()
        {

        }

        public void DieEvent()
        {

        }

        public bool CheckView()
        {
            if (unit.Target == null)
                return false;
            // need to be changed to use another method
            if ((unit.transform.position - unit.Target.transform.position).magnitude < 10)
                return true;

            return false;
        }

        public bool CheckAttackable()
        {
            // need to be changed to use another method
            if ((unit.transform.position - unit.Target.transform.position).magnitude < 5)
                return true;

            return false;
        }

        public bool doesLookRight()
        {
            return _lookDir.x > 0;
        }

        public void SetDirection(bool isRight)
        {
            anim.SetDirection(isRight);
            unit.SetDirection(isRight);
            if (isRight)
            {
                _lookDir = Vector3.right;
            }
            else
            {
                _lookDir = Vector3.left;
            }
        }
    }

    public abstract class BaseState
    {
        public abstract void enter(BossSM fsm);
        public abstract void process(BossSM fsm);
        public abstract void exit(BossSM fsm);
    }

    public class IdleState : BaseState
    {
        public override void enter(BossSM fsm)
        {
            fsm.unit.Idle();
            fsm.anim.ChangeAnim(BossAnim.Idle);
        }

        public override void process(BossSM fsm)
        {
            // about transition
            if (fsm.CheckView())
                fsm.ChangeState(fsm.chaseState);
        }

        public override void exit(BossSM fsm)
        {

        }
    }
    public class ChaseState : BaseState
    {
        private int _lastPhase = 0;
        private int _curAttackOrder = 0;
        private List<BaseState>[] _attackList = new List<BaseState>[3];

        private bool _isInit = false;
        private void Init()
        {
            _attackList[0] = new List<BaseState>();
            _attackList[0].Add(BossSM.leftPunchState);
            _attackList[0].Add(BossSM.rightPunchState);
            _attackList[0].Add(BossSM.uppercutState);

            _isInit = true;
        }

        public override void enter(BossSM fsm)
        {
            if(!_isInit)
            {
                Init();
            }    
            if(_lastPhase != fsm.CurPhase)
            {
                _lastPhase = fsm.CurPhase;
                _curAttackOrder = 0;
            }
            _curAttackOrder = _curAttackOrder % _attackList[fsm.CurPhase].Count;
            fsm.anim.ChangeAnim(BossAnim.Walk);
        }

        public override void process(BossSM fsm)
        {
            if (fsm.CheckAttackable())
            {
                fsm.ChangeState(_attackList[fsm.CurPhase][_curAttackOrder]);
                _curAttackOrder++;
                return;
            }

            fsm.SetDirection(fsm.unit.Target.transform.position.x > fsm.unit.transform.position.x);

            if (fsm.doesLookRight())
            {
                fsm.unit.Move(1);
            }
            else
            {
                fsm.unit.Move(-1);
            }
        }

        public override void exit(BossSM fsm)
        {
            fsm.unit.Idle();
            fsm.anim.ChangeAnim(BossAnim.Idle);
        }
    }
    public class RightPunchState : BaseState
    {
        public override void enter(BossSM fsm)
        {
            fsm.anim.ChangeAnim(BossAnim.RightPunch);
            fsm.unit.Idle();
            fsm.unit.SetAttackBox(0);
        }

        public override void process(BossSM fsm)
        {
            if (fsm.IsAttacked)
            {
                float deltaX;
                Vector3 pos = fsm.unit.transform.position;
                if (fsm.doesLookRight())
                {
                    deltaX = 0.5f;
                }
                else
                {
                    deltaX = -0.5f;
                }
                fsm.unit.transform.position = new Vector3(pos.x + deltaX, pos.y, pos.z);
                fsm.unit.Attack();
            }
            if (fsm.IsAttackEnd)
            {
                switch(fsm.CurPhase)
                {
                    case 0:
                        fsm.ChangeState(fsm.chaseState);
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                }    
            }
        }

        public override void exit(BossSM fsm)
        {

        }
    }
    public class LeftPunchState : BaseState
    {
        public override void enter(BossSM fsm)
        {
            fsm.anim.ChangeAnim(BossAnim.LeftPunch);
            fsm.unit.Idle();
            fsm.unit.SetAttackBox(1);
        }

        public override void process(BossSM fsm)
        {
            if (fsm.IsAttacked)
            {
                float deltaX;
                Vector3 pos = fsm.unit.transform.position;
                if (fsm.doesLookRight())
                {
                    deltaX = 0.5f;
                }
                else
                {
                    deltaX = -0.5f;
                }
                fsm.unit.transform.position = new Vector3(pos.x + deltaX, pos.y, pos.z);
                fsm.unit.Attack();
            }
            if (fsm.IsAttackEnd)
            {
                switch (fsm.CurPhase)
                {
                    case 0:
                        fsm.ChangeState(fsm.chaseState);
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                }
            }
        }

        public override void exit(BossSM fsm)
        {
        }
    }
    public class UppercutState : BaseState
    {
        public override void enter(BossSM fsm)
        {
            fsm.anim.ChangeAnim(BossAnim.Uppercut);
            fsm.unit.Idle();
            fsm.unit.SetAttackBox(2);
        }

        public override void process(BossSM fsm)
        {
            if (fsm.IsAttacked)
            {
                float deltaX;
                Vector3 pos = fsm.unit.transform.position;
                if (fsm.doesLookRight())
                {
                    deltaX = 0.5f;
                }
                else
                {
                    deltaX = -0.5f;
                }
                fsm.unit.transform.position = new Vector3(pos.x + deltaX, pos.y, pos.z);
                fsm.unit.Attack();
            }
            if (fsm.IsAttackEnd)
            {
                switch (fsm.CurPhase)
                {
                    case 0:
                        fsm.ChangeState(fsm.chaseState);
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                }
            }
        }

        public override void exit(BossSM fsm)
        {

        }
    }
    public class SeigeModeState : BaseState
    {
        public override void enter(BossSM fsm)
        {
            fsm.anim.ChangeAnim(BossAnim.SeigeMode);
            fsm.unit.Idle();
        }

        public override void process(BossSM fsm)
        {

        }

        public override void exit(BossSM fsm)
        {

        }
    }
    public class TornadoState : BaseState
    {
        public override void enter(BossSM fsm)
        {
            fsm.anim.ChangeAnim(BossAnim.Tornado);
            fsm.unit.Idle();
            fsm.unit.SetAttackBox(3);
        }

        public override void process(BossSM fsm)
        {

        }

        public override void exit(BossSM fsm)
        {

        }
    }
    public class JumpAttackState : BaseState
    {
        public override void enter(BossSM fsm)
        {
            fsm.anim.ChangeAnim(BossAnim.DownPunch);
            fsm.unit.Idle();
            fsm.unit.SetAttackBox(4);
        }

        public override void process(BossSM fsm)
        {

        }

        public override void exit(BossSM fsm)
        {

        }
    }
    public class GroggyState : BaseState
    {
        public override void enter(BossSM fsm)
        {
            fsm.anim.ChangeAnim(BossAnim.Groggy);
            fsm.unit.Idle();
        }

        public override void process(BossSM fsm)
        {

        }

        public override void exit(BossSM fsm)
        {

        }
    }
    public class DieState : BaseState
    {
        public override void enter(BossSM fsm)
        {
            fsm.anim.ChangeAnim(BossAnim.Die);
            fsm.unit.Idle();
        }

        public override void process(BossSM fsm)
        {

        }

        public override void exit(BossSM fsm)
        {

        }
    }
}