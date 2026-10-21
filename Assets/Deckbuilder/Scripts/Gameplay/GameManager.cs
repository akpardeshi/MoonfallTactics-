using System.Collections;
using Deckbuilder.Cards;
using Deckbuilder.Combat;
using Deckbuilder.EnemyAI;
using Deckbuilder.Scripts.UI;
using Deckbuilder.TurnSystem;
using Deckbuilder.UI;
using UnityEngine;
using UnityEngine.Pool;

namespace Deckbuilder.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        #region Variables

        [SerializeField] private Player player;

        public Player Player => player;

        public static GameManager Instance;

        [SerializeField] private Energy energy;
        public Energy Energy => energy;

        [SerializeField] private TurnManager turnManager;
        public TurnManager TurnManager => turnManager;

        [SerializeField] private RectTransform drawPileLocation;

        [SerializeField] private RectTransform playerHandLocation;

        [SerializeField] private TargetingController targetingController;
        public TargetingController TargetingController => targetingController;

        [SerializeField] private RectTransform targetingControllerRect;
        public RectTransform TargetingControllerRect => targetingControllerRect;

        [SerializeField] private CardManager cardManager;

        public CardManager CardManager => cardManager;

        [SerializeField] private EnemyManager enemyManager;
        public EnemyManager EnemyManager => enemyManager;

        #endregion


        #region Event Functions

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            GameEvents.BattleCompleted += BattleCompletedResults;
        }

        private void Start()
        {
            InitializeDamagePopupPool();
            InitializeDamageParticleEffectPool();
            InitializeHealParticleEffectPool();
        }

        private void OnDisable()
        {
            GameEvents.BattleCompleted -= BattleCompletedResults;
        }

        #endregion


        #region Gameplay

        [SerializeField] private BattleResultScreen battleResultScreen;

        private void BattleCompletedResults(BattleResult battleResult)
        {
            StartCoroutine(OnBattleCompleted(battleResult));
        }

        private IEnumerator OnBattleCompleted(BattleResult battleResult)
        {
            yield return new WaitForSeconds(2.0f);

            switch (battleResult)
            {
                case BattleResult.Victory:
                    battleResultScreen.ShowVictory();
                    break;

                case BattleResult.Defeat:
                    battleResultScreen.ShowDefeat();
                    break;
            }
        }

        #endregion


        #region Pooling

        [SerializeField] private int initialPoolSize;
        [SerializeField] private int maxPoolSize;

        public IObjectPool<DamagePopup> DamagePopupPool;

        [SerializeField] private DamagePopup damagePopupPrefab;
        [SerializeField] private bool damagePopupCollectionChecks;

        private void InitializeDamagePopupPool()
        {
            DamagePopupPool = new ObjectPool<DamagePopup>(CreateDamagePopup, GetDamagePopup, ReleaseDamagePopup,
                DestroyDamagePopup,
                damagePopupPrefab, initialPoolSize,
                maxPoolSize);
        }

        private DamagePopup CreateDamagePopup()
        {
            DamagePopup damagePopup = Instantiate(damagePopupPrefab);
            damagePopup.ObjectPool = DamagePopupPool;
            return damagePopup;
        }

        private void GetDamagePopup(DamagePopup damagePopup)
        {
            damagePopup.Activate();
        }

        private void ReleaseDamagePopup(DamagePopup damagePopup)
        {
            damagePopup.Deactivate();
        }

        private void DestroyDamagePopup(DamagePopup damagePopup)
        {
            if (!damagePopup) return;

            Destroy(damagePopup.gameObject);
        }

        // Particle system
        public ObjectPool<ParticleEffect> DamageParticleEffectPool { get; private set; }

        [SerializeField] private ParticleEffect particleEffectPrefab;

        private void InitializeDamageParticleEffectPool()
        {
            DamageParticleEffectPool = new ObjectPool<ParticleEffect>(CreateDamageParticleEffect,
                GetDamageParticleEffect, ReleaseDamageParticleEffect, DestroyDamageParticleEffect,
                particleEffectPrefab, initialPoolSize,
                maxPoolSize);
        }

        private ParticleEffect CreateDamageParticleEffect()
        {
            ParticleEffect particleEffect = Instantiate(particleEffectPrefab);
            return particleEffect;
        }

        private void GetDamageParticleEffect(ParticleEffect particleEffect)
        {
        }

        private void ReleaseDamageParticleEffect(ParticleEffect particleEffect)
        {
            particleEffect.Stop();
        }

        private void DestroyDamageParticleEffect(ParticleEffect particleEffect)
        {
            if (!particleEffect) return;

            Destroy(particleEffect.gameObject);
        }

        public ObjectPool<ParticleEffect> HealParticleEffectPool { get; private set; }

        [SerializeField] private ParticleEffect healParticleEffectPrefab;

        private void InitializeHealParticleEffectPool()
        {
            HealParticleEffectPool = new ObjectPool<ParticleEffect>(CreateHealParticleEffect, GetHealParticleEffect,
                ReleaseHealParticleEffect, DestroyHealParticleEffect,
                healParticleEffectPrefab, initialPoolSize,
                maxPoolSize);
        }

        private ParticleEffect CreateHealParticleEffect()
        {
            ParticleEffect healParticleEffect = Instantiate(healParticleEffectPrefab);
            return healParticleEffect;
        }

        private void GetHealParticleEffect(ParticleEffect healParticleEffect)
        {
        }

        private void ReleaseHealParticleEffect(ParticleEffect healParticleEffect)
        {
            healParticleEffect.Stop();
        }

        private void DestroyHealParticleEffect(ParticleEffect healParticleEffect)
        {
            if (!healParticleEffect) return;

            Destroy(healParticleEffect.gameObject);
        }

        public GameManager(ObjectPool<ParticleEffect> healParticleEffectPool,
            ObjectPool<ParticleEffect> damageParticleEffectPool)
        {
            HealParticleEffectPool = healParticleEffectPool;
            DamageParticleEffectPool = damageParticleEffectPool;
        }

        #endregion


        #region Pause

        [SerializeField] private Canvas pausePanel;

        public void Pause()
        {
            pausePanel.enabled = true;
            Time.timeScale = 0;
        }

        #endregion
    }
}