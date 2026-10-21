using System;

namespace Deckbuilder.Cards
{
    [Serializable]
    public class CardStateMachine
    {
        #region Events
        
        public event Action<CardState, CardState> StateChanged;
        
        #endregion
        
        
        #region Variables
        
        public CardState CurrentState { get; private set; } = CardState.Idle;
        
        #endregion
        
        
        #region Methods

        public bool TryTransition(CardState newState)
        {
            if (!CanTransition(CurrentState, newState))
            {
                return false;
            }

            CardState previousState = CurrentState;
            CurrentState = newState;

            StateChanged?.Invoke(previousState, CurrentState);
            
            return true;
        }

        private bool CanTransition(CardState current, CardState next)
        {
            return current switch
            {
                CardState.Idle =>
                    next is CardState.Hovered,

                CardState.Hovered =>
                    next is CardState.Idle or 
                        CardState.Dragging or 
                        CardState.Targeting,

                CardState.Dragging =>
                    next is CardState.Returning or 
                        CardState.Utilized,

                CardState.Returning =>
                    next == CardState.Idle,

                CardState.Utilized =>
                    next == CardState.Idle,
                
                CardState.Targeting =>
                    next == CardState.Idle,

                _ => false
            };
        }
        
        #endregion
    }
}