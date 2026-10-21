using Deckbuilder.TurnSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Deckbuilder.Scripts.Utilities
{
    public class EndTurnButtonActivityHelper: MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            TryGetComponent(out _button);
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

        private void OnPlayerTurnEnded()
        {
            _button.interactable = false;
        }

        private void OnPlayerTurnStarted()
        {
            _button.interactable = true;
        }
    }
}