using System.Collections.Generic;

namespace TowerDefense.Domain.Core.ValueObjects
{
    public class Damage : ValueObject
    {
        public float BaseDamage { get; }
        public float CriticalMultiplier { get; }
        public bool IsCritical { get; private set; }

        public Damage(float baseDamage, float criticalMultiplier = 1.5f)
        {
            BaseDamage = baseDamage;
            CriticalMultiplier = criticalMultiplier;
            IsCritical = false;
        }

        public float CalculateFinalDamage()
        {
            return IsCritical ? BaseDamage * CriticalMultiplier : BaseDamage;
        }

        public void SetCritical(bool isCritical)
        {
            IsCritical = isCritical;
        }

        public static Damage operator +(Damage a, Damage b)
        {
            return new Damage(a.BaseDamage + b.BaseDamage, a.CriticalMultiplier);
        }

        public static Damage operator *(Damage damage, float multiplier)
        {
            return new Damage(damage.BaseDamage * multiplier, damage.CriticalMultiplier);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BaseDamage;
            yield return CriticalMultiplier;
            yield return IsCritical;
        }
    }
} 