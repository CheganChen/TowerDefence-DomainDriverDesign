using System;
using Domain.Core.Services;
using TowerDefense.Domain.Core.ValueObjects;

namespace TowerDefense.Domain.Enemy.Events
{
    public class EnemyDamagedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public float Damage { get; }
        public float RemainingHealth { get; }
        public int Version { get; } = 1;

        public EnemyDamagedEvent(Guid enemyId, float damage, float remainingHealth)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Damage = damage;
            RemainingHealth = remainingHealth;
        }
    }

    public class EnemyMovedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public Position Position { get; }
        public float DistanceTraveled { get; }
        public int Version { get; } = 1;

        public EnemyMovedEvent(Guid enemyId, Position position, float distanceTraveled)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Position = position;
            DistanceTraveled = distanceTraveled;
        }
    }

    public class EnemyDiedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public Cost Reward { get; }
        public int Version { get; } = 1;

        public EnemyDiedEvent(Guid enemyId, Cost reward)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Reward = reward;
        }
    }
} 