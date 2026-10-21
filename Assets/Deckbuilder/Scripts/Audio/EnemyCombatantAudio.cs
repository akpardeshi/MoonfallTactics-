using Deckbuilder.Combat;
using ModularAudio.Core.Scripts.ScriptableEventChannels;
using UnityEngine;

namespace Deckbuilder.Audio
{
    public class EnemyCombatantAudio: MonoBehaviour, ICombatantAudioPresentation
    {
        #region Variables
        
        [SerializeField] private AudioClip deathSfx;
        [SerializeField] private AudioClip shieldSfx;
        [SerializeField] private AudioClip damageSfx;
        
        [SerializeField] private AudioEventChannelSo audioEventChannelSo;
        
        #endregion
        
        
        #region SFX Playback 
        
        public void PlayAttackSfx(ActionId actionId)
        {
            
        }

        public void PlayDamageSfx()
        {
            audioEventChannelSo?.RaiseEvent(damageSfx);
        }

        public void PlayShieldBlockSfx()
        {
            audioEventChannelSo?.RaiseEvent(shieldSfx);
        }

        public void PlayDeathSfx()
        {
            audioEventChannelSo?.RaiseEvent(deathSfx);
        }
        
        #endregion
    }
}