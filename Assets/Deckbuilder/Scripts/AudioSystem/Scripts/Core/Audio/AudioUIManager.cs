using System.Collections.Generic;
using Deckbuilder.Audio;
using EventChannel.Scripts;
using ModularAds.Scripts.Core.Audio;
using ModularForge.Trailback.Core;
using ModularForge.Trailback.Demo.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ModularAudio.Scripts
{
    public class AudioUIManager : UIBase
    {
        [Header("Audio Sliders"), SerializeField]
        private Slider masterVolumeSlider;

        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        // event channels
        [Header("Volume Event Channels"), SerializeField]
        private FloatEventChannel masterVolume;

        [SerializeField] private FloatEventChannel musicVolume;
        [SerializeField] private FloatEventChannel sfxVolume;

        [Header("Audio Mixer Channel SO"), SerializeField]
        private List<AudioMixerChannelSo> audioChannels;

        // channel mapping
        private readonly Dictionary<ModularAudioManager.AudioMixerNames, AudioMixerChannelSo> _channelMap = new();
        
        private const float InitialVolume = 0.7f;

        private const string AudioConfigKey = "AudioConfig";

        private void InitializeAudioChannels()
        {
            foreach (var channel in audioChannels)
            {
                if (channel == null)
                    continue;

                _channelMap.TryAdd(channel.channel, channel);
            }
        }

        private AudioConfig _config ;

        protected override void Awake()
        {
            base.Awake();

            if (PlayerPrefs.HasKey(AudioConfigKey))
            {
                _config = JsonUtility.FromJson<AudioConfig>(PlayerPrefs.GetString(AudioConfigKey));
                InitializeAudioVolume(_config);
            }

            else
            {
                _config = new AudioConfig(InitialVolume, InitialVolume, InitialVolume);
                InitializeAudioVolume(_config);

                string json = JsonUtility.ToJson(_config);
                PlayerPrefs.SetString(AudioConfigKey, json);
                PlayerPrefs.Save();
            }

            InitializeAudioChannels();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void Start()
        {
            OnMasterVolume(_config.MasterVolume);
            OnMusicVolume(_config.MusicVolume);
            OnSfxVolume(_config.SfxVolume);
        }

        private void OnDisable()
        {
            UnsubscribeToEvents();
        }

        private void OnMasterVolume(float value)
        {
            masterVolume?.RaiseEvent(value);
            
            AudioConfig currentConfig = JsonUtility.FromJson<AudioConfig>(PlayerPrefs.GetString(AudioConfigKey));
            AudioConfig updatedConfig = new AudioConfig(value, currentConfig.MusicVolume, currentConfig.SfxVolume);
            
            string configJson = JsonUtility.ToJson(updatedConfig);
            PlayerPrefs.SetString(AudioConfigKey, configJson);
        }

        private void OnMusicVolume(float value)
        {
            musicVolume?.RaiseEvent(value);
            
            AudioConfig currentConfig = JsonUtility.FromJson<AudioConfig>(PlayerPrefs.GetString(AudioConfigKey));
            AudioConfig updatedConfig = new AudioConfig(currentConfig.MasterVolume, value, currentConfig.SfxVolume);

            string configJson = JsonUtility.ToJson(updatedConfig);
            PlayerPrefs.SetString(AudioConfigKey, configJson);
        }

        private void OnSfxVolume(float value)
        {
            sfxVolume?.RaiseEvent(value);
            
            AudioConfig currentConfig = JsonUtility.FromJson<AudioConfig>(PlayerPrefs.GetString(AudioConfigKey));
            AudioConfig updatedConfig = new AudioConfig(currentConfig.MasterVolume, currentConfig.MusicVolume, value);

            string configJson = JsonUtility.ToJson(updatedConfig);
            PlayerPrefs.SetString(AudioConfigKey, configJson);
        }

        private void SubscribeToEvents()
        {
            if (masterVolumeSlider)
            {
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolume);
            }

            if (musicVolumeSlider)
            {
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolume);
            }

            if (sfxVolumeSlider)
            {
                sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolume);
            }
        }

        private void UnsubscribeToEvents()
        {
            if (masterVolumeSlider)
            {
                masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolume);
            }

            if (musicVolumeSlider)
            {
                musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolume);
            }
            
            if (sfxVolumeSlider)
            {
                sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolume);
            }
        }

        private void InitializeAudioVolume(AudioConfig config)
        {
            float volume = 0;
            _channelMap.TryGetValue(ModularAudioManager.AudioMixerNames.MasterVolume, out var defaultMasterChannel);
            volume = defaultMasterChannel ? defaultMasterChannel.defaultVolume : config.MasterVolume;
            masterVolume?.RaiseEvent(volume);
            masterVolumeSlider?.SetValueWithoutNotify(volume);

            _channelMap.TryGetValue(ModularAudioManager.AudioMixerNames.MusicVolume, out var defaultMusicChannel);
            volume = defaultMusicChannel ? defaultMusicChannel.defaultVolume : config.MusicVolume;
            musicVolume?.RaiseEvent(volume);
            musicVolumeSlider?.SetValueWithoutNotify(volume);

            _channelMap.TryGetValue(ModularAudioManager.AudioMixerNames.SfxVolume, out var defaultSfxChannel);
            volume = defaultSfxChannel ? defaultSfxChannel.defaultVolume : config.SfxVolume;
            sfxVolume?.RaiseEvent(volume);
            sfxVolumeSlider?.SetValueWithoutNotify(volume);
        }

        public override void Back()
        {
            base.Back();
            TrailbackFacade.Back();
        }
    }
}