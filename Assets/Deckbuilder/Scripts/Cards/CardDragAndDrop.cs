using System;
using UnityEngine.EventSystems;

namespace Deckbuilder.Cards
{
    public class CardDragAndDrop : DragBase, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        #region Events
        
        public event Action<CardDragEvent, PointerEventData> OnCardDragEvent;
        
        #endregion
        
        
        #region Dragging

        private bool _canDrag;
        
        public bool CanDrag 
        {
            set => _canDrag = value;
        }

        #endregion
        
 
        #region Pointer Events
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!_canDrag)
            {
                return;
            }
            
            base.OnBeginDrag(eventData);
            
            OnCardDragEvent?.Invoke(CardDragEvent.BeginDrag, eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!_canDrag)
            {
                return;
            }
            
            base.OnDrag(eventData);
            
            OnCardDragEvent?.Invoke(CardDragEvent.Drag, eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (!_canDrag)
            {
                return;
            }
            
            base.OnEndDrag(eventData);

            OnCardDragEvent?.Invoke(CardDragEvent.EndDrag, eventData);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnCardDragEvent?.Invoke(CardDragEvent.PointerEnter, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnCardDragEvent?.Invoke(CardDragEvent.PointerExit, eventData);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnCardDragEvent?.Invoke(CardDragEvent.PointerDown, eventData);
        }
        
        #endregion
    }
}