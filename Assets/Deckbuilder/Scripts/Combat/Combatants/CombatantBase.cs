using System;
using System.Collections;
using Deckbuilder.Audio;
using Deckbuilder.Gameplay;
using Deckbuilder.Health;
using Deckbuilder.Scripts.UI;
using Systems.ScreenShake;
using UnityEngine;

namespace Deckbuilder.Combat
{
    public class CombatantBase : MonoBehaviour
    {
        #region Actions

        public event Action<CombatantBase> Died;

        #endregion


        #region Variables

        [Header("Combat Components")] [SerializeField]
        private HealthMonitor healthMonitor;

        [SerializeField] private DamageReceiverBase damageReceiver;
        [SerializeField] private Shield shield;

        public HealthMonitor HealthMonitor => healthMonitor;
        public Shield Shield => shield;

        public int Health => healthMonitor.Health;
        public int MaxHealth => healthMonitor.MaxHealth;
        public bool IsAlive => healthMonitor.IsAlive();

        // Presentation
        private ICombatantActionPresentation _combatantActionPresentation;
        private ICombatantAudioPresentation _combatantAudioPresentation;

        // Screen Shake
        [Header("Screen Shake")] [SerializeField]
        private CameraShakeProfileScriptable deathProfile;

        [SerializeField] private CameraShakeProfileScriptable shieldProfile;
        [SerializeField] private CameraShakeProfileScriptable damageProfile;
        [SerializeField] private ScreenShakeEventChannel screenShakeEventChannel;

        private IScreenShakable _iScreenShakable;

        [Header("Attack Animations")] [SerializeField]
        private float attackDistance = 0.5f;

        [SerializeField] private float attackMoveDuration = 0.12f;
        [SerializeField] private float attackReturnDuration = 0.15f;
        [SerializeField] private float attackAnimationDelay = 0.25f;
        [SerializeField] private bool attackRight;

        [SerializeField] private ParticleSystem particles;

        #endregion


        #region Event Functions

        protected virtual void Awake()
        {
            InitializeComponents();
            SubscribeToHealth();

            _iScreenShakable = new ScreenShakeEffect(screenShakeEventChannel);
        }

        protected virtual void OnDestroy()
        {
            UnsubscribeFromHealth();
        }

        #endregion


        #region Initialization

        private void InitializeComponents()
        {
            if (!healthMonitor) healthMonitor = GetComponent<HealthMonitor>();
            if (!damageReceiver) damageReceiver = GetComponent<DamageReceiverBase>();

            if (!healthMonitor)
            {
                Debug.LogError($"{name} requires a {nameof(HealthMonitor)}.", this);
            }

            if (!damageReceiver)
            {
                Debug.LogError($"{name} requires a {nameof(DamageReceiverBase)}.", this);
            }

            if (!shield)
            {
                TryGetComponent(out shield);
            }

            TryGetComponent(out _combatantActionPresentation);
            TryGetComponent(out _combatantAudioPresentation);
        }

        private void SubscribeToHealth()
        {
            if (healthMonitor) healthMonitor.Died += OnDeath;
        }

        private void UnsubscribeFromHealth()
        {
            if (healthMonitor) healthMonitor.Died -= OnDeath;
        }

        #endregion


        #region Presentation

        private void HandleShieldDamage()
        {
            _combatantAudioPresentation?.PlayShieldBlockSfx();

            _iScreenShakable.RaiseShakeScreenEvent(shieldProfile);
        }

        private void HandleHealthDamage()
        {
            _combatantActionPresentation?.PlayDamageReaction();
            _combatantAudioPresentation?.PlayDamageSfx();

            _iScreenShakable.RaiseShakeScreenEvent(damageProfile);
        }

        #endregion


        #region Health Monitor

        public void Heal(int amount)
        {
            if (!IsAlive)
            {
                return;
            }

            ParticleEffect effect = GameManager.Instance.HealParticleEffectPool.Get();
            
            Vector3 position = transform.position;
            position.y = 0.25f;
            effect.transform.position = position;
            
            HealthMonitor.ModifyHealth(amount);
        }

        public void AddShield(int amount)
        {
            if (!IsAlive)
            {
                return;
            }

            Shield.AddShield(amount);
        }

        #endregion


        #region Combat

        [SerializeField] private float rot;

        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            DamageResultWrapper result = damageReceiver.ApplyDamage(damage);

            switch (result.DamageResult)
            {
                case DamageResult.Damage:
                    ConfigureDamageEffect();

                    HandleHealthDamage();
                    break;

                case DamageResult.Shield:
                    HandleShieldDamage();
                    break;
            }

            DamagePopup popup = GameManager.Instance.DamagePopupPool.Get();
            Vector3 position = transform.position;
            position.y = 1.25f;
            popup.transform.position = position;

            popup.DisplayDamage(result);
            popup.StartTransitions();
        }

        protected virtual void OnDeath()
        {
            Died?.Invoke(this);

            _iScreenShakable.RaiseShakeScreenEvent(deathProfile);
            
            ConfigureDamageEffect();

            _combatantAudioPresentation?.PlayDeathSfx();
            _combatantActionPresentation?.PlayDeathReaction();
        }

        private void ConfigureDamageEffect()
        {
            ParticleEffect effect = GameManager.Instance.DamageParticleEffectPool.Get();
            Vector3 pos = transform.position;

            effect.transform.position = pos;
            effect.Play();

            Quaternion rotation = Quaternion.Euler(0.0f, rot, 0.0f);
            effect.transform.rotation = rotation;
        }

        #endregion


        #region Attack Animation

        public void ExecuteAttackVisual(ActionId actionId)
        {
            StartCoroutine(PerformAttack(actionId));
        }

        private IEnumerator PerformAttack(ActionId actionId)
        {
            Vector3 originalPosition = transform.position;
            Vector3 attackPosition = GetAttackPosition(originalPosition);

            yield return MoveToPosition(attackPosition, attackMoveDuration);

            _combatantActionPresentation?.PerformAction(actionId);

            yield return new WaitForSeconds(attackAnimationDelay);
            yield return MoveToPosition(originalPosition, attackReturnDuration);
        }

        private Vector3 GetAttackPosition(Vector3 originalPosition)
        {
            float direction = attackRight ? 1f : -1f;
            originalPosition.x += direction * attackDistance;
            return originalPosition;
        }

        private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            transform.position = targetPosition;
        }

        #endregion
    }
}