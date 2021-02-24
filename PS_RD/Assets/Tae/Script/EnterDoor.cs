using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnterDoor : MonoBehaviour
{
    public CinemachineVirtualCamera curVirtualCamera;
    public CinemachineVirtualCamera nextVirtualCamera;
    public GameObject prevWall;
    public Material runeMT;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            curVirtualCamera.Priority = 0;
            nextVirtualCamera.Priority = 1;
            Debug.Log("asdasdasd");
            runeMT.SetColor("Color_79988DDE", new Color32(191, 84, 0, 255));
            prevWall.SetActive(true);
            GetComponent<BoxCollider2D>().enabled = false;
            //gameObject.SetActive(false);
        }
    }
}
