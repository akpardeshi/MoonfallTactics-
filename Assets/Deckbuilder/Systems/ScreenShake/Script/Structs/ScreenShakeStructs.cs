using Unity.Cinemachine;

namespace Systems.ScreenShake
{
    [System.Serializable]
    public struct CameraShakeProfile
    {
        public CinemachineImpulseDefinition cinemachineImpulseDefinition;
        public CinemachineImpulseListener.ImpulseReaction impulseReaction;
        public float impulseWithForce;
    }
}