using System.Collections.Generic;
using Deckbuilder.EnemyAI;
using UnityEngine;

namespace Deckbuilder.Combat
{
    public class Enemy : CombatantBase
    {
        #region Variables

        [SerializeField] private Player player;

        [SerializeField] private List<EnemyAbility> abilities;

        #endregion


        #region Event Functions

        private void Start()
        {
            Died += OnDied;
        }

        #endregion


        #region Combat Actions

        private void OnDied(CombatantBase combatant)
        {
        }

        public void PerformRandomAbility()
        {
            EnemyAbility ability = GetRandomAbility();
            CombatantBase target = ResolveTarget(ability.TargetType);

            ability.ExecuteAbility(target);

            if (ability.Type == EnemyAbilityType.Attack)
            {
                ExecuteAttackVisual(ability.ActionId);
            }
        }

        private EnemyAbility GetRandomAbility()
        {
            EnemyAbility ability;

            if (Health <= MaxHealth / 5)
            {
                ability = abilities.Find(x => x.Type == EnemyAbilityType.Heal);
            }

            else
            {
                ability = abilities[Random.Range(1, abilities.Count)];
            }

            return ability;
        }

        #endregion


        #region Targeting

        private CombatantBase ResolveTarget(EnemyTargetType targetType)
        {
            switch (targetType)
            {
                case EnemyTargetType.Self:
                    return this;

                case EnemyTargetType.Player:
                    return player;

                default:
                    return null;
            }
        }

        #endregion
    }
}