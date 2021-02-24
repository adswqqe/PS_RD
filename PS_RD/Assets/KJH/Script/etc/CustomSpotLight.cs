using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.Rendering.Universal;

public class CustomSpotLight : MonoBehaviour
{
    private Light2D light;

    public LayerMask layerMask;
    private PlayerShadowUnit shadowUnit;
    bool isFrist = true;
    bool isHit = false;
    RaycastHit2D hit;
    private void Awake()
    {
        light = GetComponent<Light2D>();
        Assert.IsNotNull(light);
    }

    private void Update()
    {
        if (transform.rotation.z >= 0)
             hit = Physics2D.Raycast(transform.position, Vector3.left, 5.0f, layerMask);
        else
            hit = Physics2D.Raycast(transform.position, Vector3.right, 5.0f, layerMask);


        if (hit.collider != null)
        {
            if(hit.collider.tag == "Shadow")
            {
                isHit = true;
                Debug.Log(hit.collider.name);
                if (isFrist)
                {
                    shadowUnit = hit.collider.GetComponent<PlayerShadowUnit>();
                    shadowUnit.LightDetection();
                    isFrist = false;
                }
                //if(InLight(hit.collider.GetComponent<Transform>().position))
                //{
                //}
            }
        }

        if(hit.collider == null)
        {
            if (shadowUnit == null)
                return;

            isFrist = true;
            shadowUnit.Skill1End();
            shadowUnit = null;
        }
    }

    public bool InLight(Vector3 pos)
    {
        float radian = transform.rotation.eulerAngles.z * Mathf.PI / 180.0f;
        Vector3 lookDir = new Vector3(Mathf.Sin(radian),Mathf.Cos(radian));
        Vector3 Dif = (pos - transform.position);
        float length = light.pointLightOuterRadius;
        float angle = light.pointLightOuterAngle;

        return (Vector3.Dot(Dif.normalized, lookDir) >= Mathf.Cos(angle * Mathf.PI / 360.0f) && Dif.magnitude <= length * length);
    }
}
