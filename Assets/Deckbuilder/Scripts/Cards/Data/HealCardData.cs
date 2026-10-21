using System.Collections.Generic;
using Deckbuilder.Combat;
using UnityEngine;

namespace Deckbuilder.Cards
{
    [CreateAssetMenu(fileName = "Health Card Data", menuName = "Deckbuilder/Cards/Health Card", order = 1)]
    public class HealCardData : CardDataBase
    {
        [SerializeField] private int healAmount = 5;

        public override void PlayCard(List<CombatantBase> targets)
        {
            targets[0].Heal(healAmount);
        }
    }
}