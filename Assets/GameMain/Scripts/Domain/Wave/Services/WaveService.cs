using System;
using System.Collections.Generic;
using TowerDefense.Domain.Core.ValueObjects;
using TowerDefense.Domain.Enemy.Entities;
using TowerDefense.Domain.Enemy.Services;
using TowerDefense.Domain.Enemy.ValueObjects;
using TowerDefense.Domain.Wave.Entities;
using TowerDefense.Domain.Wave.ValueObjects;

namespace TowerDefense.Domain.Wave.Services
{
    public class WaveService
    {
        private const float BaseSpawnInterval = 1.0f;
        private const int BaseEnemyCount = 10;
        private readonly IEnemyService _enemyService;

        public WaveService(IEnemyService enemyService)
        {
            _enemyService = enemyService ?? throw new ArgumentNullException(nameof(enemyService));
        }

        public WaveConfig GenerateWaveConfig(int waveNumber)
        {
            var spawnInterval = CalculateSpawnInterval(waveNumber);
            var enemyDistribution = GenerateEnemyDistribution(waveNumber);
            var totalEnemyCount = CalculateEnemyCount(waveNumber);

            return new WaveConfig(waveNumber, spawnInterval, totalEnemyCount, enemyDistribution);
        }

        private float CalculateSpawnInterval(int waveNumber)
        {
            return Math.Max(0.2f, BaseSpawnInterval - (waveNumber - 1) * 0.1f);
        }

        private int CalculateEnemyCount(int waveNumber)
        {
            return BaseEnemyCount + (waveNumber - 1) * 2;
        }

        private Dictionary<EnemyType, int> GenerateEnemyDistribution(int waveNumber)
        {
            var distribution = new Dictionary<EnemyType, int>();
            var totalEnemies = CalculateEnemyCount(waveNumber);
            
            distribution[EnemyType.Basic] = (int)(totalEnemies * 0.4f);
            
            if (waveNumber >= 2)
            {
                distribution[EnemyType.Fast] = (int)(totalEnemies * 0.2f);
            }

            if (waveNumber >= 3)
            {
                distribution[EnemyType.Tank] = (int)(totalEnemies * 0.2f);
            }

            if (waveNumber >= 4)
            {
                distribution[EnemyType.Flying] = (int)(totalEnemies * 0.1f);
            }
            
            if (waveNumber % 5 == 0)
            {
                distribution[EnemyType.Boss] = 1;
            }
            
            var currentTotal = 0;
            foreach (var count in distribution.Values)
            {
                currentTotal += count;
            }

            if (currentTotal < totalEnemies)
            {
                distribution[EnemyType.Basic] += totalEnemies - currentTotal;
            }

            return distribution;
        }

        public WaveEntity CreateWave(int waveNumber)
        {
            var config = GenerateWaveConfig(waveNumber);
            return new WaveEntity(config);
        }
        
        public EnemyEntity SpawnEnemyForWave(WaveEntity wave, Position startPosition)
        {
            if (wave == null)
                throw new ArgumentNullException(nameof(wave));
                
            if (!wave.CanSpawnEnemy())
                return null;
                
            var enemyType = wave.GetNextEnemyType();
            var enemy = _enemyService.SpawnEnemy(enemyType, startPosition);
            
            // 将敌人添加到波次中
            wave.SpawnEnemy(enemyType, startPosition);
            
            return enemy;
        }
    }
} 