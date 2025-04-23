using System;
using Domain.Core.Services;
using TowerDefense.Domain.Core.Entities.Base;
using TowerDefense.Domain.Core.ValueObjects;
using TowerDefense.Domain.Enemy.ValueObjects;

namespace TowerDefense.Domain.Enemy.Entities
{
    public class EnemyEntity : Entity
    {
        public EnemyType Type { get; private set; }
        public Position Position { get; private set; }
        public EnemyStats Stats { get; private set; }
        public float DistanceTraveled { get; private set; }
        public bool IsDead { get; private set; }

        public EnemyEntity(EnemyType type, Position position, EnemyStats stats)
        {
            Type = type;
            Position = position;
            Stats = stats;
            DistanceTraveled = 0f;
            IsDead = false;
        }

        public void Move(Position newPosition, float distance)
        {
            Position = newPosition;
            DistanceTraveled += distance;
            EventBus.instance.AddDomainEvent(new EnemyMovedEvent(Id, Position, DistanceTraveled));
        }

        public void TakeDamage(float damage)
        {
            if (IsDead)
                return;

            Stats.Health.TakeDamage(damage);
            EventBus.instance.AddDomainEvent(new EnemyDamagedEvent(Id, damage, Stats.Health.CurrentHealth));

            if (Stats.Health.IsDead)
            {
                IsDead = true;
                EventBus.instance.AddDomainEvent(new EnemyDiedEvent(Id, Stats.Reward));
            }
        }

        public bool HasReachedEnd(float pathLength)
        {
            return DistanceTraveled >= pathLength;
        }
    }
    
    public class EnemyDamagedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public float Damage { get; }
        public float RemainingHealth { get; }

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

        public EnemyDiedEvent(Guid enemyId, Cost reward)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Reward = reward;
        }
    }
} 