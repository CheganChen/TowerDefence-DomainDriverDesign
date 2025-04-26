using System;
using System.Collections.Generic;

namespace TowerDefense.Domain.Core.ValueObjects
{
    public class Health : ValueObject
    {
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; }
        public bool IsFull => CurrentHealth >= MaxHealth;
        public event Action<float> HealthChanged;

        public Health(float maxHealth)
        {
            if (maxHealth <= 0)
                throw new ArgumentException("Max health must be greater than 0", nameof(maxHealth));
                
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public Health(float currentHealth, float maxHealth)
        {
            if (maxHealth <= 0)
                throw new ArgumentException("Max health must be greater than 0", nameof(maxHealth));
            if (currentHealth < 0 || currentHealth > maxHealth)
                throw new ArgumentException("Current health must be between 0 and max health", nameof(currentHealth));
                
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }

        public void TakeDamage(float damage)
        {
            if (damage < 0)
                throw new ArgumentException("Damage cannot be negative", nameof(damage));
                
            float previousHealth = CurrentHealth;
            CurrentHealth = Math.Max(0, CurrentHealth - damage);
            
            if (previousHealth != CurrentHealth)
            {
                HealthChanged?.Invoke(CurrentHealth);
            }
        }

        public void Heal(float amount)
        {
            if (amount < 0)
                throw new ArgumentException("Heal amount cannot be negative", nameof(amount));
                
            float previousHealth = CurrentHealth;
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
            
            if (previousHealth != CurrentHealth)
            {
                HealthChanged?.Invoke(CurrentHealth);
            }
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