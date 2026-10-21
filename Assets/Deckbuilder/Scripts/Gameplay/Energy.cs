using Deckbuilder.TurnSystem;
using TMPro;
using UnityEngine;

namespace Deckbuilder.Gameplay
{
    public class Energy : MonoBehaviour
    {
        #region Variables

        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private int maxEnergy = 10;

        private int _currentEnergy;

        #endregion


        #region Event Functions
        
        private void OnEnable()
        {
            TurnManager.PlayerTurnStarted += OnPlayerTurnStarted;
        }

        private void Start()
        {
            ResetCurrentEnergy(0);
            UpdateVisuals();
        }
        
        private void OnDisable()
        {
            TurnManager.PlayerTurnStarted -= OnPlayerTurnStarted;
        }

        #endregion
        
        
        #region Event Handlers
        
        private void OnPlayerTurnStarted()
        {
            ResetCurrentEnergy(0);
        }
        
        #endregion
        

        #region Energy Managements

        public void SetMaxEnergy(int value)
        {
            maxEnergy = value;
        }

        private void ResetCurrentEnergy(int modifier)
        {
            _currentEnergy = maxEnergy;
            _currentEnergy += modifier;

            UpdateVisuals();
        }

        public bool HasEnergy(int value)
        {
            return _currentEnergy >= value;
        }

        public void ConsumeEnergy(int value)
        {
            _currentEnergy -= value;
            UpdateVisuals();
        }

        private void UpdateCurrentEnergy(int value)
        {
            _currentEnergy += value;
        }
        
        #endregion
        
        
        #region UI Functions
        
        private void UpdateVisuals()
        {
            energyText.text = $"{_currentEnergy} / {maxEnergy}";
        }
        
        #endregion
    }
}