using Unity.Cinemachine;

namespace Systems.ScreenShake
{
    public interface IScreenShakeReceivable
    {
        ScreenShakeEventChannel ScreenShakeEventChannel { get; }

        CinemachineImpulseSource CinemachineImpulseSource { get; }
        CinemachineImpulseListener CinemachineImpulseListener { get; }

        void OnScreenShake(CameraShakeProfile cameraShakeProfile);
    }
}