using Unity.Cinemachine;
using UnityEngine;

namespace Systems.ScreenShake
{
    [RequireComponent(typeof(CinemachineImpulseListener))]
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class ScreenShakeManager : MonoBehaviour, IScreenShakeReceivable
    {
        #region Variables

        [field: SerializeField] public ScreenShakeEventChannel ScreenShakeEventChannel { get; private set; }

        public CinemachineImpulseSource CinemachineImpulseSource { get; private set; }

        public CinemachineImpulseListener CinemachineImpulseListener { get; private set; }

        #endregion


        #region Event Functions

        private void Awake()
        {
            Initialize();
        }

        private void OnEnable()
        {
            if (ScreenShakeEventChannel)
            {
                ScreenShakeEventChannel.OnEventRaised += OnScreenShake;
            }
        }

        private void OnDisable()
        {
            if (ScreenShakeEventChannel)
            {
                ScreenShakeEventChannel.OnEventRaised -= OnScreenShake;
            }
        }

        #endregion


        #region Initialization

        private void Initialize()
        {
            CinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
            CinemachineImpulseListener = GetComponent<CinemachineImpulseListener>();
        }

        #endregion


        #region IScreenShakeReceivable

        public void OnScreenShake(CameraShakeProfile cameraShakeProfile)
        {
            CinemachineImpulseListener.ReactionSettings = cameraShakeProfile.impulseReaction;

            CinemachineImpulseSource.ImpulseDefinition = cameraShakeProfile.cinemachineImpulseDefinition;
            CinemachineImpulseSource.GenerateImpulseWithForce(cameraShakeProfile.impulseWithForce);
        }

        #endregion
    }
}