using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraUtilities : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private float shakeTimer;
    private float shakeTotalTime;
    private float startingIntensity;

    [HideInInspector]public float defaultFOV;

    private void Start()
    {
        virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
        defaultFOV = virtualCamera.m_Lens.FieldOfView;
        shakeTimer = 0f;
        shakeTotalTime = 0f;
        startingIntensity = 0f;
    }

    public void ChangeFov(int _reachedFOV, float _duration)
    {
        StartCoroutine(ChangeFovCoroutine(_reachedFOV, _duration));
    }

    private IEnumerator ChangeFovCoroutine(int _reachedFOV, float _duration)
    {
        float elapsedTime = 0f;
        float initialFOV = virtualCamera.m_Lens.FieldOfView;

        while (elapsedTime < _duration)
        {
            float currentFOV = Mathf.Lerp(initialFOV, _reachedFOV, elapsedTime / _duration);
            virtualCamera.m_Lens.FieldOfView = currentFOV;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.FieldOfView = _reachedFOV;
    }

    public void ShakeCamera(float _intensity, float _time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = _intensity;
        startingIntensity = _intensity;
        shakeTotalTime = _time;
        shakeTimer = _time;
    }

    private void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            float shackProgression = 1 - (shakeTimer / shakeTotalTime); 
            float smoothT = 1 - Mathf.Pow(1 - shackProgression, 3);
            cinemachineBasicMultiChannelPerlin.m_FrequencyGain = Mathf.Lerp(startingIntensity, 0f, smoothT);
        }
    }
}
