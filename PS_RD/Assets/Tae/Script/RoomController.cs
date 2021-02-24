using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public List<UnitBase> curRoomUnit;
    public List<UnitBase> nextRoomUnit;
    public GameObject roomDoor;
    public GameObject nextDoor;
    public Material runeMT;


    [ReadOnly, SerializeField]
    bool _isRoomClear = false;

    private void Start()
    {
        foreach (var u in nextRoomUnit)
        {
            u.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (_isRoomClear)
            RoomClear();

        foreach (var u in curRoomUnit)
        {
            if (u.gameObject.activeSelf)
                return;
        }

        _isRoomClear = true;
    }

    private void RoomClear()
    {

        foreach (var u in nextRoomUnit)
        {
            u.gameObject.SetActive(true);
        }

        runeMT.SetColor("Color_79988DDE", new Color(0, 0.68f, 0.191f));
        roomDoor.SetActive(false);
        if (nextDoor != null)
        nextDoor.SetActive(true);
        //this.gameObject.SetActive(false);
        this.enabled = false;
    }
}
