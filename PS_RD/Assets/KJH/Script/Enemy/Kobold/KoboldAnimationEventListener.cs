using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyKobold
{
    public class KoboldAnimationEventListener : MonoBehaviour
    {
        private KoboldSM _koboldSM;
        private KoboldAnimCtrl _anim;

    private void Awake()
        {
            _koboldSM = GetComponentInParent<KoboldSM>();
            _anim = GetComponentInParent<KoboldAnimCtrl>();
        }

        public void AnimEvent_Attack()
        {
            if (_koboldSM)
                _koboldSM.IsAttacked = true;
        }

        public void AnimEvent_AttackEnd()
        {
            if (_anim)
                _anim.ChangeAnim((int)KoboldAnim.Idle);
        }
    }
}
