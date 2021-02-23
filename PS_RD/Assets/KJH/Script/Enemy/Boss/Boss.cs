using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boss : UnitBase
{
    // components
    private Transform _tr;
    private Rigidbody2D _rigid;
    public BoxCollider2D hitBox;
    public BoxCollider2D[] attackBox;
    private int _curAttackBox = 0;
    public BoxCollider2D groundCheckCollider;
    public LayerMask groundLayer;

    public MonsterStat stat;

    private float _horizonalSpeed;
    public ContactFilter2D filter2D;


    [SerializeField]
    private UnitBase _target;
    public UnitBase Target
    { get { return _target; } set { _target = value; } }

    public UnityEvent damageEvent;
    public UnityEvent dieEvent;

    private void Start()
    {
        // component allocation
        _tr = transform;
        _rigid = GetComponentInChildren<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigid.velocity = new Vector2(_horizonalSpeed, _rigid.velocity.y);
    }
    public override void Progress()
    {

    }
    
    // 넉백에 관한것이 상정되어 있지 않음
    public override void Attack()
    {
        if (attackBox[_curAttackBox].IsTouchingLayers(_target.gameObject.layer))
        {
            List<Collider2D> colliders = new List<Collider2D>();
            attackBox[_curAttackBox].OverlapCollider(filter2D, colliders);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject == this.gameObject)
                    continue;
                UnitBase unitBase = collider.gameObject.GetComponent<UnitBase>();
                if (unitBase)
                {
                    unitBase.Hit(20);
                }
            }
        }
    }

    public override void Dash()
    {

    }

    public override void Hit(float damage)
    {
        stat.nowHP -= damage;

        if (stat.nowHP > 0)
        {
            damageEvent.Invoke();
        }
        else
        {
            dieEvent.Invoke();
        }
    }

    public override void Idle()
    {
        _horizonalSpeed = 0;
    }

    public override void Jump(float height)
    {
        if (CheckGround())
            _rigid.velocity = new Vector2(_rigid.velocity.x, Mathf.Sqrt(-Physics2D.gravity.y * 2 * height));
    }

    public void Jump()
    {
        Jump(stat.jumpPower);
    }

    public override void JumpAttack()
    {
        _rigid.velocity = new Vector2(0,Physics2D.gravity.y);
    }

    public override void Move(float deltaX)
    {
        _horizonalSpeed = stat.moveSpeed * deltaX;
    }

    public void SetAttackBox(int index)
    {
        _curAttackBox = index;
    }

    public void SetDirection(bool isRight)
    {
        if (isRight)
        {

        }
        else
        {

        }
    }

    public bool CheckGround()
    {
        return groundCheckCollider.IsTouchingLayers(groundLayer);
    }

}
