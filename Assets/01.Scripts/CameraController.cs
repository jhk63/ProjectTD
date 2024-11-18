using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private Transform player;

    void Awake()
    {
        player = FindObjectOfType<Player>().transform;

        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        SetVirtualCamera();
    }

    private void SetVirtualCamera()
    {
        virtualCamera.Follow = player;
        virtualCamera.LookAt = player;
    }

    public void ShakeCamera(float intensity, float duration)
    {
        CinemachineBasicMultiChannelPerlin perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        if (perlin != null)
        {
            perlin.m_AmplitudeGain = intensity;
            StartCoroutine(StopShake(duration, perlin));
        }
    }

    private IEnumerator StopShake(float duration, CinemachineBasicMultiChannelPerlin perlin)
    {
        yield return new WaitForSeconds(duration);
        perlin.m_AmplitudeGain = 0f;
    }
}
