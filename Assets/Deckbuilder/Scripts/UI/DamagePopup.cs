using Deckbuilder.Health;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Pool;

namespace Deckbuilder.Scripts.UI
{
    public class DamagePopup : MonoBehaviour
    {
        #region Variables
        
        [SerializeField] private TextMeshProUGUI damageText;

        private RectTransform _rectTransform;

        public IObjectPool<DamagePopup> ObjectPool;
        
        #endregion
        
        
        
        #region Event Functions

        private void Awake()
        {
            TryGetComponent(out _rectTransform);
        }

        #endregion
        
        
        #region Tween
        
        public void StartTransitions()
        {
            transform.localScale = Vector3.one * 0.005f;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_rectTransform.DOScale(0.007f, 0.35f));
            sequence.Append(_rectTransform.DOScale(0f, 0.55f));
            sequence.OnComplete(Release);
        }
        
        #endregion
        
        
        #region UI Setup

        public void DisplayDamage(DamageResultWrapper damageResultWrapper)
        {
            switch (damageResultWrapper.DamageResult)
            {
                case DamageResult.Shield:
                    damageText.text = "Blocked!!";
                    break;

                case DamageResult.Damage:
                case DamageResult.Death:
                    damageText.text = $"{damageResultWrapper.HealthDamageDealt}";
                    break;
            }
        }
        
        #endregion
        
        
        #region Pooling

        private void Release()
        {
            ObjectPool.Release(this);
        }

        public void Activate()
        {
            ManageActivity(true);
        }

        public void Deactivate()
        {
            ManageActivity(false);
        }
        
        #endregion
        

        #region Activity
        
        private void ManageActivity(bool activeStatus)
        {
            gameObject.SetActive(activeStatus);
        }
        
        #endregion
    }
}