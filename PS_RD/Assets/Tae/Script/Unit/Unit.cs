using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Unit : UnitBase
{
    // 스탯 관련 더미 변수
    float _maxSpeed = 10.0f;
    float _speed = 10.0f;
    float _maxJumpHeight = 1.5f;
    float _defaultJumpHeight = 1.5f;
    float _curHp = 0;
    float _maxHp = 1000;
    float _invincibilityTime = 1.0f;

    // 이동 관련 변수
    float _walkAcceleration = 75;
    float _airAcceleration = 30;
    float _groundDeceleration = 70;
    float _acceleration = 0;
    float _deceleration = 0;
    Vector2 _velocity;
    float _facingDir = 1;
    float _inputDir = 1;
    bool _isFacingRight = true;

    // 점프 관련 변수
    private float _coyoteTime = 0.2f;
    public float CoyoteTime => _coyoteTime;

    // 공격 관련 변수
    [HideInInspector]
    public int curAttackIndex = 0;
    public AttackBase[] basicAttacks;
    public AttackBase basicJumpAttack;

    // 상태 판단
    private bool _isGround = true;
    public bool IsGround => _isGround;
    private bool _isInvincibility = false;
    private bool _isStateAbnormality = false;

    public AniState CurAniState;

    // Compoenent
    Rigidbody2D _rigid2D;
    public Transform groundCheck;    // Ground체크를 위함
    public Vector2 groundCheckBoxSize;
    public LayerMask groundLayer;
    public PlayerShadowUnit playerShadowUnit;
    public SpriteRenderer _spriteRenderer;

    // Events
    public event UnityAction OnDamageAction;
    public event UnityAction OnSkill1TransformAniEvent;
    public event UnityAction OnHitAniEndAction;

    // Prefabs
    public GameObject skill1GO;

    // skill
    public bool isSkill1CollTimeOk = true;
    private float _skill1CoolTime = 5.0f;

    private void Start()
    {
        Init();
    }

    protected void Init()
    {
        //FsmSystem.SetUnit(this);
        _velocity = Vector2.zero;
        _rigid2D = GetComponent<Rigidbody2D>();
        _curHp = _maxHp;
    }

    public override void Attack()
    {
        basicAttacks[curAttackIndex].SetDamage(0, 10);
        basicAttacks[curAttackIndex].gameObject.SetActive(true);
    }

    public override void Idle()
    {
    }

    public void SetAttackSpeed()
    {
        if (_isStateAbnormality == false)
            _speed = 2;

        _velocity = Vector2.zero;
    }

    public void SetMoveSpeed()  // 나중에는 SetPlayerMoveSpeed 이런식으로 해놓고
                                // PlayerDataCenter 이런 클래스를 통해 데이터를 받아와서 스피드를 조절하자.
    {
        if (_isStateAbnormality == false)
            _speed = 10;
    }


    public override void Dash()
    {
        _rigid2D.velocity = new Vector2(_velocity.x + 10, _velocity.y) * Time.timeScale;
    }

    public override void Jump(float height)
    {
        _isGround = false;
        _rigid2D.velocity = new Vector2(_velocity.x, Mathf.Sqrt(-Physics2D.gravity.y * 2 * (height))) * Time.timeScale;
    }

    public void AddJumpGravity()
    {
        _velocity = new Vector2(_velocity.x, _velocity.y - 0.02f);
    }

    public override void JumpAttack()
    {
        basicJumpAttack.gameObject.SetActive(true);
    }

    public override void Move(float deltaX)
    {
        _inputDir = deltaX;

        if (deltaX != 0)
            _velocity = new Vector2(Mathf.MoveTowards(_velocity.x, _speed * deltaX, _acceleration * Time.deltaTime), _rigid2D.velocity.y);
        else
            _velocity = new Vector2(Mathf.MoveTowards(_velocity.x, 0, _deceleration * Time.deltaTime), _rigid2D.velocity.y);
    }

    public override void Hit(float damage)
    {
        if (_isInvincibility == true)
            return;

        _curHp -= damage;

        if(_curHp <= 0) // 사망
        {

        }
        else
        {
            StartCoroutine(HitTime());
        }

        OnDamageAction?.Invoke();
    }

    public void HitKnockBack()
    {
        _rigid2D.MovePosition(new Vector2(transform.position.x - (0.5f * _facingDir), transform.position.y + 0.5f));
    }

    public void HitAniEnd()
    {
        OnHitAniEndAction?.Invoke();
    }

    public void Skill1()
    {
        playerShadowUnit.Skill1();
        var temp = Instantiate(skill1GO, new Vector3(transform.position.x + (0.5f * _facingDir), transform.position.y, transform.position.z), Quaternion.identity);
        temp.GetComponent<Skill1Ctrl>().Init(transform, _facingDir, playerShadowUnit);
        StartCoroutine(Skill1Tick());
    }

    public void Skill1AniEvent()
    {
        OnSkill1TransformAniEvent?.Invoke();
    }

    public override void Progress()
    {
        CheckGround();

        _acceleration = _isGround ? _walkAcceleration : _airAcceleration;
        _deceleration = _isGround ? _groundDeceleration : 0;

        _rigid2D.velocity = new Vector2(_velocity.x, _rigid2D.velocity.y);

        AniCtrl.PlayAni(CurAniState);
    }

    public void CheckMovementDir()
    {
        if (_isFacingRight && _inputDir < 0)
        {
            Flip();
        }
        else if (!_isFacingRight && _inputDir > 0)
        {
            Flip();
        }
    }

    void Flip()
    {
        _facingDir *= -1;
        _isFacingRight = !_isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);   // y값 돌리는 숫자를 curANi 머 선형보간 등 애니메이션을 주면 빙글도는 이쁜 애니메이션~
    }

    void CheckGround()
    {
        bool lastIsGround = _isGround;
        _isGround = Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0, groundLayer);

        if(_isGround == false)
        {
            _coyoteTime -= Time.deltaTime;
        }
        else if (_isGround == true)
        {
            _coyoteTime = 0.2f;
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null && groundCheckBoxSize != null)
            Gizmos.DrawWireCube(groundCheck.position, groundCheckBoxSize);
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
            _velocity = new Vector2(Mathf.MoveTowards(_velocity.x, 0, _deceleration * Time.deltaTime), _rigid2D.velocity.y);
            yield return null;
        }
        Debug.Log("Velocity : " + _velocity);
        _isStopMoveCoroutineRunning = false;
    }

    IEnumerator HitTime()
    {
        _isInvincibility = true;
        float Totaltick = 0;
        float tick = 0;
        int count = 0;

        _spriteRenderer.color = new Color32(70, 70, 70, 255);


        while (true)
        {
            Totaltick += Time.deltaTime;
            tick += Time.deltaTime;

            if (Totaltick >= _invincibilityTime * 0.7f)
            {
                if (tick >= 0.05f)
                {
                    count++;

                    if (count % 2 == 0)
                    {
                        _spriteRenderer.color = new Color32(70, 70, 70, 255);
                    }
                    else
                    {
                        _spriteRenderer.color = new Color32(255, 255, 255, 255);
                        //Debug.Log("asdasdsadasd");
                    }

                    tick = 0;
                }
            }


            if (Totaltick >= _invincibilityTime)
                break;

            yield return null;
        }

        //yield return new WaitForSeconds(Status.hitTime);

        _isInvincibility = false;
        _spriteRenderer.color = new Color32(255, 255, 255, 255);
    }

    public void SlowStateStart()
    {
        StartCoroutine(SlowStateAbnormality());
    }

    IEnumerator SlowStateAbnormality()
    {
        _isStateAbnormality = true;
        _speed = _speed * 0.1f;

        yield return new WaitForSeconds(1.5f);

        _speed = _maxSpeed;
        _isStateAbnormality = false;
    }

    IEnumerator Skill1Tick()
    {
        isSkill1CollTimeOk = false;

        yield return new WaitForSeconds(_skill1CoolTime);

        isSkill1CollTimeOk = true;

    }
}
