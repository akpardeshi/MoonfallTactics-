using UnityEngine;

namespace Systems.ScreenShake
{
    [CreateAssetMenu(fileName = "CameraShakeProfileSO", menuName = "Systems/CameraShake/CameraShakeProfile")]
    public class CameraShakeProfileScriptable : ScriptableObject
    {
        [SerializeField] private CameraShakeProfile cameraShakeProfile;

        public CameraShakeProfile GetCameraShakeProfile() => cameraShakeProfile;
    }
}