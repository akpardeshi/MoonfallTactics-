using System;
using Deckbuilder.Cards;

namespace Deckbuilder.Gameplay
{
    public static class GameEvents
    {
        public static Action<PlayerCard> CardSelected;

        public static Action<BattleResult> BattleCompleted;
    }
}