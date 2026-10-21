using System.Collections.Generic;
using Deckbuilder.Combat;
using Deckbuilder.Gameplay;
using Deckbuilder.TurnSystem;
using DG.Tweening;
using ModularAudio.Core.Scripts.ScriptableEventChannels;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Pool;

namespace Deckbuilder.Cards
{
    [SelectionBase]
    public class PlayerCard : MonoBehaviour
    {
        #region Card Data

        [SerializeField] private CardDataBase cardData;

        public CardDataBase CardData => cardData;

        [SerializeField] private TextMeshProUGUI energyNeeded;
        
        [SerializeField] private Image headerImage;

        public void SetCard(CardDataBase card)
        {
            // reset
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            trailRenderer.enabled = false;
            _canvasGroup.alpha = 1;
            _playableImage.raycastTarget = true;

            cardData = card;
            _cardDragAndDrop.CanDrag = !cardData.NeedsTargeting;
            energyNeeded.text = $"{card.EnergyNeeded}";
            
            SetCardVisuals(card);
        }

        #endregion


        #region Card View

        [SerializeField] private TextMeshProUGUI cardName;
        [SerializeField] private TextMeshProUGUI cardDescription;
        [SerializeField] private Image cardIcon;

        private void SetCardVisuals(CardDataBase card)
        {
            cardName.text = card.CardName;
            cardName.color = card.HeaderColor;
            
            cardDescription.text = card.CardDescription;
            
            cardIcon.sprite = card.CardIcon;
            
            headerImage.color = card.AccentColor;
        }

        #endregion


        #region Pooling

        public IObjectPool<PlayerCard> ObjectPool;

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void Release()
        {
            ObjectPool.Release(this);
        }

        #endregion


        #region State Transitions

        private CardStateMachine _cardStateMachine;

        private bool TryChangeState(CardState newState)
        {
            return _cardStateMachine.TryTransition(newState);
        }

        #endregion


        #region UI

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        [SerializeField, Range(0.0f, 1.0f)] private float dragOpacity = 0.75f;

        private Image _playableImage;
        public Image PlayableImage => _playableImage;

        [SerializeField] private int hoverSorting = 111;
        [SerializeField] private int defaultSorting = 110;

        public void SetVisibility(bool canvasOverrideSorting)
        {
            _canvas.overrideSorting = canvasOverrideSorting;
            _canvas.sortingOrder = canvasOverrideSorting ? hoverSorting : defaultSorting;
        }

        #region Presentation

        private void SetDragVisualState(bool canvasGroupBlocksRaycasts)
        {
            if (!_canvasGroup) return;

            _canvasGroup.alpha = canvasGroupBlocksRaycasts ? 1 : dragOpacity;
        }

        #endregion

        #endregion


        #region Card Slot

        private CardSlot _cardSlot;
        public CardSlot CardSlot => _cardSlot;

        private RectTransform SlotParent { get; set; }

        public void SetSlotParent(RectTransform slotParent)
        {
            SlotParent = slotParent;
            _cardTransitionController.SetParent(SlotParent);
        }

        #endregion


        #region Event Functions

        private void Awake()
        {
            _mainCamera = Camera.main;

            _cardDragAndDrop = GetComponent<CardDragAndDrop>();
            _cardDragAndDrop.OnCardDragEvent += OnCardDragEvent;

            _cardStateMachine = new();

            _cardTransitionController = GetComponent<CardTransitionController>();

            _playableImage = GetComponentInChildren<Image>();

            TryGetComponent(out _canvas);
            TryGetComponent(out _canvasGroup);
        }

        private void OnDestroy()
        {
            _cardDragAndDrop.OnCardDragEvent -= OnCardDragEvent;
        }

        #endregion


        #region Event Handlers

        [SerializeField] private float cardScale = 1.2f;
        [SerializeField] private float transitionDuration = 0.2f;

        private void OnCardDragEvent(CardDragEvent cardDragEvent, PointerEventData eventData)
        {
            if (!GameManager.Instance) return;
            
            if (GameManager.Instance.TargetingController.IsTargeting)
            {
                return;
            }

            switch (cardDragEvent)
            {
                case CardDragEvent.BeginDrag:
                    OnBeginDrag();
                    break;

                case CardDragEvent.EndDrag:
                    OnEndDrag(eventData);
                    break;

                case CardDragEvent.PointerEnter:
                    OnPointerEnter();
                    break;

                case CardDragEvent.PointerExit:
                    OnPointerExit();
                    break;

                case CardDragEvent.PointerDown:
                    OnPointerDown();
                    break;
            }
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            SetDragVisualState(true);

            if (!CanPlayCardAt(eventData.position))
            {
                HandleInvalidDrop();
                return;
            }

            if (!TryChangeState(CardState.Utilized))
            {
                return;
            }

            List<CombatantBase> targets = new();

            if (cardData.CardType == CardType.Heal || cardData.CardType == CardType.Shield)
            {
                targets.Add(GameManager.Instance.Player);
            }

            else
            {
                targets.AddRange(GameManager.Instance.EnemyManager.Enemies);
            }

            PlayCard(targets);
        }

        private void HandleInvalidDrop()
        {
            ReturnToSlot();
            PlayCardBlockSfx();
            TryChangeState(CardState.Returning);
        }

        private void OnPointerEnter()
        {
            if (!TryChangeState(CardState.Hovered))
            {
                return;
            }

            Vector2 targetSize = new(_cardSlot.OriginalSize.x * cardScale, _cardSlot.OriginalSize.y);
            _cardTransitionController.Scale(1.2f, targetSize, transitionDuration);

            SetVisibility(true);
        }

