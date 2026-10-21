using System.Collections.Generic;
using Deckbuilder.Cards;

namespace Deckbuilder.CardPiles
{
    public class DiscardPile : CardPileBase
    {
        #region Virtual Methods
        
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCounter();
        }
        
        #endregion

        
        #region Helper Methods

        public List<CardDataBase> GetAll()
        {
            return new List<CardDataBase>(cardPile);
        }
        
        #endregion
    }
}