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

    public class KoboldAnimCtrl : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _spriteRndr;
        private int _curAnimIndex;

        private string[] _animName = new string[6] { "Idle", "Walk", "Attack", "Hit1", "Hit2", "Die" };

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _spriteRndr = GetComponentInChildren<SpriteRenderer>();
            _curAnimIndex = 0;
        }

        private void Update()
        {
            if(_animator.GetCurrentAnimatorStateInfo(0).IsName(_animName[_curAnimIndex]))
                _animator.SetInteger("AnimIndex", -1);
            else
                _animator.SetInteger("AnimIndex", _curAnimIndex);
        }

        public void ChangeAnim(KoboldAnim anim)
        {
            int animIndex = (int)anim;
            if (_curAnimIndex != animIndex)
                _curAnimIndex = animIndex;

            _animator.speed = Time.timeScale;
        }

        public void SetDirection(bool isRight)
        {
            if(isRight)
            {
                _spriteRndr.flipX = true;
            }
            else
            {
                _spriteRndr.flipX = false;
            }
        }
    }
}
