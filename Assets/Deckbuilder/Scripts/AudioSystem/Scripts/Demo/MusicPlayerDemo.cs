using ModularAudio.Core.Scripts.ScriptableEventChannels;
using ModularAds.Scripts.Core.Audio;
using UnityEngine;

namespace ModularAds.Scripts.Demo
{
    public class MusicPlayerDemo : MonoBehaviour, IAudioPlayable
    {
        [field: SerializeField] public AudioEventChannelSo AudioEventChannelSo { get; private set; }

        private void Start()
        {
            PlayAudio1();
        }
        
        public void PlayAudio(AudioWrapperStruct audioWrapper)
        {
            if (!AudioEventChannelSo)
            {
                Debug.LogWarning("AudioEventChannelSo is not set", gameObject);
            }
            
            AudioEventChannelSo.RaiseEvent(audioWrapper.clip);
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
    }
}