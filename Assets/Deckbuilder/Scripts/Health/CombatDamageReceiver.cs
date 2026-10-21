namespace Deckbuilder.Health
{
    public class CombatDamageReceiver : DamageReceiverBase
    {
        private IDamageInterceptor[] _interceptors;

        protected override void Awake()
        {
            base.Awake();

            _interceptors = GetComponents<IDamageInterceptor>();
        }

        public override DamageResultWrapper ApplyDamage(int damage)
        {
            if (damage <= 0)
            {
                return new DamageResultWrapper(DamageResult.None, 0);
            }

            int remainingDamage = damage;

            foreach (var interceptor in _interceptors)
            {
                interceptor.TryInterceptDamage(ref remainingDamage);

                if (remainingDamage <= 0)
                {
                     return new DamageResultWrapper(DamageResult.Shield, 0);
                }
            }

            Combatant.HealthMonitor.ModifyHealth(-remainingDamage);

            DamageResult result = !Combatant.IsAlive ? DamageResult.Death : DamageResult.Damage;

            DamageResultWrapper resultWrapper = new(result, remainingDamage);
            return resultWrapper;
        }
    }
}