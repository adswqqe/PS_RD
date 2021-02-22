using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyBoss
{
    public class BossAnimationEventListener : MonoBehaviour
    {
        private BossSM _bossSM;

        private void Awake()
        {
            _bossSM = GetComponentInParent<BossSM>();
        }

        public void AnimEvent_Attack()
        {
            if (_bossSM)
                _bossSM.IsAttacked = true;
        }

        public void AnimEvent_AttackEnd()
        {
            if (_bossSM)
                _bossSM.IsAttackEnd = true;
        }
    }
}
