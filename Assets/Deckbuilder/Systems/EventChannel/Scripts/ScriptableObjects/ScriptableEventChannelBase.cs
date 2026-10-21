using System;
using UnityEngine;

namespace Systems.EventChannel.ScriptableObjects
{
    public abstract class ScriptableEventChannelBase<T> : ScriptableObject
    {
        public Action<T> OnEventRaised;

        public virtual void RaiseEvent(T eventData)
        {
            OnEventRaised?.Invoke(eventData);
        }
    }
}