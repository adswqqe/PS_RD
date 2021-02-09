using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityTimeManager : MonoBehaviour
{
    public static UnityTimeManager instance;

    private float _timer = 0.0f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HitStop(2);
            CinemachineManager.Instance.ShakeCamera(0.5f, 1);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            BulletTime(2, 0.3f);
            CinemachineManager.Instance.ShakeCamera(0.5f, 1);
        }
    }

    public void HitStop(float time)
    {
        _timer = time;
        StartCoroutine(HitStopTimer());
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void BulletTime(float time, float timeScale)
    {
        _timer = time;
        StartCoroutine(HitStopTimer());
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    IEnumerator HitStopTimer()
    {
        yield return new WaitForSecondsRealtime(_timer);

        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
