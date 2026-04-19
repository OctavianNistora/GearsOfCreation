namespace DefaultNamespace
{
    public abstract class BaseCombatModifier
    {
        protected BaseEntity Target;
        protected int TurnsRemaining;

        protected BaseCombatModifier(BaseEntity target, int duration)
        {
            Target = target;
            TurnsRemaining = duration;
        }
        
        public void ExtendDuration(int additionalTurns)
        {
            TurnsRemaining += additionalTurns;
        }
        
        public void RoundEnded()
        {
            TurnsRemaining--;

            if (TurnsRemaining > 0) return;
            
            Target.RemoveModifier(this);
            CombatManager.Instance.OnCombatRoundEnd -= RoundEnded;
        }
    }
}