        private void OnPointerExit()
        {
            if (_cardStateMachine.CurrentState != CardState.Hovered)
            {
                return;
            }

            ResetCardScale();
        }

        private void OnPointerDown()
        {
            if (!cardData.NeedsTargeting)
            {
                return;
            }

            if (!CanPlayCard())
            {
                PlayCardBlockSfx();
                return;
            }

            if (!TryChangeState(CardState.Targeting)) return;

            StartTargeting();
        }


        private void OnBeginDrag()
        {
            SetDragVisualState(false);
            TryChangeState(CardState.Dragging);
        }

        #endregion


        #region Gameplay

        private CardDragAndDrop _cardDragAndDrop;
        public CardDragAndDrop CardDragAndDrop => _cardDragAndDrop;

        private CardTransitionController _cardTransitionController;
        public CardTransitionController CardTransitionController => _cardTransitionController;

        private Camera _mainCamera;

        private RectTransform _discardPileRect;
        private RectTransform _exhaustPileRect;

        [SerializeField] private TrailRenderer trailRenderer;

        public void PlayCard(List<CombatantBase> targets)
        {
            GameManager.Instance.Energy.ConsumeEnergy(CardData.EnergyNeeded);
            CardData.PlayCard(targets);
            PrepareForDiscard();
            
            PlayCardTransition();
        }

        private void PrepareForDiscard()
        {
            transform.SetParent(_discardPileRect);
            _playableImage.raycastTarget = false;
        }

        private void PlayCardTransition()
        {
            if (cardData.IsExhaustCard)
            {
                _canvasGroup.DOFade(0, transitionDuration).OnComplete(() =>
                {
                    transform.SetParent(_exhaustPileRect);
                    ((RectTransform) transform).anchoredPosition = Vector2.zero;
                    
                    TryChangeState(CardState.Idle);

                    CardManager.Instance.ExhaustCard(cardData);
                    CardManager.Instance.PlayerHand.RemoveVisualCard(this);
                });
                
                return;
            }
            
            if (CardData.CardType == CardType.Attack)
            {
                ExecuteAttackCardTransition();
            }

            else
            {
                ExecuteRegularCardTransition();
            }
        }

        private bool CanPlayCard()
        {
            return GameManager.Instance.Energy.HasEnergy(cardData.EnergyNeeded) &&
                   GameManager.Instance.TurnManager.CurrentParticipant == TurnParticipant.Player;
        }

        private bool CanPlayCardAt(Vector3 position)
        {
            Vector3 worldPosition = GetWorldPosition(position);
            return Physics2D.OverlapPoint(worldPosition) && CanPlayCard();
        }

        public void SetDiscardParent(RectTransform parent)
        {
            _discardPileRect = parent;
        }

        public void SetExhaustParent(RectTransform parent)
        {
            _exhaustPileRect = parent;
        }

        public void SetCardSlot(CardSlot slot)
        {
            _cardSlot = slot;
        }

        private Vector3 GetWorldPosition(Vector3 position)
        {
            return _mainCamera.ScreenToWorldPoint(
                new Vector3(
                    position.x,
                    position.y,
                    Mathf.Abs(_mainCamera.transform.position.z)
                )
            );
        }

        #endregion


        #region Presentation

        private void ReturnToSlot()
        {
            _cardDragAndDrop.SetParent(SlotParent);
            _cardTransitionController.MoveAndScale(
                Vector2.zero, 
                1.0f,
                CardSlot.OriginalSize,
                transitionDuration, 
                OnReturnedToSlot);
        }

        private void OnReturnedToSlot()
        {
            SetVisibility(false);
            _cardDragAndDrop.IsDragging = false;
            TryChangeState(CardState.Idle);
        }

        private void ResetCardScale()
        {
            if (!TryChangeState(CardState.Idle))
            {
                return;
            }

            _cardTransitionController.Scale(
                1.0f,
                _cardSlot.OriginalSize,
                transitionDuration,
                () => { SetVisibility(false); });
        }

        private void StartTargeting()
        {
            transform.SetParent(GameManager.Instance.TargetingControllerRect, true);
            _cardTransitionController.Move(GameManager.Instance.TargetingControllerRect.rect.center,
                transitionDuration);

            GameEvents.CardSelected?.Invoke(this);
        }

        public void ResetCardVisuals()
        {
            transform.SetParent(_cardSlot.transform, true);
            _cardTransitionController.MoveAndScale(Vector2.zero, 1.0f, _cardSlot.OriginalSize, transitionDuration,
                OnReturnedToSlot);
        }

        private void OnCardPlayed()
        {
            TryChangeState(CardState.Idle);

            CardManager.Instance.DiscardCard(cardData);
            CardManager.Instance.PlayerHand.RemoveVisualCard(this);
        }

        public void ManageTrailRendererVisibility(bool visible)
        {
            trailRenderer.enabled = visible;
        }

        private void ExecuteRegularCardTransition()
        {
            _cardTransitionController.RotateAndScale(Vector3.zero, .1f, 270.0f, transitionDuration,
                () => { trailRenderer.enabled = true; },
                OnCardPlayed);
        }

        [SerializeField] private int attackCardOffset = 500;

        private void ExecuteAttackCardTransition()
        {
            _cardTransitionController.AttackCardTransition(
                Vector3.zero,
                0.1f,
                transitionDuration,
                attackCardOffset,
                () => { trailRenderer.enabled = true; },
                OnCardPlayed);
        }

        #endregion


        #region Audio

        [SerializeField] private AudioEventChannelSo audioChannel;
        [SerializeField] private AudioClip cardBlockedSfx;

        private void PlayCardBlockSfx()
        {
            audioChannel?.RaiseEvent(cardBlockedSfx);
        }

        #endregion
    }
}