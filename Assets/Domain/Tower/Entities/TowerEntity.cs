using System;
using Domain.Core.Services;
using TowerDefense.Domain.Core.Entities.Base;
using TowerDefense.Domain.Core.ValueObjects;
using TowerDefense.Domain.Tower.ValueObjects;

namespace TowerDefense.Domain.Tower.Entities
{
    public class TowerEntity : Entity
    {
        public TowerType Type { get; private set; }
        public Position Position { get; private set; }
        public TowerStats Stats { get; private set; }
        public Cost UpgradeCost { get; private set; }
        public bool IsAttacking { get; private set; }
        public float LastAttackTime { get; private set; }

        public TowerEntity(TowerType type, Position position, TowerStats stats, Cost upgradeCost)
        {
            Type = type;
            Position = position;
            Stats = stats;
            UpgradeCost = upgradeCost;
            IsAttacking = false;
            LastAttackTime = 0f;
        }

        public void Upgrade()
        {
            Stats = Stats.Upgrade();
            UpgradeCost = UpgradeCost * 2;
            EventBus.instance.AddDomainEvent(new TowerUpgradedEvent(Id, Stats.Level));
        }

        public bool CanAttack(float currentTime)
        {
            return currentTime - LastAttackTime >= 1f / Stats.AttackSpeed;
        }

        public void Attack(float currentTime)
        {
            if (!CanAttack(currentTime))
                return;

            IsAttacking = true;
            LastAttackTime = currentTime;
            EventBus.instance.AddDomainEvent(new TowerAttackedEvent(Id, Stats.Damage));
        }

        public void StopAttacking()
        {
            IsAttacking = false;
        }

        public bool IsInRange(Position targetPosition)
        {
            return Stats.Range.IsInRange(Position.DistanceTo(targetPosition));
        }
    }
    
    public class TowerUpgradedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public int NewLevel { get; }

        public TowerUpgradedEvent(Guid towerId, int newLevel)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            NewLevel = newLevel;
        }
    }

    public class TowerAttackedEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public Damage Damage { get; }

        public TowerAttackedEvent(Guid towerId, Damage damage)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Damage = damage;
        }
    }
    
} 