using System;
using Deckbuilder.Cards;
using TMPro;
using UnityEngine;

namespace Deckbuilder.DeckBuilding
{
    public class CardConfigWindow : MonoBehaviour
    {
        #region Actions

        public Action<CardDataBase, int> OnCardEquippedCountChanged;

        #endregion


        #region Variables
        
        private int _equippedCount;

        [SerializeField] private TextMeshProUGUI equippedCountText;
        [SerializeField] private Transform cardParent;

        public Transform CardParent => cardParent;
        public CardDataBase CardDataBase;

        #endregion
        
        
        #region Event Functions
        
        private void Awake()
        {
            _equippedCount = 0;
            UpdateCounter();
        }
        
        #endregion


        #region UI Methods

        public void Increase()
        {
            _equippedCount++;
            _equippedCount = Mathf.Clamp(_equippedCount, 0, 10);

            OnCardEquippedCountChanged?.Invoke( CardDataBase, _equippedCount);
            UpdateCounter();
        }

        public void Decrease()
        {
            _equippedCount--;
            _equippedCount = Mathf.Clamp(_equippedCount, 0, 10);

            OnCardEquippedCountChanged?.Invoke(CardDataBase, _equippedCount);
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            equippedCountText.text = $"{_equippedCount}";
        }
        
        #endregion


        #region Counters

        public void SetExistingCardAmount(int amount)
        {
            _equippedCount = amount;
            
            UpdateCounter();
        }
        
        #endregion
    }
}