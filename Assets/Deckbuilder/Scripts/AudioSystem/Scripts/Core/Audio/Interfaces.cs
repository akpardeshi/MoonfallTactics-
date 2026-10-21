using ModularAudio.Core.Scripts.ScriptableEventChannels;
using UnityEngine;
using UnityEngine.Serialization;

namespace ModularAds.Scripts.Core.Audio
{
    public interface IAudioControllable
    {
        void PlayAudio(AudioClip clip);
        void StopAudio();
    }

    public interface IAudioPlayable
    {
        AudioEventChannelSo AudioEventChannelSo { get; }

        void PlayAudio(AudioWrapperStruct AudioWrappers);

        AudioWrapperStruct[] AudioWrappers { get; }
    }

    [System.Serializable]
    public struct AudioWrapperStruct
    {
        public string sfxName;
        public AudioClip clip;
    }
}