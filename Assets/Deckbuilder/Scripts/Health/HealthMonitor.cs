using System;
using UnityEngine;

namespace Deckbuilder.Health
{
    public class HealthMonitor : MonoBehaviour
    {
        [field: SerializeField]
        public int MaxHealth { get; private set; } = 50;

        public int Health { get; private set; }

        public event Action Died;
        public event Action<int> HealthChanged;

        private bool _hasDied;

        private void Awake()
        {
            InitializeHealth();
        }

        public void ModifyHealth(int amount)
        {
            if (_hasDied)
                return;

            int previousHealth = Health;

            Health = Mathf.Clamp(
                Health + amount,
                0,
                MaxHealth);

            if (Health == previousHealth)
                return;

            HealthChanged?.Invoke(Health);

            if (!IsAlive())
            {
                HandleDeath();
            }
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        private void HandleDeath()
        {
            if (_hasDied)
                return;

            _hasDied = true;
            Died?.Invoke();
        }

        private void InitializeHealth()
        {
            Health = MaxHealth;
            _hasDied = false;
        }
    }
}