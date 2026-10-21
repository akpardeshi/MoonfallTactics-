using UnityEngine;
using UnityEngine.EventSystems;

namespace Deckbuilder.Cards
{
    public interface IDraggable
    {
        #region Variables
        
        Vector2 InitialMousePosition { get; set; }
        
        RectTransform DragTarget { get; set; }
        
        Vector2 InitialObjectPosition{ get; set; }
        
        #endregion

        
        #region Functions
        
        public void OnDrag(PointerEventData eventData);
        public void OnBeginDrag(PointerEventData eventData);
        public void OnEndDrag(PointerEventData eventData);
        
        #endregion
    }
}