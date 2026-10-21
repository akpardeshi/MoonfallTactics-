using System.Collections.Generic;
using Deckbuilder.Combat;
using UnityEngine;

namespace Deckbuilder.Cards
{
    public abstract class CardDataBase : ScriptableObject
    {
        
        [SerializeField] private string cardID;
        public string CardID => cardID;
        
        [SerializeField] private bool needsTargeting;
        
        public bool NeedsTargeting => needsTargeting;
        
        [SerializeField] private CardType cardType;
        public CardType CardType => cardType;
        
        [SerializeField] private string cardName;
        
        public string CardName => cardName;
        
        [SerializeField] private Sprite cardIcon;
        
        public Sprite CardIcon => cardIcon;
        
        [SerializeField] private string cardDescription;
        
        public string CardDescription => cardDescription;

        [SerializeField] private int energyNeeded;
        
        public int EnergyNeeded => energyNeeded;

        public abstract void PlayCard(List<CombatantBase> targets);

        [SerializeField] 
        private bool isExhaustCard;
        public bool IsExhaustCard => isExhaustCard;
        
        [SerializeField] private Color accentColor;
        public Color AccentColor => accentColor;
        
        [SerializeField] private Color headerColor;
        public Color HeaderColor => headerColor;
    }
}