using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyBoss
{
    public enum BossAnim
    {
        Idle = 0,
        Die,
        DownPunch,
        Groggy,
        LeftPunch,
        RightPunch,
        SeigeMode,
        Tornado,
        Uppercut,
        Walk,
    }

    public class BossAnimCtrl : AnimCtrl
    {
        public override void Awake()
        {
            base.Awake();
            base.SetAnimName(new string[10] { "Idle", "Die", "DownPunch", "Groggy", "LPunch", "RPunch", "SeigeMod", "Tornado", "Uppercut", "Walk" });
        }

        public void ChangeAnim(BossAnim anim)
        {
            base.ChangeAnim((int)anim);
        }
    }
}
