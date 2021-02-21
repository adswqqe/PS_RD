using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EnemyKobold
{
    public class KoboldSM : MonoBehaviour
    {
        public static IdleState idleState = new IdleState();
        public ChaseState chaseState = new ChaseState();
        public FightState fightState = new FightState();
        public DamageState damageState = new DamageState();
        public static DieState dieState = new DieState();

        // 나중에 푸시다운 오토마타로 바꿀 필요가 생길때를 대비해서 리마인딩용으로 남긴 것
        //private BaseState[] _stateStack = new BaseState[10];
        private BaseState _curState;

        [HideInInspector]
        public KoboldAnimCtrl anim;
        [HideInInspector]
        public Kobold unit;
        public UnitBase targetUnit;

        public GameObject light;

        [SerializeField]
        private Vector3 _lookDir;
        [Header("Use for triangle search")]
        [SerializeField]
        private float _viewAngle;
        private float _angleCos;
        [SerializeField]
        private float _chaseDistance;
        [Header("Use for Circle search")]
        [SerializeField]
        private float _searchCircleRadius;
        [SerializeField]
        private float _searchCircleOffset;
        [Space(12)]
        [SerializeField]
        private float _attackDistance;
        [SerializeField]
        private float _attakcTick;


        private void Update()
        {
            _curState.process(this);
            unit.Progress();
        }

        private void Awake()
        {
            unit = GetComponent<Kobold>();
            anim = GetComponentInChildren<KoboldAnimCtrl>();
            Assert.IsNotNull(unit);
            Assert.IsNotNull(anim);
        }

        private void Start()
        {
            _curState = KoboldSM.idleState;
            ChangeState(idleState); 
            _angleCos = Mathf.Cos(_viewAngle / 360 * Mathf.PI);
        }

        public void ChangeState(BaseState state)
        {
            _curState.exit();
            _curState = state;
            _curState.enter(this);
        }

        public void DamageEvent()
        {
            ChangeState(damageState);
        }

        public void DieEvent()
        {
            ChangeState(KoboldSM.dieState);
        }

        public bool CheckView()
        {
            if (targetUnit == null)
                return false;

            Vector3 vectorDiff = targetUnit.transform.position - transform.position;
            Vector3 normalizeVec = vectorDiff.normalized;
            float sqrDistance = vectorDiff.sqrMagnitude;

            if ((Vector3.Dot(normalizeVec, _lookDir) >= _angleCos && sqrDistance <= _chaseDistance * _chaseDistance)
                || Vector3.Distance(targetUnit.transform.position, transform.position + _lookDir * _searchCircleOffset) <= _searchCircleRadius)
                return true;

            return false;
        }

        public bool CheckAttackable()
        {
            return (targetUnit.transform.position - transform.position).sqrMagnitude <= _attackDistance * _attackDistance;
        }

        public bool doesLookRight()
        {
            return _lookDir.x > 0;
        }

        public void SetDirection(bool isRight)
        {
            anim.SetDirection(isRight);
            if (isRight)
            {
                light.transform.localPosition = new Vector3(0.2f, 0.19f, 0f);
                light.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else
            {
                light.transform.localPosition = new Vector3(-0.2f, 0.19f, 0f);
                light.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = (_curState == chaseState ? Color.red : Color.blue);
            Gizmos.DrawWireSphere(transform.position + _lookDir * _searchCircleOffset, _searchCircleRadius);

            Debug.DrawLine(transform.position, transform.position + (Vector3)(Matrix4x4.Rotate(Quaternion.AngleAxis(_viewAngle / 360 * Mathf.PI, Vector3.forward)) * _lookDir * _chaseDistance), _curState == chaseState ? Color.red : Color.blue);
            Debug.DrawLine(transform.position, transform.position + (Vector3)(Matrix4x4.Rotate(Quaternion.AngleAxis(_viewAngle / 360 * Mathf.PI, Vector3.forward)) * _lookDir * _chaseDistance), _curState == chaseState ? Color.red : Color.blue);
            Debug.DrawLine(transform.position, transform.position + _lookDir * _attackDistance, _curState == fightState ? Color.red : Color.green);
        }
#endif
    }

    public abstract class BaseState
    {
        public abstract void enter(KoboldSM fsm);
        public abstract void process(KoboldSM fsm);
        public abstract void exit();
    }

    public class IdleState : BaseState
    {
        public override void enter(KoboldSM fsm)
        {
            fsm.anim.ChangeAnim(KoboldAnim.Idle);
            fsm.unit.Idle();
            Debug.Log("Kobold Idle State start");
        }

        public override void process(KoboldSM fsm)
        {
            if (fsm.CheckView())
            {
                fsm.ChangeState(fsm.chaseState);
            }
        }

        public override void exit()
        {
        }
    }

    public class ChaseState : BaseState
    {
        Vector3 _targetPosition;
        Vector3 _originalPosition;
        float _returnTime;
        bool _returning;

        public override void enter(KoboldSM fsm)
        {
            //fsm.anim.ChangeAnim(KoboldAnim.Run);
            _targetPosition = fsm.targetUnit.transform.position;
            _originalPosition = fsm.transform.position;
            _returnTime = 0;
            _returning = false;
            Debug.Log("Kobold Chase State start");
        }

        public override void process(KoboldSM fsm)
        {
            if (_returnTime > 0)
                fsm.anim.ChangeAnim(KoboldAnim.Idle);
            else
                fsm.anim.ChangeAnim(KoboldAnim.Run);

            fsm.SetDirection(_targetPosition.x > fsm.transform.position.x);

            if (fsm.CheckAttackable())
            {
                fsm.ChangeState(fsm.fightState);
                return;
            }

            bool checkVeiw = fsm.CheckView();
            fsm.light.SetActive(checkVeiw);
            if (checkVeiw)
            {
                fsm.light.SetActive(true);
                _targetPosition = fsm.targetUnit.transform.position;
                _returning = false;
                _returnTime = 0;
            }

            fsm.unit.Move(_targetPosition.x > fsm.transform.position.x ? 1 : -1);

            if (fsm.unit.NotMovable)
            {
                _returnTime += Time.deltaTime;
            }

            if(Mathf.Abs(_targetPosition.x - fsm.transform.position.x) <= 0.1)
            {
                if (_returning)
                {
                    fsm.ChangeState(KoboldSM.idleState);
                }
                else
                {
                    fsm.unit.Idle();
                    _returnTime += Time.deltaTime;
                }
            }

            if(_returnTime > 5)
            {
                _returnTime = 0;
                Debug.Log("Kobold returning start");
                _targetPosition = _originalPosition;
                _returning = true;
            }
        }

        public override void exit()
        {
        }
    }

    public class FightState : BaseState
    {
        float _attackDelay;
        public override void enter(KoboldSM fsm)
        {
            fsm.unit.Idle();
            fsm.anim.ChangeAnim(KoboldAnim.Attack);
            _attackDelay = 0;
            Debug.Log("Kobold Fight State start");
        }

        public override void process(KoboldSM fsm)
        {
            _attackDelay += Time.deltaTime;
            if (_attackDelay > 3)
            {
                fsm.anim.ChangeAnim(KoboldAnim.Attack);
                // 코볼트의 공격 시키는 유니티 애니메이션 이벤트를 받아서 처리합니다. KoboldAnimationEventListener 클래스를 참고해 주세요.
                //fsm.unit.CustomAttack(fsm.targetUnit);
                _attackDelay = 0;
            }

            fsm.SetDirection(fsm.targetUnit.transform.position.x > fsm.transform.position.x);

            if (!fsm.CheckAttackable())
                fsm.ChangeState(fsm.chaseState);
        }

        public override void exit()
        {
        }
    }

    public class DamageState : BaseState
    {
        float _stunTime;

        public override void enter(KoboldSM fsm)
        {
            fsm.anim.ChangeAnim(KoboldAnim.Hit1);
            //fsm.anim.ChangeAnim(KoboldAnim.Hit2);

            _stunTime = 0;
            Debug.Log("Kobold Damage State start");
        }

        public override void process(KoboldSM fsm)
        {
            _stunTime += Time.deltaTime;
            if(_stunTime >= 10)
            {
                fsm.ChangeState(KoboldSM.idleState);
            }
        }

        public override void exit()
        {
        }
    }

    public class DieState : BaseState
    {

        public override void enter(KoboldSM fsm)
        {
            fsm.anim.ChangeAnim(KoboldAnim.Destroy);
            Debug.Log("Kobold Die State start");
        }

        public override void process(KoboldSM fsm)
        {
        }

        public override void exit()
        {
        }
    }
}
