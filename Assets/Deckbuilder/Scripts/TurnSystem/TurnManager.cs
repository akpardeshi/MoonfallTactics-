using System;
using System.Collections;
using ModularAudio.Core.Scripts.ScriptableEventChannels;
using UnityEngine;
using Deckbuilder.EnemyAI;
using Deckbuilder.Gameplay;

namespace Deckbuilder.TurnSystem
{
    public class TurnManager : MonoBehaviour
    {
        #region Actions

        public static Action PlayerTurnStarted;
        public static Action PlayerTurnEnded;

        #endregion


        #region Variables

        public TurnParticipant CurrentParticipant { get; private set; }

        [SerializeField] private EnemyManager enemyManager;
        
        [SerializeField] private float turnDelayOffset = 0.25f;

        #endregion


        #region Event Functions

        private void Start()
        {
            StartPlayerTurn();
        }

        #endregion


        #region Turn Flow

        private void StartPlayerTurn()
        {
            CurrentParticipant = TurnParticipant.Player;

            PlayerTurnStarted?.Invoke();
        }

        private IEnumerator StartEnemyTurn()
        {
            yield return new WaitForSeconds(GetEnemyTurnDelay());
            CurrentParticipant = TurnParticipant.Enemy;

            enemyManager.StartEnemyTurn();
        }

        public void EndPlayerTurn()
        {
            if (CurrentParticipant != TurnParticipant.Player) return;

            PlayerTurnEnded?.Invoke();
            PlayEndTurnSfx();
            StartCoroutine(StartEnemyTurn());
        }

        public void EndEnemyTurn()
        {
            if (CurrentParticipant != TurnParticipant.Enemy)
                return;

            StartPlayerTurn();
        }
        
        private float GetEnemyTurnDelay()
        {
            return turnDelayOffset * (GameManager.Instance.CardManager.PlayerHand.PlayerHandSize + 1) + turnDelayOffset;
        }
        
        #endregion
        

        #region Audio

        [SerializeField] private AudioEventChannelSo audioChannel;
        [SerializeField] private AudioClip endTurnSfx;

        private void PlayEndTurnSfx()
        {
            audioChannel?.RaiseEvent(endTurnSfx);
        }

        #endregion
    }
}