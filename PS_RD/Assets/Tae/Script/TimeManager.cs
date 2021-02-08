using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static float _deltaTime = 0.0f;
    private static float _timeScale = 1.0f;
    private static float _fixedDeltaTime = 0.0f;
    private static float _scaleTimer = 0.0f;

    public static bool isTest = false;

    public static float SetDeltaTime(float value) { _deltaTime = value; return deltaTime; }
    public static float deltaTime { get { return _deltaTime * _timeScale; } }

    public static float SetFixedDeltaTime(float value) { _fixedDeltaTime = value; return fixedDeltaTime; }

    public static float fixedDeltaTime { get { return _fixedDeltaTime * _timeScale; } }

    public static float GetTimeScale { get { return _timeScale; } }

    public static void SetTimeScale(float value)
    {
        if (_scaleTimer != 0)
            _scaleTimer = 0;

        _timeScale = value;
    }

    public static void HitStop(float time)
    {
        _scaleTimer = time;
        _timeScale = 0;
        isTest = true;
    }

    public static void BulletTime(float time, float timeScale)
    {
        _scaleTimer = time;
        _timeScale = timeScale;
        isTest = true;
    }

    private void Update()
    {
        SetDeltaTime(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            HitStop(1.0f);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            BulletTime(5.0f, 0.1f);

        if (_scaleTimer > 0)
        {
            _scaleTimer -= Time.deltaTime;
        }
        else
        {
            SetTimeScale(1);
            isTest = false;
        }
    }

    // StartCoroutine이 static이 아니라 실행 불가.. 어떻게 처리 하는게 제일 좋을까?
    IEnumerator ScaleTimerCoroutine()
    {
        if (_scaleTimer <= 0)
            Debug.LogError("Sclae Timer가 0임");

        while (_scaleTimer >= 0)
        {
            _scaleTimer -= Time.deltaTime;
            yield return null;
        }

        _timeScale = 1;
    }
}
