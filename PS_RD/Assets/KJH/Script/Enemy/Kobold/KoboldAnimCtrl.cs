using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyKobold
{
    public enum KoboldAnim
    {
        Idle = 0,
        Run,
        Attack,
        Hit1,
        Hit2,
        Destroy,
    }

    public class KoboldAnimCtrl : AnimCtrl
    {
        public override void Awake()
        {
            base.Awake();
            base.SetAnimName(new string[6] { "Idle", "Walk", "Attack", "Hit1", "Hit2", "Die" });
        }

        public void ChangeAnim(KoboldAnim anim)
        {
            base.ChangeAnim((int)anim);
        }
    }
}
