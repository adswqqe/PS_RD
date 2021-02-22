using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyKobold
{
    public class KoboldAnimationEventListener : MonoBehaviour
    {
        private Kobold _kobold;
        private KoboldAnimCtrl _anim;

    private void Awake()
        {
            _kobold = GetComponentInParent<Kobold>();
            _anim = GetComponentInParent<KoboldAnimCtrl>();
        }

        public void AnimEvent_Attack()
        {
            if (_kobold)
                _kobold.Attack();
        }

        public void AnimEvent_AttackEnd()
        {
            if (_anim)
                _anim.ChangeAnim((int)KoboldAnim.Idle);
        }
    }
}
