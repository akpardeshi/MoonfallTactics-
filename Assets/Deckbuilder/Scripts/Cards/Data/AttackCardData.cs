using System.Collections.Generic;
using Deckbuilder.Combat;
using UnityEngine;

namespace Deckbuilder.Cards
{
    [CreateAssetMenu(fileName = "Attack Base", menuName = "Deckbuilder/Cards/AttackCard", order = 0)]
    public class AttackCardData : CardDataBase
    {
        #region Variables
        
        [SerializeField] private ActionId actionId;
        public ActionId ActionId => actionId;
        
        [SerializeField] private int damage;
        
        #endregion
        
        
        #region Methods
        
        public override void PlayCard(List<CombatantBase> targets)
        {
            targets[0].TakeDamage(damage);
        }
        
        #endregion
    }
}