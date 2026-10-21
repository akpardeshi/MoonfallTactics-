using System.Collections.Generic;
using Deckbuilder.Combat;
using UnityEngine;

namespace Deckbuilder.Cards
{
    [CreateAssetMenu(fileName = "Shield Card Data", menuName = "Deckbuilder/Cards/ShieldCard", order = 2)]
    public class ShieldCardData : CardDataBase
    {
        [SerializeField] private int shieldAmount = 5;
        
        public override void PlayCard(List<CombatantBase> targets)
        {
            targets[0].AddShield(shieldAmount);
        }
    }
}