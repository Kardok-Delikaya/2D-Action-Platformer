using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraShakeEffect : MonoBehaviour
{
    public bool canScreenShake = true;

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