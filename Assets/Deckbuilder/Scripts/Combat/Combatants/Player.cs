using System;
using Deckbuilder.Gameplay;
using Deckbuilder.TurnSystem;

namespace Deckbuilder.Combat
{
    public class Player: CombatantBase
    {
        #region Event Functions

        private void OnEnable()
        {
            Died += OnDied;
            TurnManager.PlayerTurnStarted += OnPlayerTurnStarted;
        }

        private void OnDisable()
        {
            Died -= OnDied;
            TurnManager.PlayerTurnStarted -= OnPlayerTurnStarted;
        }

        #endregion
        
        
        #region Combat Events

        private void OnDied(CombatantBase combatant)
        {
            GameEvents.BattleCompleted?.Invoke(BattleResult.Defeat);
        }

        private void OnPlayerTurnStarted()
        {
            Shield.RemoveShield(Shield.CurrentShield);
        }
        
        #endregion
    }
}