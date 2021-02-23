using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestPurposeUI : MonoBehaviour
{
    public UnitBase unit;

    public void Jump()
    {
        unit.Jump(5);
    }

    public void Idle()
    {
        unit.Idle();
    }

    public void moveRight()
    {
        unit.Move(1);
    }

    public void moveLeft()
    {
        unit.Move(-1);
    }
}
