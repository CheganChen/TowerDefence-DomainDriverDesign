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
        
        private EnemyState _currentState;
        public EnemyStateType CurrentStateType
        {
            get
            {
                switch (_currentState.GetType().Name)
                {
                    case nameof(MovingState):
                        return EnemyStateType.Moving;
                    case nameof(DamagedState):
                        return EnemyStateType.Damaged;
                    case nameof(DeadState):
                        return EnemyStateType.Dead;
                    default:
                        throw new InvalidOperationException("Unknown state type");
                }
            }
        }

        public bool IsDead => CurrentStateType == EnemyStateType.Dead;

        public EnemyEntity(EnemyType type, Position position, EnemyStats stats)
        {
            Type = type;
            Position = position;
            Stats = stats;
            DistanceTraveled = 0f;
            
            // 初始化状态
            ChangeState(new MovingState(this));
        }

        public void Move(Position newPosition, float distance)
        {
            if (IsDead)
                return;

            Position = newPosition;
            DistanceTraveled += distance;
            EventBus.instance.AddDomainEvent(new EnemyMovedEvent(Id, Position, DistanceTraveled));
            
            _currentState.Update();
        }

        public void TakeDamage(float damage)
        {
            if (IsDead)
                return;

            Stats.Health.TakeDamage(damage);
            EventBus.instance.AddDomainEvent(new EnemyDamagedEvent(Id, damage, Stats.Health.CurrentHealth));

            if (Stats.Health.IsDead)
            {
                ChangeState(new DeadState(this));
                EventBus.instance.AddDomainEvent(new EnemyDiedEvent(Id, Stats.Reward));
            }
            else
            {
                ChangeState(new DamagedState(this));
            }
        }

        public bool HasReachedEnd(float pathLength)
        {
            return DistanceTraveled >= pathLength;
        }

        private void ChangeState(EnemyState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
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