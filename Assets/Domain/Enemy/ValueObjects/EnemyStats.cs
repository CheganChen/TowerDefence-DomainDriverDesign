using System.Collections.Generic;
using TowerDefense.Domain.Core.ValueObjects;

namespace TowerDefense.Domain.Enemy.ValueObjects
{
    public class EnemyStats : ValueObject
    {
        public Health Health { get; }
        public float Speed { get; }
        public Cost Reward { get; }
        public float Size { get; }

        public EnemyStats(Health health, float speed, Cost reward, float size = 1f)
        {
            Health = health;
            Speed = speed;
            Reward = reward;
            Size = size;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Health;
            yield return Speed;
            yield return Reward;
            yield return Size;
        }
    }
} 