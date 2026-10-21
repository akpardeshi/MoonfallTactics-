using System.Collections.Generic;
using Deckbuilder.Combat;
using UnityEngine;

namespace Deckbuilder.Cards
{
    [CreateAssetMenu(fileName = "Multi Attack", menuName = "Deckbuilder/Cards/MultiAttackCard", order = 0)]
    public class MultiTargetAttack : CardDataBase
    {
        #region Variables

        [SerializeField] private ActionId actionId;
        public ActionId ActionId => actionId;

        [SerializeField] private int damage;

        #endregion


        #region Methods

        public override void PlayCard(List<CombatantBase> targets)
        {
            foreach (var target in targets)
            {
                target.TakeDamage(damage);
            }
        }

        #endregion
    }
}