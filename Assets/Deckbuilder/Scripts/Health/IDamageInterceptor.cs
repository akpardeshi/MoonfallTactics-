namespace Deckbuilder.Health
{
    public interface IDamageInterceptor
    {
        bool TryInterceptDamage(ref int damage);
    }
}