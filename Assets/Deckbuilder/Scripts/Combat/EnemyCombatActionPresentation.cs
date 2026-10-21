using UnityEngine;

namespace Deckbuilder.Combat
{
    public class EnemyCombatActionPresentation : MonoBehaviour, ICombatantActionPresentation
    {
        #region Variables
        
        private Animator _animator;

        #endregion
        
        
        #region Event Functions
        
        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        #endregion
        
        
        #region Public Functions
        
        public void PerformAction(ActionId actionId)
        {
            _animator.Play("Attack");
        }

        public void PlayDamageReaction()
        {
            _animator.Play("Hurt");
        }

        public void PlayDeathReaction()
        {
            _animator.Play("Death");
        }
        
        #endregion
    }
}