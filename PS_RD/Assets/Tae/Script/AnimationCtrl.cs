using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AniState
{
    Idle = 0,
    Move,
    Attack,
    Jump,
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

    }
}
