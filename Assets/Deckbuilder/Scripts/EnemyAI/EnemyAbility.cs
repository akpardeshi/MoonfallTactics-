using System;
using Deckbuilder.Combat;

namespace Deckbuilder.EnemyAI
{
    [Serializable]
    public class EnemyAbility
    {
        public EnemyAbilityType Type;
        public EnemyTargetType TargetType;
        public int Amount;

        public ActionId ActionId;

        public void ExecuteAbility(CombatantBase target)
        {
            switch (Type)
            {
                case EnemyAbilityType.Attack:
                    target.TakeDamage(Amount);
                    break;
                
                case EnemyAbilityType.Shield:
                    target.AddShield(Amount);
                    break;
                
                case EnemyAbilityType.Heal:
                    target.Heal(Amount);
                    break;
            }
        }
    }
}