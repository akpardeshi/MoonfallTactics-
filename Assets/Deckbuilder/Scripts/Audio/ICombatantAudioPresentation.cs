using Deckbuilder.Combat;

namespace Deckbuilder.Audio
{
    public interface ICombatantAudioPresentation
    {
        void PlayAttackSfx(ActionId actionId);
        void PlayDamageSfx();
        void PlayShieldBlockSfx();
        void PlayDeathSfx();
    }
}