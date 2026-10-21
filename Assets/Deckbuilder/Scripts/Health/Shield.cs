using System;
using UnityEngine;

namespace Deckbuilder.Health
{
    public class Shield : MonoBehaviour, IDamageInterceptor
    {
        [SerializeField]
        private int currentShield;

        public int CurrentShield => currentShield;
        
        public event Action<int> ShieldChanged; 

        public void AddShield(int amount)
        {
            if (amount <= 0)
                return;

            currentShield += amount;
            
            ShieldChanged?.Invoke(currentShield);
        }

        public void RemoveShield(int amount)
        {
            if (amount <= 0)
                return;

            currentShield = Mathf.Max(0, currentShield - amount);
            
            ShieldChanged?.Invoke(currentShield);
        }

        public bool TryInterceptDamage(ref int damage)
        {
            if (currentShield <= 0 || damage <= 0)
                return false;

            int absorbedDamage = Mathf.Min(currentShield, damage);

            currentShield -= absorbedDamage;
            damage -= absorbedDamage;

            ShieldChanged?.Invoke(currentShield);
            return true;
        }
    }
}