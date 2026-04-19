namespace DefaultNamespace
{
    public class DamageDealtModifier : BaseCombatModifier
    {
        protected float DamageMultiplier;
        
        public DamageDealtModifier(BaseEntity target, int duration, float damageMultiplier) : base(target, duration)
        {
            DamageMultiplier = damageMultiplier;
        }

        public float Apply(float damage)
        {
            return damage * DamageMultiplier;
        }
    }
}