using System.Collections.Generic;

namespace TowerDefense.Domain.Core.ValueObjects
{
    public class Health : ValueObject
    {
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; }

        public Health(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public Health(float currentHealth, float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth = System.Math.Max(0, CurrentHealth - damage);
        }

        public void Heal(float amount)
        {
            CurrentHealth = System.Math.Min(MaxHealth, CurrentHealth + amount);
        }

        public bool IsDead => CurrentHealth <= 0;

        public float HealthPercentage => CurrentHealth / MaxHealth;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CurrentHealth;
            yield return MaxHealth;
        }
    }
} 