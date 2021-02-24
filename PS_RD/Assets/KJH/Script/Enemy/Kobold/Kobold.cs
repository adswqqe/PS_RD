using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// this script will be only charge in in-game logic

public class Kobold : UnitBase
{
    // components
    private Transform _tr;
    private Rigidbody2D _rigid;
    public BoxCollider2D hitBox;
    public BoxCollider2D groundCheckCollider;
    public BoxCollider2D[] nextGroundCheckCollider;
    public LayerMask groundLayer;

    public MonsterStat stat;
    public GameObject bulletPrefab;
    public Transform bulletPos;

    //public float accelation;
    //public float maxSpeed;
    [SerializeField]
    private float _horizonalSpeed;
    public bool NotMovable
    {
        get{ return !CheckMovable(_horizonalSpeed > 0); }
    }

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
        if (!CheckMovable(_horizonalSpeed > 0))
            _horizonalSpeed = 0;
    }

    public override void Idle()
    {
        _horizonalSpeed = 0;
    }

    public override void Move(float deltaX)
    {
        _horizonalSpeed = stat.moveSpeed * deltaX;
    }

    public override void Attack()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab);
        if (bullet)
        {
            bullet.GetComponent<KoboldProjectile>().Initiate(bulletPos.position, new Vector3(_target.transform.position.x, bulletPos.position.y), this);
        }
    }

    public override void Jump(float height)
    {
        // there's no Jump in Kobold
    }
    public override void Dash()
    {
        // there's no dash in kobold
    }
    public override void JumpAttack()
    {
        // there's no jump attack in kobold
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
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        gameObject.SetActive(false);
    }

    private bool CheckMovable(bool isRight)
    {
        if (isRight)
        {
            return nextGroundCheckCollider[0].IsTouchingLayers(groundLayer) || !nextGroundCheckCollider[2].IsTouchingLayers(groundLayer);
        }
        else
        { 
            return nextGroundCheckCollider[1].IsTouchingLayers(groundLayer) || !nextGroundCheckCollider[3].IsTouchingLayers(groundLayer);
        }
    }

    public void SetDirection(bool isRight)
    {

    }
}

