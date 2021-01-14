using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : UnitBase
{
    // 스탯 관련 더미 변수
    float _speed = 10.0f;

    // 이동 관련 변수
    float _walkAcceleration = 75;
    float _airAcceleration = 30;
    float _groundDeceleration = 70;
    float _acceleration = 0;
    float _deceleration = 0;
    Vector2 _velocity;

    // 상태 판단
    public bool _isGround = true;

    // Compoenent
    Rigidbody2D _rigid2D;

    private void Start()
    {
        _rigid2D = GetComponent<Rigidbody2D>();
        Init();
    }

    private void Init()
    {
        FsmSystem.SetUnit(this);
        _velocity = Vector2.zero;
    }

    public override void Attack()
    {

    }

    public override void Idle()
    {
        
    }

    public override void Jump()
    {

    }

    public override void Move(float deltaX)
    {
        if (deltaX != 0)
            _velocity = new Vector2(Mathf.MoveTowards(_velocity.x, _speed * deltaX, _acceleration * Time.deltaTime), _rigid2D.velocity.y);
        else
            _velocity = new Vector2(Mathf.MoveTowards(_velocity.x, 0, _deceleration * Time.deltaTime), _rigid2D.velocity.y);
    }

    public override void Progress()
    {
        _acceleration = _isGround ? _walkAcceleration : _airAcceleration;
        _deceleration = _isGround ? _groundDeceleration : 0;

        _rigid2D.velocity = _velocity;
    }

    public void StopMove()
    {
        if (_isStopMoveCoroutineRunning)
            StopCoroutine(_StopMoveCoroutine);

        _StopMoveCoroutine = StartCoroutine(StopMoveCoroutine());

    }
    
    public Coroutine _StopMoveCoroutine;
    private bool _isStopMoveCoroutineRunning = false;

    public IEnumerator StopMoveCoroutine()
    {
        _isStopMoveCoroutineRunning = true;
        while (Mathf.Abs(_velocity.x) >= 0.01f)
        {
            Debug.Log("asdasdasd");
            _velocity = new Vector2(Mathf.MoveTowards(_velocity.x, 0, _deceleration * Time.deltaTime), _rigid2D.velocity.y);
            yield return null;
        }
        _isStopMoveCoroutineRunning = false;
        Debug.Log("End Corutin");
    }
}
