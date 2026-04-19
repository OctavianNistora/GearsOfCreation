namespace DefaultNamespace
{
    public class DamageReceivedModifier : BaseCombatModifier
    {
        protected float DamageMultiplier;
        
        public DamageReceivedModifier(BaseEntity target, int duration, float damageMultiplier) : base(target, duration)
        {
            DamageMultiplier = damageMultiplier;
        }

        public float Apply(float damage)
        {
            return damage * DamageMultiplier;
        }
    }
}