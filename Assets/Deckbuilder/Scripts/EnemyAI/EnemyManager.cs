using System.Collections;
using System.Collections.Generic;
using Deckbuilder.Combat;
using Deckbuilder.Gameplay;
using Deckbuilder.TurnSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Deckbuilder.EnemyAI
{
    public class EnemyManager : MonoBehaviour
    {
        #region Variables

        [SerializeField] private TurnManager turnManager;
        [SerializeField] private List<Enemy> enemies;

        public List<Enemy> Enemies => enemies;

        #endregion


        #region Event Functions

        private void OnEnable()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Died += OnEnemyDied;
            }
        }

        private void OnDisable()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Died -= OnEnemyDied;
            }
        }

        #endregion

        
        #region Event Functions
        
        private void OnEnemyDied(CombatantBase combatant)
        {
            Enemy enemy = combatant as Enemy;

            if (enemy == null)
            {
                return;
            }

            enemy.Died -= OnEnemyDied;

            enemies.Remove(enemy);

            StartCoroutine(DisableEnemyAfterSomeTime(enemy.gameObject));

            if (enemies.Count == 0)
            {
                GameEvents.BattleCompleted?.Invoke(BattleResult.Victory);
            }
        }

        #endregion
        
        
        #region Turn Management Functions
        
        public void StartEnemyTurn()
        {
            ResetEnemyShield();
            StartCoroutine(ExecuteEnemyTurn());
        }

        private IEnumerator ExecuteEnemyTurn()
        {
            WaitForSeconds waitForSeconds = new(1.0f);
            List<Enemy> enemies = GetEnemiesInRandomOrder();

            foreach (Enemy enemy in enemies)
            {
                enemy.PerformRandomAbility();

                yield return waitForSeconds;
            }

            yield return waitForSeconds;

            turnManager.EndEnemyTurn();
        }

        private List<Enemy> GetEnemiesInRandomOrder()
        {
            List<Enemy> randomizedEnemies = new(enemies);

            for (int i = randomizedEnemies.Count - 1; i > 0; i--)
            {
                int index = Random.Range(0, i + 1);

                (randomizedEnemies[i], randomizedEnemies[index]) =
                    (randomizedEnemies[index], randomizedEnemies[i]);
            }

            return randomizedEnemies;
        }

        private IEnumerator DisableEnemyAfterSomeTime(GameObject enemy)
        {
            yield return new WaitForSeconds(0.99f);
            enemy.SetActive(false);
        }
        
        #endregion
        
        
        #region Enemy State Management
        
        private void ResetEnemyShield()
        {
            foreach (var enemy in enemies)
            {
                enemy.Shield.RemoveShield(enemy.Shield.CurrentShield);
            }
        }
        
        #endregion
    }
}