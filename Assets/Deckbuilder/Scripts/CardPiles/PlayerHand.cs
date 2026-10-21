using System.Collections.Generic;
using Deckbuilder.Cards;
using Deckbuilder.Gameplay;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Deckbuilder.CardPiles
{
    public class PlayerHand : CardPileBase
    {
        #region Variables

        public int PlayerHandSize => _activeCards.Count;

        [SerializeField] private int maxCardCount;
        public int MaxCardCount => maxCardCount;

        #endregion


        #region Unity Event Functions

        private void Awake()
        {
            InitializeCardsPool();
            InitializeCardsSlotPool();

            TryGetComponent(out _layoutGroup);
        }

        private void Start()
        {
            _offset = GetOffset();
        }

        #endregion


        #region Pooling

        [SerializeField] private int initialSize;
        [SerializeField] private int maxSize;

        private IObjectPool<PlayerCard> _cardsPool;

        [SerializeField] private PlayerCard playerCardPrefab;
        [SerializeField] private bool cardsCollectionChecks;

        private void InitializeCardsPool()
        {
            _cardsPool = new ObjectPool<PlayerCard>(CreatePlayerCard, GetCard, ReleaseCard, DestroyCard,
                cardsCollectionChecks, initialSize,
                maxSize);
        }

        private PlayerCard CreatePlayerCard()
        {
            PlayerCard playerCard = Instantiate(playerCardPrefab);
            playerCard.ObjectPool = _cardsPool;

            playerCard.CardDragAndDrop.SetDragParent(
                (RectTransform)GameObject.FindGameObjectWithTag(GameData.DragParent).transform);

            return playerCard;
        }

        private void GetCard(PlayerCard playerCard)
        {
            playerCard.Activate();
            playerCard.SetVisibility(false);
        }

        private void ReleaseCard(PlayerCard playerCard)
        {
            playerCard.Deactivate();
        }

        private void DestroyCard(PlayerCard playerCard)
        {
            if (!playerCard) return;

            Destroy(playerCard.gameObject);
        }

        // Card Selection Slot
        private IObjectPool<CardSlot> _cardsSlotPool;

        [SerializeField] private bool cardsSlotCollectionChecks;
        [SerializeField] private CardSlot cardSlotPrefab;
        [SerializeField] private RectTransform cardsContainer;

        [SerializeField] private float transitionDuration = 0.2f;

        private void InitializeCardsSlotPool()
        {
            _cardsSlotPool = new ObjectPool<CardSlot>(CreateCardSlot, GetCardSlot, ReleaseCardSlot, DestroyCardSlot,
                cardsSlotCollectionChecks, initialSize,
                maxSize);
        }

        private CardSlot CreateCardSlot()
        {
            CardSlot cardSlot = Instantiate(cardSlotPrefab, cardsContainer);
            cardSlot.ObjectPool = _cardsSlotPool;
            return cardSlot;
        }

        private void GetCardSlot(CardSlot cardSlot)
        {
            cardSlot.Activate();
        }

        private void ReleaseCardSlot(CardSlot cardSlot)
        {
            RectTransform cardSlotTransform = cardSlot.transform as RectTransform;
            cardSlotTransform.DOSizeDelta(Vector2.zero, transitionDuration).OnComplete(cardSlot.Deactivate);
        }

        private void DestroyCardSlot(CardSlot cardSlot)
        {
            if (!cardSlot) return;

            Destroy(cardSlot.gameObject);
        }

        #endregion


        #region Overrides

        public override void ResetPile()
        {
            base.ResetPile();

            for (int i = _activeCards.Count - 1; i >= 0; i--)
            {
                RemoveVisualCard(_activeCards[i]);
            }
        }

        public override void Add(List<CardDataBase> cardsDataBase)
        {
            base.Add(cardsDataBase);

            List<PlayerCard> drawnCards = new();

            foreach (var cardDataBase in cardsDataBase)
            {
                drawnCards.Add(AddCardVisual(cardDataBase));
            }
            
            PlayDrawTransitions(drawnCards);
        }

        #endregion


        #region Visuals

        [SerializeField] private float drawTransitionStagger = 0.07f;
        [SerializeField] private RectTransform discardParent;
        [SerializeField] private RectTransform exhaustParent;
        [SerializeField] private RectTransform drawOrigin;

        private readonly List<PlayerCard> _activeCards = new();
        private readonly List<CardSlot> _cardSlots = new();

        private HorizontalLayoutGroup _layoutGroup;
        private float _offset;

        private float GetOffset()
        {
            RectTransform rectTransform = (RectTransform)cardSlotPrefab.transform;
            return rectTransform.rect.width - _layoutGroup.spacing;
        }

        private PlayerCard AddCardVisual(CardDataBase cardDataBase)
        {
            CardSlot slot = _cardsSlotPool.Get();
            PlayerCard playerCard = _cardsPool.Get();

            SetupCardVisual(cardDataBase, playerCard, slot);

            _activeCards.Add(playerCard);
            _cardSlots.Add(slot);

            return playerCard;
        }

        private void SetupCardVisual(CardDataBase cardDataBase, PlayerCard playerCard, CardSlot slot)
        {
            playerCard.SetDiscardParent(discardParent);
            playerCard.SetExhaustParent(exhaustParent);

            playerCard.transform.SetParent(drawOrigin);
            slot.transform.SetSiblingIndex(PlayerHandSize);

            ((RectTransform)playerCard.transform).anchoredPosition = Vector3.zero;
            playerCard.transform.localScale = Vector3.one * 0.2f;

            playerCard.SetCardSlot(slot);
            playerCard.SetSlotParent((RectTransform)slot.transform);

            playerCard.PlayableImage.raycastTarget = true;
            playerCard.SetCard(cardDataBase);

            SetCardInitialState(playerCard, slot.transform);
        }

        public void RemoveVisualCard(PlayerCard card)
        {
            card.Release();
            card.CardSlot.Release();

            _activeCards.Remove(card);
            _cardSlots.Remove(card.CardSlot);
        }

        private void SetCardInitialState(PlayerCard card, Transform parent)
        {
            card.transform.SetParent(parent, true);

            Vector2 anchorPos = ((RectTransform)card.transform).anchoredPosition;
            anchorPos.x -= 155 * (card.transform.parent.GetSiblingIndex() - 1);

            ((RectTransform)card.transform).anchoredPosition = anchorPos;
        }

        private void ReleaseCard(PlayerCard card, CardSlot slot)
        {
            if (!card) return;

            _activeCards.Remove(card);
            _cardSlots.Remove(slot);

            card.Release();
            slot.Release();
        }

        public void DiscardCards()
        {
            for (int i = _activeCards.Count - 1; i >= 0; i--)
            {
                PlayerCard card = _activeCards[i];
                float delay = i * drawTransitionStagger;

                DOVirtual.DelayedCall(
                    delay,
                    () => PlayDiscardTransition(card));
            }

            base.ResetPile();
        }

        #endregion


        #region Transitions

        private void PlayDrawTransitions(List<PlayerCard> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                PlayerCard card = cards[i];
                float delay = i * drawTransitionStagger;

                DOVirtual.DelayedCall(
                    delay,
                    () => PlayDrawTransition(card));
            }
        }

        private void PlayDrawTransition(PlayerCard card)
        {
            card.CardTransitionController.CardDrawTransition(
                Vector3.zero,
                1f,
                transitionDuration,
                () => { card.ManageTrailRendererVisibility(false); });
        }

        private void PlayDiscardTransition(PlayerCard card)
        {
            if (!card) return;

            CardSlot slot = card.CardSlot;
            card.transform.SetParent(discardParent, true);

            card.CardTransitionController.MoveAndScale(
                Vector2.zero,
                0f,
                card.CardSlot.OriginalSize,
                transitionDuration,
                () => { ReleaseCard(card, slot); });
        }

        #endregion
    }
}