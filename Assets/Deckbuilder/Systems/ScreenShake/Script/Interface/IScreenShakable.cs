namespace Systems.ScreenShake
{
    public interface IScreenShakable
    {
        // CameraShakeProfileScriptable CameraShakeProfile { get; }

        ScreenShakeEventChannel ScreenShakeEventChannel { get; }

        void RaiseShakeScreenEvent(CameraShakeProfileScriptable cameraShakeProfile);
    }
}