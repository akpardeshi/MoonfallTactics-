using Deckbuilder.Combat;
using UnityEngine;

namespace Deckbuilder.Health
{
    public abstract class DamageReceiverBase : MonoBehaviour
    {
        protected CombatantBase Combatant { get; private set; }

        protected virtual void Awake()
        {
            Combatant = GetComponent<CombatantBase>();
        }

        public abstract DamageResultWrapper ApplyDamage(int damage);
    }
}