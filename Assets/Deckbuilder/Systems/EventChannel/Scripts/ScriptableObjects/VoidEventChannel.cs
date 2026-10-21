using System;
using UnityEngine;

namespace Systems.EventChannel.ScriptableObjects
{
    [CreateAssetMenu(fileName = "VoidEventChannel", menuName = "Systems/ScriptableEventChannels/VoidEventChannel")]
    public class VoidEventChannel : ScriptableObject
    {
        public Action OnEventRaised;

        public void RaiseEvent()
        {
            OnEventRaised?.Invoke();
        }
    }
}