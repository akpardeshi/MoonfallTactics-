using System.Collections.Generic;
using Deckbuilder.Cards;
using TMPro;
using UnityEngine;

namespace Deckbuilder.CardPiles
{
    public class CardPileBase : MonoBehaviour
    {
        #region Variables

        [SerializeField] private TextMeshProUGUI counterText;
        [SerializeField] protected List<CardDataBase> cardPile;

        public List<CardDataBase> CardPile => cardPile;

        #endregion


        #region Unity Event Functions

        protected virtual void OnEnable()
        {
            UpdateCounter();
        }

        #endregion


        #region Virtual Methods

        public virtual void ResetPile()
        {
            cardPile.Clear();
            UpdateCounter();
        }

        #endregion


        #region Custom Methods

        public void Add(CardDataBase cardDataBase)
        {
            cardPile.Add(cardDataBase);
            UpdateCounter();
        }

        public virtual void Add(List<CardDataBase> cardsDataBase)
        {
            cardPile.AddRange(cardsDataBase);
            UpdateCounter();
        }

        protected void UpdateCounter()
        {
            if (!counterText) return;

            counterText.text = $"{cardPile.Count}";
        }

        public void Remove(CardDataBase cardDataBase)
        {
            if (cardPile.Remove(cardDataBase))
            {
                UpdateCounter();
            }
        }

        public List<CardDataBase> Draw(int cardsCount)
        {
            List<CardDataBase> cards = new();
            int availableCardsCount = Mathf.Min(cardsCount, cardPile.Count);

            CardDataBase card;

            for (int i = 0; i < availableCardsCount; i++)
            {
                card = cardPile[^1];
                cardPile.RemoveAt(cardPile.Count - 1);
                cards.Add(card);
            }

            UpdateCounter();
            return cards;
        }

        #endregion
    }
}