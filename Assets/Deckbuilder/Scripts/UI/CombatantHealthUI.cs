using Deckbuilder.Combat;
using TMPro;
using UnityEngine;

namespace Deckbuilder.Cards
{
    public class CombatantHealthUI : MonoBehaviour
    {
        #region Variables
        
        private int _maxHealth;

        [SerializeField] private CombatantBase combatant;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI shieldText;
        
        #endregion
        
        
        #region Event Functions

        private void Awake()
        {
            _maxHealth = combatant.MaxHealth;
        }
        
        private void OnEnable()
        {
            combatant.HealthMonitor.HealthChanged += OnHealthChanged;
            combatant.Shield.ShieldChanged += OnShieldChanged;
        }

        private void Start()
        {
            OnHealthChanged(combatant.Health);
            OnShieldChanged(combatant.Shield.CurrentShield);
        }

        private void OnDisable()
        {
            combatant.HealthMonitor.HealthChanged -= OnHealthChanged;
            combatant.Shield.ShieldChanged -= OnShieldChanged;
        }
        
        #endregion
        
        
        #region Event Handlers

        private void OnHealthChanged(int health)
        {
            healthText.text = $"{health} / {_maxHealth}";
        }

        private void OnShieldChanged(int shield)
        {
            shieldText.text = $"{shield}";
        }
        
        #endregion
    }
}