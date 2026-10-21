using System;

namespace Deckbuilder.Health
{
    public interface IDamageable
    {
        event Action Died;
        event Action<int> HealthChanged;

        int Health { get; }
        int MaxHealth { get; }

        void ModifyHealth(int amount);

        bool IsAlive();
    }
}