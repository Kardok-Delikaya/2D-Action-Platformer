using System;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraShakeEffect : MonoBehaviour
{
    public static CameraShakeEffect Instance;
    
    public bool canScreenShake = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public IEnumerator ShakeCameraCorutine(float shakeIntensity, float shakeTime)
    {
        if (canScreenShake)
        {
            CinemachineBasicMultiChannelPerlin _cbmcp = GetComponent<CinemachineBasicMultiChannelPerlin>();
            _cbmcp.AmplitudeGain = shakeIntensity;
            yield return new WaitForSeconds(shakeTime);
            _cbmcp.AmplitudeGain = 0;
        }
    }
}