using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Deckbuilder.CardPiles;
using Deckbuilder.Gameplay;
using Deckbuilder.TurnSystem;
using UnityEngine;
using Random = UnityEngine.Random;
using ModularAudio.Core.Scripts.ScriptableEventChannels;

namespace Deckbuilder.Cards
{
    public class CardManager : MonoBehaviour
    {
        #region Deck Definition

        [SerializeField] private List<DeckDefinition> playerDeck;

        #endregion


        #region Variables

        // instance
        public static CardManager Instance;

        [SerializeField] private PlayerHand playerHand;
        [SerializeField] private DrawPile drawPile;
        [SerializeField] private DiscardPile discardPile;
        [SerializeField] private ExhaustPile exhaustPile;

        public PlayerHand PlayerHand => playerHand;

        // card count
        [SerializeField] private int initialCardCount = 3;

        // Audio
        [SerializeField] private AudioEventChannelSo audioEventChannelSo;
        [SerializeField] private AudioClip cardDrawAudioClip;
        [SerializeField] private AudioClip cardShuffleAudioClip;

        #endregion


        #region Event Functions

        private void Awake()
        {
            Instance = this;

            InitializePlayerDeck();
        }

        private void OnEnable()
        {
            TurnManager.PlayerTurnStarted += OnPlayerTurnStarted;
            TurnManager.PlayerTurnEnded += OnPlayerTurnEnded;
        }

        private void OnDisable()
        {
            TurnManager.PlayerTurnStarted -= OnPlayerTurnStarted;
            TurnManager.PlayerTurnEnded -= OnPlayerTurnEnded;
        }

        #endregion


        #region Deck Operations

        private List<CardDataBase> _cardDataBases = new();

        private void InitializePlayerDeck()
        {
            _cardDataBases = Resources.LoadAll<CardDataBase>("Cards").ToList();

            Dictionary<string, CardDataBase> dict = new();

            // remap dict
            foreach (var cardDataBase in _cardDataBases)
            {
                dict.TryAdd(cardDataBase.CardID, cardDataBase);
            }

            List<CardDataBase> runtimeDeck = new();

            foreach (var cardDeckElement in GameData.PlayerDeck.DeckDefinitions)
            {
                dict.TryGetValue(cardDeckElement.CardID, out CardDataBase cardData);

                for (var i = 0; i < cardDeckElement.CardCount; i++)
                {
                    CardDataBase card = Instantiate(cardData);
                    runtimeDeck.Add(card);
                }
            }

            Shuffle(runtimeDeck);
            drawPile.Add(runtimeDeck);
        }

        private void RefillDrawPile()
        {
            List<CardDataBase> cards = discardPile.GetAll();

            discardPile.ResetPile();

            Shuffle(cards);
            audioEventChannelSo?.RaiseEvent(cardShuffleAudioClip);

            drawPile.Add(cards);
        }

        private void Shuffle(List<CardDataBase> cards)
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);

                (cards[i], cards[randomIndex]) = (cards[randomIndex], cards[i]);
            }
        }

        #endregion


        #region Turn Operations

        private void OnPlayerTurnEnded()
        {
            EndPlayerTern();
        }

        private void OnPlayerTurnStarted()
        {
            StartCoroutine(DrawCards());
        }

        private void EndPlayerTern()
        {
            discardPile.Add(playerHand.CardPile);
            playerHand.DiscardCards();
        }

        #endregion


        #region Card Operations

        public void DiscardCard(CardDataBase card)
        {
            playerHand.Remove(card);
            discardPile.Add(card);
        }

        public void ExhaustCard(CardDataBase card)
        {
            playerHand.Remove(card);
            exhaustPile.Add(card);
        }

        public IEnumerator DrawCards()
        {
            int availableCardsSlot = playerHand.MaxCardCount - playerHand.PlayerHandSize;
            int cardsToDraw = initialCardCount;

            if (availableCardsSlot <= 0)
            {
                yield break;
            }

            if (availableCardsSlot < initialCardCount)
            {
                cardsToDraw = availableCardsSlot;
            }

            var cards = drawPile.Draw(cardsToDraw);

            if (cards.Count < cardsToDraw)
            {
                int remainingCardsToDraw = cardsToDraw - cards.Count;
                RefillDrawPile();

                var additionalCards = drawPile.Draw(remainingCardsToDraw);
                cards.AddRange(additionalCards);
            }

            yield return new WaitForSeconds(0.5f);

            audioEventChannelSo?.RaiseEvent(cardDrawAudioClip);
            playerHand.Add(cards);
        }

        #endregion
    }
}