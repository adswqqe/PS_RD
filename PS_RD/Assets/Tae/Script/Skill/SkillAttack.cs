using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttack : AttackBase
{
    BoxCollider2D boxCollider2D;
    float damage = 0.0f;
    [SerializeField]
    bool _testAlwaysAttackTrue;
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
        BulltTime();
        Flash();
        HitStop();
        PlayFx();

        Debug.Log(gameObject.name);
        gameObject.SetActive(false);
    }

    public override void AttackDamage()
    {
    }

    public override void BulltTime()
    {
    }

    public override void Flash()
    {
    }

    public override void HitStop()
    {
    }

    public override void PlayFx()
    {
    }

    public override void PlaySfx()
    {
    }

    public override void SetDamage(float min, float max)
    {
    }
    IEnumerator BloodStateAbnormality(Unit unit)
    {
        yield return new WaitForSeconds(1.5f);
    }
}
