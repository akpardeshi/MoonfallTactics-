using System;
using DG.Tweening;
using UnityEngine;

namespace Deckbuilder.Cards
{
    public class CardTransitionController : MonoBehaviour
    {
        #region Variables

        private RectTransform _rectTransform;
        private RectTransform _parentRect;

        #endregion


        #region Event Functions

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        #endregion


        #region Setup

        public void SetParent(RectTransform parent)
        {
            _parentRect = parent;
        }

        #endregion


        #region Basic Transitions

        public void Move(Vector2 position, float duration, Action onComplete = null)
        {
            KillExistingTweens(_rectTransform);

            _rectTransform
                .DOAnchorPos(position, duration)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Scale(float scale, Vector2 parentSize, float duration, Action onComplete = null)
        {
            KillExistingTweens(_rectTransform);
            KillExistingTweens(_parentRect);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(_rectTransform.DOScale(scale, duration))
                .Join(_parentRect.DOSizeDelta(parentSize, duration))
                .OnComplete(() => onComplete?.Invoke());
        }

        public void MoveAndScale(Vector2 position, float scale, Vector2 parentSize, float duration,
            Action onComplete = null)
        {
            KillExistingTweens(_rectTransform);
            KillExistingTweens(_parentRect);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(_rectTransform.DOScale(scale, duration))
                .Join(_parentRect.DOSizeDelta(parentSize, duration))
                .Join(_rectTransform.DOAnchorPos(position, duration))
                .OnComplete(() => onComplete?.Invoke());
        }

        #endregion


        #region Card Transitions

        public void RotateAndScale(Vector3 position, float scale, float rotation, float duration,
            Action appendCallback = null, Action onComplete = null)
        {
            KillExistingTweens(_rectTransform);

            Vector2 start = _rectTransform.anchoredPosition;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_rectTransform.DOScale(scale, duration))
                .Join(_rectTransform.DOLocalRotate(new Vector3(0, 0, rotation), duration, RotateMode.FastBeyond360))
                .AppendCallback(() => { appendCallback?.Invoke(); });
            sequence.Append(CreateBezierMovement(start, position, duration));
            sequence.Join(_rectTransform.DOScale(scale, duration));
            sequence.OnComplete(() => onComplete?.Invoke());
        }

        public void CardDrawTransition(Vector2 position, float targetScale, float duration,
            Action onComplete = null)
        {
            KillExistingTweens(_rectTransform);

            Vector2 start = _rectTransform.anchoredPosition;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(CreateBezierMovement(start, position, duration));
            sequence.Join(_rectTransform.DOScale(targetScale, duration));
            sequence.SetEase(Ease.OutBack);
            sequence.OnComplete(() => { onComplete?.Invoke(); });
        }

        public void AttackCardTransition(Vector2 position, float targetScale, float duration, float offset,
            Action appendCallback = null, Action onComplete = null)
        {
            KillExistingTweens(_rectTransform);

            Vector2 start = _rectTransform.anchoredPosition;
            start.y += offset;
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y + 500.0f, duration))
                .AppendCallback(() => { appendCallback?.Invoke(); });
            sequence.Append(_rectTransform.DOLocalRotate(new Vector3(0, 0, 270.0f), duration,
                RotateMode.FastBeyond360));
            sequence.Append(CreateBezierMovement(start, position, duration));
            sequence.Join(_rectTransform.DOScale(targetScale, duration));
            sequence.OnComplete(() => { onComplete?.Invoke(); });
        }

        private Tween CreateBezierMovement(Vector2 start, Vector2 end, float duration)
        {
            Vector2 controlPoint = GetControlPoint(start, end);

            return DOVirtual.Float(0f, 1f, duration,
                t => { _rectTransform.anchoredPosition = EvaluateQuadraticBezier(start, controlPoint, end, t); });
        }

        #endregion


        #region Utilities

        private void KillExistingTweens(RectTransform target)
        {
            DOTween.Kill(target);
        }

        private Vector2 EvaluateQuadraticBezier(Vector2 start, Vector2 control, Vector2 end, float t)
        {
            float oneMinusT = 1f - t;

            return
                oneMinusT * oneMinusT * start +
                2f * oneMinusT * t * control +
                t * t * end;
        }

        private Vector2 GetControlPoint(Vector2 start, Vector2 end)
        {
            Vector2 direction = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            float curveAmount = Mathf.Min(
                Vector2.Distance(start, end) * 0.15f,
                100f);

            return Vector2.Lerp(start, end, 0.5f) + perpendicular * curveAmount;
        }

        #endregion
    }
}