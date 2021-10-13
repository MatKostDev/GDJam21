using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    CinemachineVirtualCamera vCam;

    float shakeTimer;

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin noise = 
                    vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                noise.m_AmplitudeGain = 0f;
            }
        }
    }

    public void ApplyShake(float a_intensity, float a_duration)
    {
        CinemachineBasicMultiChannelPerlin noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        noise.m_AmplitudeGain = a_intensity;

        shakeTimer = a_duration;
    }
}
