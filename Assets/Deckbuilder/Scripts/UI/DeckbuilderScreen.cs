using System.Collections.Generic;
using System.Linq;
using Deckbuilder.Cards;
using Deckbuilder.DeckBuilding;
using Deckbuilder.Gameplay;
using ModularForge.Trailback.Core;
using ModularForge.Trailback.Demo.UI;
using UnityEngine;

namespace Deckbuilder.Scripts.UI
{
    public class DeckbuilderScreen : UIBase
    {
        #region Variables

        private const string PlayerDeckKey = "PlayerDeck";

        [SerializeField] private DeckDefinitionWrapper playerDeckDefinitionWrapper;
        [SerializeField] private DeckDefinitionWrapper defaultDeckDefinitionWrapper;

        [SerializeField] private PlayerCard playerCardPrefab;

        [SerializeField] private CardConfigWindow cardConfigWindowPrefab;
        [SerializeField] private Transform cardConfigWindowParent;

        [SerializeField] private List<CardDataBase> cardDataBase;

        private Canvas _deckBuilderCanvas;

        private readonly List<CardConfigWindow> _cardConfigWindows = new();
        private readonly Dictionary<CardDataBase, int> _deckConfig = new();

        #endregion


        #region Event Functions

        protected override void Awake()
        {
            base.Awake();

            if (PlayerPrefs.HasKey(PlayerDeckKey))
            {
                string key = PlayerPrefs.GetString(PlayerDeckKey);
                playerDeckDefinitionWrapper = JsonUtility.FromJson<DeckDefinitionWrapper>(key);
                GameData.PlayerDeck = playerDeckDefinitionWrapper;
            }

            else
            {
                GameData.PlayerDeck = defaultDeckDefinitionWrapper;
                playerDeckDefinitionWrapper = defaultDeckDefinitionWrapper;
            }

            cardDataBase = new();
            cardDataBase = Resources.LoadAll<CardDataBase>("Cards").ToList();

            TryGetComponent(out _deckBuilderCanvas);
        }

        private void Start()
        {
            foreach (var cardData in cardDataBase)
            {
                var amount = playerDeckDefinitionWrapper.DeckDefinitions.Find(x => x.CardID == cardData.CardID)
                    .CardCount;

                var cardConfigWindow = Instantiate(cardConfigWindowPrefab, cardConfigWindowParent);
                cardConfigWindow.OnCardEquippedCountChanged += UpdateCardAmount;
                var card = Instantiate(playerCardPrefab, cardConfigWindow.CardParent);
                card.SetCard(cardData);

                cardConfigWindow.SetExistingCardAmount(amount);

                cardConfigWindow.CardDataBase = cardData;
                _deckConfig.Add(cardData, amount);

                _cardConfigWindows.Add(cardConfigWindow);
            }
        }

        private void OnDisable()
        {
            foreach (var cardConfigWindow in _cardConfigWindows)
            {
                cardConfigWindow.OnCardEquippedCountChanged -= UpdateCardAmount;
            }
        }

        #endregion


        #region UI Functions

        private void UpdateCardAmount(CardDataBase cardDataBase, int count)
        {
            if (_deckConfig.TryGetValue(cardDataBase, out int _currentCount))
            {
                _deckConfig[cardDataBase] = count;
            }
        }

        #endregion


        #region Helper Functions

        private void SaveDeckConfig()
        {
            _deckBuilderCanvas.enabled = false;

            List<DeckDefinition> deckDefinition = new();

            if (_deckConfig.Count > 0)
            {
                foreach (var deck in _deckConfig)
                {
                    var deckDef = new DeckDefinition(deck.Key.CardID, deck.Value);
                    deckDefinition.Add(deckDef);
                }

                playerDeckDefinitionWrapper = new()
                {
                    DeckDefinitions = deckDefinition
                };

                string json = JsonUtility.ToJson(playerDeckDefinitionWrapper);
                PlayerPrefs.SetString(PlayerDeckKey, json);
                PlayerPrefs.Save();

                GameData.PlayerDeck = playerDeckDefinitionWrapper;
            }

            else
            {
                GameData.PlayerDeck = defaultDeckDefinitionWrapper;
            }
        }

        #endregion


        #region Inherited Methods

        public override void Back()
        {
            SaveDeckConfig();

            base.Back();
            TrailbackFacade.Back();
        }

        #endregion
    }
}