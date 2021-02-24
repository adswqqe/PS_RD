using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.Rendering.Universal;

public class CustomSpotLight : MonoBehaviour
{
    private Light2D light;

    private void Awake()
    {
        light = GetComponent<Light2D>();
        Assert.IsNotNull(light);
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
