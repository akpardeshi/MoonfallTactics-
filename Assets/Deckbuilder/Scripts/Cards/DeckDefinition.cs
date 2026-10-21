namespace Deckbuilder.Cards
{
    [System.Serializable]
    public struct DeckDefinition
    {
        public string CardID;
        public int CardCount;

        public DeckDefinition(string cardID, int cardCount)
        {
            CardID = cardID;
            CardCount = cardCount;
        }
    }
}