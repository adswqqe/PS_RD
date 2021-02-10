using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AniState
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
    Attack2,
    Attack3,
    JumpAttack,
    Skll1,
}

public enum ShadowAniState // 50~
{
    OutOfControl = 50,
    ControlRecovery,
    Destroy,
    Resurrection,
    Skill1,
}

public class AnimationCtrl : MonoBehaviour
{
    Animator _animator = null;
    public Animator Animator => _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void PlayAni(AniState aniState)
    {
        Animator.speed = TimeManager.GetTimeScale;
        Animator.SetInteger("State", (int)aniState);
    }

    public void PlayAni(ShadowAniState aniState)
    {
        Animator.SetInteger("State", (int)aniState);
    }

}
