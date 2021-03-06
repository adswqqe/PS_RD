﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCtrl : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _spriteRndr;
    private int _curAnimIndex;

    private string[] _animName;
    public void SetAnimName(string[] names)
    {
        _animName = names;
    }

    public virtual void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _spriteRndr = GetComponentInChildren<SpriteRenderer>();
        _curAnimIndex = 0;
    }

    private void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animName[_curAnimIndex]))
            _animator.SetInteger("AnimIndex", -1);
        else
            _animator.SetInteger("AnimIndex", _curAnimIndex);
    }

    public void ChangeAnim(int animIndex)
    {
        if (_curAnimIndex != animIndex)
            _curAnimIndex = animIndex;

        _animator.speed = Time.timeScale;
    }

    public void SetSpeed(float speed)
    {
        _animator.speed = speed;
    }

    public void SetDirection(bool isRight)
    {
        if (isRight)
        {
            _spriteRndr.flipX = true;
        }
        else
        {
            _spriteRndr.flipX = false;
        }
    }
}
