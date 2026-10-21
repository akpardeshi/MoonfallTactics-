namespace Deckbuilder.Combat
{
    public interface ICombatantActionPresentation
    {
        void PerformAction(ActionId actionId);

        void PlayDamageReaction();

        void PlayDeathReaction();
    }
}