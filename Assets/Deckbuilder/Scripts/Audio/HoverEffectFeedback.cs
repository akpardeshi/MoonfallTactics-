using ModularAudio.Core.Scripts.ScriptableEventChannels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Deckbuilder.Audio
{
    public class HoverEffectFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,  IPointerDownHandler
    {
        #region Variables
        
        [SerializeField] private AudioClip hoverAudioClip;
        [SerializeField] private AudioClip hoverExitAudioClip;
        [SerializeField] private AudioClip clickAudioClip;

        [SerializeField] private AudioEventChannelSo audioChannel;
        
        #endregion
        
        
        #region SFX Playback
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            audioChannel?.RaiseEvent(hoverAudioClip);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            audioChannel?.RaiseEvent(hoverExitAudioClip);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            audioChannel?.RaiseEvent(clickAudioClip);
        }
        
        #endregion
    }
}