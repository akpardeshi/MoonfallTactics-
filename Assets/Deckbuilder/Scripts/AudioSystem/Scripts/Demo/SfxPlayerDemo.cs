using ModularAudio.Core.Scripts.ScriptableEventChannels;
using ModularAds.Scripts.Core.Audio;
using UnityEngine;

namespace ModularAds.Scripts.Demo
{
    public class SfxPlayerDemo : MonoBehaviour, IAudioPlayable
    {
        [field: SerializeField] public AudioEventChannelSo AudioEventChannelSo { get; private set; }

        public void PlayAudio(AudioClip audioClip)
        {
            if (!AudioEventChannelSo)
            {
                Debug.LogWarning("AudioEventChannelSo is not set", gameObject);
            }

            AudioEventChannelSo.RaiseEvent(audioClip);
        }

        public void PlayAudio(AudioWrapperStruct audioWrapper)
        {
            PlayAudio(audioWrapper.clip);
        }

        [field: SerializeField] public AudioWrapperStruct[] AudioWrappers { get; private set; }

        public void PlayAudio1()
        {
            PlayAudio(AudioWrappers[0]);
        }

        public void PlayAudio2()
        {
            PlayAudio(AudioWrappers[1]);
        }

        public void PlayAudio3()
        {
            PlayAudio(AudioWrappers[2]);
        }
    }
}