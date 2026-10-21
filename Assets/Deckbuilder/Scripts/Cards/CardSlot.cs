using UnityEngine;
using UnityEngine.Pool;

namespace Deckbuilder.Cards
{
    public class CardSlot : MonoBehaviour
    {
        #region Variables
        
        private RectTransform _rectTransform;

        public IObjectPool<CardSlot> ObjectPool;
        
        private Vector2 _originalSize;
        public Vector2 OriginalSize => _originalSize;
        
        #endregion
        
        
        #region Event functions
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalSize = _rectTransform.sizeDelta;
        }
        
        #endregion
        
        
        #region Lifecycle Functions
        
        public void Release()
        {
            ObjectPool.Release(this);
        }

        public void Activate()
        {
            ManageActivity(true);
        }

        public void Deactivate()
        {
            ResetSize();
            ManageActivity(false);
        }

        private void ManageActivity(bool activeStatus)
        {
            gameObject.SetActive(activeStatus);
        }

        private void ResetSize()
        {
            _rectTransform.sizeDelta = _originalSize;
        }
        
        #endregion
    }
}