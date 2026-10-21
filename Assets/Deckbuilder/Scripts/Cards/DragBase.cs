using UnityEngine;
using UnityEngine.EventSystems;

namespace Deckbuilder.Cards
{
    public class DragBase : MonoBehaviour, IDraggable, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        #region Variables

        [SerializeField] private Canvas canvas;

        public bool IsDragging;

        // Position
        public Vector2 InitialMousePosition { get; set; }
        public Vector2 InitialObjectPosition { get; set; }

        // RectTransform
        public RectTransform DragTarget { get; set; }

        private RectTransform _dragParent;

        #endregion


        #region Event Functions

        protected virtual void Awake()
        {
            DragTarget = GetComponent<RectTransform>();
        }

        #endregion


        #region Drag Functions
        
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (IsDragging)
                return;

            if (_dragParent == null)
            {
                Debug.LogError($"{name}: Drag parent has not been assigned.");
                return;
            }

            IsDragging = true;

            SetParent(_dragParent);

            RectTransform parentRect = DragTarget.parent as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPointerPosition);

            InitialMousePosition = localPointerPosition;
            InitialObjectPosition = DragTarget.localPosition;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!IsDragging)
                return;

            RectTransform parentRect = DragTarget.parent as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPointerPosition);

            Vector2 offset = localPointerPosition - InitialMousePosition;

            DragTarget.localPosition = InitialObjectPosition + offset;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!IsDragging) return;

            IsDragging = false;
        }

        #endregion


        #region parenting

        public void SetDragParent(RectTransform parent)
        {
            _dragParent = parent;
        }

        public void SetParent(RectTransform parent)
        {
            DragTarget.SetParent(parent, true);
        }

        #endregion
    }
}