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

    public override void Attack()
    {
        if (hitBox.IsTouchingLayers(_target.gameObject.layer))
        {
            List<Collider2D> colliders = new List<Collider2D>();
            hitBox.OverlapCollider(filter2D, colliders);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject == this.gameObject)
                    continue;
                UnitBase unitBase = collider.gameObject.GetComponent<UnitBase>();
                if (unitBase)
                {
                    unitBase.Hit(Random.Range(stat.minAtk, stat.maxAtk));
                }
            }
        }
    }

    public override void Dash()
    {

    }

    public override void Hit(float damage)
    {

    }

    public override void Idle()
    {
        _horizonalSpeed = 0;
    }

    public override void Jump(float height)
    {

    }

    public override void JumpAttack()
    {

    }

    public override void Move(float deltaX)
    {
        _horizonalSpeed = deltaX;
    }

    public override void Progress()
    {

    }
}
