namespace Systems.ScreenShake
{
    public class ScreenShakeEffect : IScreenShakable
    {
        #region Variables

        // public CameraShakeProfileScriptable CameraShakeProfile { get; }

        public ScreenShakeEventChannel ScreenShakeEventChannel { get; }

        #endregion


        #region Initialization

        public ScreenShakeEffect(/*CameraShakeProfileScriptable cameraShakeProfile,*/
            ScreenShakeEventChannel screenShakeEventChannel)
        {
            // CameraShakeProfile = cameraShakeProfile;
            ScreenShakeEventChannel = screenShakeEventChannel;
        }

        #endregion


        #region IScreenShakable

        public void RaiseShakeScreenEvent(CameraShakeProfileScriptable cameraShakeProfile)
        {
            ScreenShakeEventChannel?.RaiseEvent(cameraShakeProfile.GetCameraShakeProfile());
        }

        #endregion
    }
}