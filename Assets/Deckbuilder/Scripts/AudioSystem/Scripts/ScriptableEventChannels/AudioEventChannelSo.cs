using System;
using UnityEngine;

namespace ModularAudio.Core.Scripts.ScriptableEventChannels
{
    [CreateAssetMenu(fileName = "AudioEventChannelSO", menuName = "Scriptable Objects/AudioEventChannelSO")]
    public class AudioEventChannelSo : ScriptableObject
    {
        public Action<AudioClip> OnEventRaised;

        public void RaiseEvent(AudioClip clip)
        {
            if (!clip)
            {
                Debug.LogWarning("[Audio] Play event raised with null clip.");
                return;
            }

            OnEventRaised?.Invoke(clip);
        }
    }
}