using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineManager : MonoBehaviour
{
    public static CinemachineManager Instance { get; private set; }
    public PolygonCollider2D allGroundCollider;
    public CinemachineVirtualCamera[] _cinemachineVirtualCamera;
    public CinemachineBrain cinemachineBrain;
    private CinemachineConfiner _cinemachineConfiner;
    public float _shakeTimer;
    public int cameraIndex;
    private float _shakeTimerTotal;
    private float _startingIntensity;
    public bool isCourutin = false;

    public void SetCameraIndex(int index)
    {
        Debug.Log("isCu " + isCourutin);
        cameraIndex = index;
    }


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        //_cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cinemachineConfiner = GetComponent<CinemachineConfiner>();
        cameraIndex = 0;
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            _cinemachineVirtualCamera[cameraIndex].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();


        _startingIntensity = intensity;
        _shakeTimerTotal = time;
        _shakeTimer = time;
        if (!isCourutin)
            StartCoroutine(ShakeTick());
    }

    IEnumerator ShakeTick()
    {
        isCourutin = true;
        bool _isShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                        _cinemachineVirtualCamera[cameraIndex].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        while (_isShake)
        {
            if (_shakeTimer >= 0.0f)
            {
                Debug.Log(_shakeTimer);
                _shakeTimer -= 0.02f;
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
                          Mathf.Lerp(_startingIntensity, 0f, _shakeTimer / _shakeTimerTotal);
                if (_shakeTimer <= 0.0f)
                {
                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
                    _isShake = false;
                }
                //Debug.Log(Mathf.Lerp(_startingIntensity, 0f, _shakeTimer / _shakeTimerTotal));
            }
            yield return new WaitForSecondsRealtime(0.02f);
        }
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        isCourutin = false;
    }

    public void SetAllGroundCollider()
    {
        _cinemachineConfiner.m_BoundingShape2D = allGroundCollider;
        _cinemachineConfiner.InvalidatePathCache();
    }

    public void SetGroundCollider(PolygonCollider2D polygonCollider2D)
    {
        _cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;
        _cinemachineConfiner.InvalidatePathCache();
    }

    public void SetPriority(int priority)
    {
        _cinemachineVirtualCamera[0].Priority = priority;
    }

    public void SetMainpriority()
    {
        _cinemachineVirtualCamera[0].Priority = 100;
    }
}
