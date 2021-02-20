using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoboldProjectile : MonoBehaviour
{
    private Kobold _bulletOwner;
    private Vector3 _targetPos;
    private Vector3 _originPos;
    [SerializeField]
    private float _bulletLength;
    [SerializeField]
    private float _speed;
    private BoxCollider2D _collider;
    private Vector3 _bulletDirection;
    private float _moveLength;

    private float _timeCount;
    public LayerMask targetLayer;
    public ContactFilter2D contactFilter;

    private bool _isAttacked;

    public void Initiate(Vector3 originPos, Vector3 targetPos, Kobold owner)
    {
        _originPos = originPos;
        _targetPos = targetPos;
        _bulletOwner = owner;
        _moveLength = 0.0f;

        _bulletDirection = (targetPos - originPos).normalized;

        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(_bulletDirection.y, _bulletDirection.x) * Mathf.Rad2Deg, Vector3.forward);

        _timeCount = 0;
        _isAttacked = false;
    }

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        _moveLength += _speed * Time.deltaTime;

        transform.position = _originPos + _bulletDirection * _moveLength;

        AttackCheck();

        if (_moveLength > _bulletLength || _isAttacked)
            HandleDeath();
    }

    public void AttackCheck()
    {
        if (!_isAttacked && _collider.IsTouchingLayers(targetLayer))
        {
            List<Collider2D> colliders = new List<Collider2D>();
            _collider.OverlapCollider(contactFilter, colliders);
            foreach (var collider in colliders)
            {
                if (collider.gameObject == _bulletOwner.gameObject)
                    continue;
                UnitBase unitBase = collider.gameObject.GetComponent<UnitBase>();
                if (unitBase)
                {
                    unitBase.Hit(Random.Range(_bulletOwner.stat.minAtk, _bulletOwner.stat.maxAtk));
                    _isAttacked = true;

                    Debug.Log("bullet collision occured");
                }
            }
        }
    }

    public void HandleDeath()
    {
        //this.gameObject.SetActive(false);
        DestroyImmediate(this.gameObject);
    }
}
