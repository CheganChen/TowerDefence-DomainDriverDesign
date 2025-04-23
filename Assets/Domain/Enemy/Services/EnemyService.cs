using System;
using System.Collections.Generic;
using TowerDefense.Domain.Core.ValueObjects;
using TowerDefense.Domain.Enemy.Entities;
using TowerDefense.Domain.Enemy.ValueObjects;

namespace TowerDefense.Domain.Enemy.Services
{
    public class EnemyService
    {
        private readonly Dictionary<EnemyType, EnemyStats> _defaultStats;

        public EnemyService()
        {
            _defaultStats = new Dictionary<EnemyType, EnemyStats>
            {
                { EnemyType.Basic, new EnemyStats(
                    new Health(100),
                    2f,
                    new Cost(10)
                )},
                { EnemyType.Fast, new EnemyStats(
                    new Health(50),
                    4f,
                    new Cost(15),
                    0.8f
                )},
                { EnemyType.Tank, new EnemyStats(
                    new Health(300),
                    1f,
                    new Cost(25),
                    1.5f
                )},
                { EnemyType.Flying, new EnemyStats(
                    new Health(75),
                    3f,
                    new Cost(20),
                    1f
                )},
                { EnemyType.Boss, new EnemyStats(
                    new Health(1000),
                    1.5f,
                    new Cost(100),
                    2f
                )}
            };
        }

        public EnemyEntity CreateEnemy(EnemyType type, Position position)
        {
            if (!_defaultStats.ContainsKey(type))
                throw new ArgumentException($"Invalid enemy type: {type}");

            var stats = _defaultStats[type];
            return new EnemyEntity(type, position, stats);
        }

        public float CalculateEnemySpeed(EnemyEntity enemy, float waveNumber)
        {
            var baseSpeed = enemy.Stats.Speed;
            var waveMultiplier = 1f + (waveNumber - 1) * 0.1f;
            return baseSpeed * waveMultiplier;
        }

        public float CalculateEnemyHealth(EnemyEntity enemy, float waveNumber)
        {
            var baseHealth = enemy.Stats.Health.MaxHealth;
            var waveMultiplier = 1f + (waveNumber - 1) * 0.2f;
            return baseHealth * waveMultiplier;
        }

        public Cost CalculateEnemyReward(EnemyEntity enemy, float waveNumber)
        {
            var baseReward = enemy.Stats.Reward.Amount;
            var waveMultiplier = 1f + (waveNumber - 1) * 0.15f;
            return new Cost((int)(baseReward * waveMultiplier));
        }
    }
} 