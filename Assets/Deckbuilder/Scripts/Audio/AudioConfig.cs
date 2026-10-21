namespace Deckbuilder.Audio
{
    [System.Serializable]
    public struct AudioConfig
    {
        public float MasterVolume;
        public float MusicVolume;
        public float SfxVolume;

        public AudioConfig(float masterVolume, float musicVolume, float sfxVolume)
        {
            MasterVolume = masterVolume;
            MusicVolume = musicVolume;
            SfxVolume = sfxVolume;
        }
    }
}