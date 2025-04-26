using System;
using System.Collections.Generic;
using Domain.Core.Services;
using TowerDefense.Domain.Core.Entities.Base;
using TowerDefense.Domain.Core.ValueObjects;
using TowerDefense.Domain.Enemy.Entities;
using TowerDefense.Domain.Wave.ValueObjects;

namespace TowerDefense.Domain.Wave.Entities
{
    public class WaveEntity : AggregateRoot
    {
        public int WaveNumber { get; private set; }
        public float SpawnInterval { get; private set; }
        public int TotalEnemyCount { get; private set; }
        public Dictionary<EnemyType, int> EnemyDistribution { get; private set; }
        public bool IsCompleted { get; private set; }
        public int EnemiesSpawned { get; private set; }
        public int EnemiesDefeated { get; private set; }
        
        // 存储波次内的敌人实体
        private readonly List<EnemyEntity> _enemies = new List<EnemyEntity>();
        public IReadOnlyCollection<EnemyEntity> Enemies => _enemies.AsReadOnly();

        public WaveEntity(WaveConfig config)
        {
            WaveNumber = config.WaveNumber;
            SpawnInterval = config.SpawnInterval;
            TotalEnemyCount = config.TotalEnemyCount;
            EnemyDistribution = new Dictionary<EnemyType, int>(config.EnemyDistribution);
            IsCompleted = false;
            EnemiesSpawned = 0;
            EnemiesDefeated = 0;
            
            // 添加波次开始事件
            EventBus.instance.AddDomainEvent(new WaveStartedEvent(Id, WaveNumber));
        }

        public EnemyEntity SpawnEnemy(EnemyType enemyType, Position startPosition)
        {
            if (IsCompleted || EnemiesSpawned >= TotalEnemyCount)
            {
                return null;
            }

            // 创建敌人实体
            var enemy = new EnemyEntity(enemyType, startPosition, null); // 注意：这里需要传入正确的EnemyStats
            _enemies.Add(enemy);
            EnemiesSpawned++;
            
            // 添加敌人生成事件
            EventBus.instance.AddDomainEvent(new EnemySpawnedEvent(Id, enemy.Id));
            
            return enemy;
        }

        public void EnemyDefeated(EnemyEntity enemy)
        {
            if (enemy == null || !_enemies.Contains(enemy))
            {
                return;
            }
            
            EnemiesDefeated++;
            
            // 添加敌人击败事件
            EventBus.instance.AddDomainEvent(new EnemyDefeatedEvent(Id, EnemiesDefeated));
            
            if (EnemiesDefeated >= TotalEnemyCount)
            {
                IsCompleted = true;
                // 添加波次完成事件
                EventBus.instance.AddDomainEvent(new WaveCompletedEvent(Id, WaveNumber));
            }
        }

        public bool CanSpawnEnemy()
        {
            return !IsCompleted && EnemiesSpawned < TotalEnemyCount;
        }

        public EnemyType GetNextEnemyType()
        {
            if (!CanSpawnEnemy())
            {
                throw new InvalidOperationException("Cannot spawn more enemies in this wave");
            }

            // Find the first enemy type that still has enemies to spawn
            foreach (var kvp in EnemyDistribution)
            {
                if (kvp.Value > 0)
                {
                    EnemyDistribution[kvp.Key]--;
                    return kvp.Key;
                }
            }

            throw new InvalidOperationException("No more enemies to spawn in distribution");
        }
        
        public class WaveStartedEvent : IDomainEvent
        {
            public Guid Id { get; }
            public DateTime OccurredOn { get; }
            public int WaveNumber { get; }

            public WaveStartedEvent(Guid waveId, int waveNumber)
            {
                Id = Guid.NewGuid();
                OccurredOn = DateTime.UtcNow;
                WaveNumber = waveNumber;
            }
        }
        
        public class EnemySpawnedEvent : IDomainEvent
        {
            public Guid Id { get; }
            public DateTime OccurredOn { get; }
            public Guid EnemyId { get; }

            public EnemySpawnedEvent(Guid waveId, Guid enemyId)
            {
                Id = Guid.NewGuid();
                OccurredOn = DateTime.UtcNow;
                EnemyId = enemyId;
            }
        }
        
        public class EnemyDefeatedEvent : IDomainEvent
        {
            public Guid Id { get; }
            public DateTime OccurredOn { get; }
            public int EnemiesDefeated { get; }

            public EnemyDefeatedEvent(Guid waveId, int enemiesDefeated)
            {
                Id = Guid.NewGuid();
                OccurredOn = DateTime.UtcNow;
                EnemiesDefeated = enemiesDefeated;
            }
        }
        
        public class WaveCompletedEvent : IDomainEvent
        {
            public Guid Id { get; }
            public DateTime OccurredOn { get; }
            public int WaveNumber { get; }

            public WaveCompletedEvent(Guid waveId, int waveNumber)
            {
                Id = Guid.NewGuid();
                OccurredOn = DateTime.UtcNow;
                WaveNumber = waveNumber;
            }
        }
    }
} 