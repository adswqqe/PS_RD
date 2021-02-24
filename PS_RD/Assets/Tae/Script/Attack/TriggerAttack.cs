using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAttack : AttackBase
{
    BoxCollider2D boxCollider2D;
    float damage = 0.0f;
    [SerializeField]
    bool _testAlwaysAttackTrue;
    bool isEnter = false;
    bool isSkillEnter = false;

    private void OnEnable()
    {
        if (boxCollider2D == null)
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
        }

        Progress();
    }

    private void Progress()
    {
        AttackDamage();
        PlaySfx();

        if(isSkillEnter)
        {
            playShader();
            HitStop();
            CameraShake();
        }
        else if (isEnter)
        {
            BulltTime();
            Flash();
            HitStop();
            PlayFx();
        }

        gameObject.SetActive(false);
    }

    private void playShader()
    {

    }

    public override void AttackDamage()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.position, boxCollider2D.size, 0.0f, _hitLayerMask);
        isEnter = false;
        foreach (var item in collider2Ds)
        {
            if (item.gameObject.name.Contains("Player"))
                continue;

            if(item.gameObject.tag == "Skill")
            {
                Debug.Log("asdasdasd");
                // Skill1 
                item.GetComponent<Skill1Ctrl>().SetSkill1Ex();
            }

            if (item.CompareTag("Enemy") /*&& item.GetComponent<UnitBase>().Status.isAlive*/)
            {
                isEnter = true;

                //크리티컬 확률 계산
                float criticalRoulette = Random.Range(0, 100);
                bool isCritical = (_criticalPercentage > criticalRoulette) ? true : false;

                if (isCritical)
                {
                    item.GetComponent<UnitBase>().Hit(damage * 1.5f);
                }
                else
                {
                    item.GetComponent<UnitBase>().Hit(damage);
                }
            }
        }
    }

    public override void BulltTime()
    {
        if (_isAbleBulltTime == false)
            return;

        UnityTimeManager.instance.BulletTime(_bulltTime, _bulletTimeScale);
    }

    public override void Flash()
    {
        if (_isAbleFlash == false)
            return;

        _flashGO.gameObject.SetActive(true);
        _flashGO.SetFlash(_flashTime);
    }

    public override void HitStop()
    {
        if (_isAbleHitStop == false)
            return;

        UnityTimeManager.instance.HitStop(_hitStopTime);
    }

    public override void PlayFx()
    {
        if (_isDisplyFx == false)
            return;
    }

    public override void PlaySfx()
    {

    }

    public override void CameraShake()
    {
        CinemachineManager.Instance.ShakeCamera(_cameraShakePower, _camerShakeTime);
    }

    public override void SetDamage(float min, float max)
    {
        _minAttackDamage = min;
        _maxAttackDamage = max;

        damage = Random.Range(_minAttackDamage, _maxAttackDamage);
    }
}
