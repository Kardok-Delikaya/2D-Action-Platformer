using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

namespace ActionPlatformer
{
    public class CameraShakeEffect : MonoBehaviour
    {
        public bool screenShake = true;

        public IEnumerator ShakeCameraCorutine(float shakeIntensity, float shakeTime)
        {
            if (screenShake)
            {
                CinemachineBasicMultiChannelPerlin _cbmcp = GetComponent<CinemachineBasicMultiChannelPerlin>();
                _cbmcp.AmplitudeGain = shakeIntensity;
                yield return new WaitForSeconds(shakeTime);
                _cbmcp.AmplitudeGain = 0;
            }
        }
    }
}