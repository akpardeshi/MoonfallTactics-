using System;
using UnityEngine;

namespace Systems.ScreenShake
{
    [CreateAssetMenu(fileName = "ScreenShakeEventChannel", menuName = "Systems/CameraShake/ScreenShakeEventChannel")]
    public class ScreenShakeEventChannel: ScriptableObject /*: ScriptableEventChannelBase<CameraShakeProfile>*/
    {
        public Action<CameraShakeProfile> OnEventRaised;
        
        public void RaiseEvent(CameraShakeProfile eventData)
        {
            OnEventRaised?.Invoke(eventData);
        }
    }
}