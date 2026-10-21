using UnityEngine;

namespace Deckbuilder.Gameplay
{
    public class ParticleEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystem;

        public void Play()
        {
            particleSystem.Play();
        }

        public void Stop()
        {
            particleSystem.Stop();
        }
    }
}