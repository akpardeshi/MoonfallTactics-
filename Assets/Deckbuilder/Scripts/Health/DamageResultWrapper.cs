namespace Deckbuilder.Health
{
    public struct DamageResultWrapper
    {
        public readonly DamageResult DamageResult;
        public readonly int HealthDamageDealt;

        public DamageResultWrapper(DamageResult damageResult, int healthDamageDealt)
        {
            DamageResult =  damageResult;
            HealthDamageDealt = healthDamageDealt;
        }
    }
}