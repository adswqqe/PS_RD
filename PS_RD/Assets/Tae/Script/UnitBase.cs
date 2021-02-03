using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    private AnimationCtrl _aniCtrl;
    public AnimationCtrl AniCtrl => _aniCtrl;

    private CustomFSMSystem _fsmSystem;
    public CustomFSMSystem FsmSystem => _fsmSystem;

    public abstract void Progress();

    private bool _isInit = false;
    public bool IsInit => _isInit;

    private void Awake()
    {
        InitUnit();
    }

    private void InitUnit()
    {
        _aniCtrl = GetComponentInChildren<AnimationCtrl>();
        _fsmSystem = GetComponentInChildren<CustomFSMSystem>();

        _isInit = true;
    }

    public abstract void Idle();

    public abstract void Move(float deltaX);
    
    public abstract void Attack();

    public abstract void Jump(float height);

    public abstract void Dash();
}
