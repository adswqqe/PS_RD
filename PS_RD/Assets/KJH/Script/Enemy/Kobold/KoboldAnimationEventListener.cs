using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyKobold
{
    public class KoboldAnimationEventListener : MonoBehaviour
    {
        private KoboldSM _fsm;

    private void Awake()
        {
            _fsm = GetComponentInParent<KoboldSM>();
        }

        public void AnimEvent_Attack()
        {
            _fsm.unit.CustomAttack(_fsm.targetUnit);
        }

        public void AnimEvent_AttackEnd()
        {
            _fsm.anim.ChangeAnim(KoboldAnim.Idle);
        }
    }
}
