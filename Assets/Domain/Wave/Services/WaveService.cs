using System;
using System.Collections.Generic;
using TowerDefense.Domain.Enemy.Entities;
using TowerDefense.Domain.Wave.Entities;
using TowerDefense.Domain.Wave.ValueObjects;

namespace TowerDefense.Domain.Wave.Services
{
    public class WaveService
    {
        private const float BASE_SPAWN_INTERVAL = 1.0f;
        private const int BASE_ENEMY_COUNT = 10;

        public WaveConfig GenerateWaveConfig(int waveNumber)
        {
            var spawnInterval = CalculateSpawnInterval(waveNumber);
            var enemyDistribution = GenerateEnemyDistribution(waveNumber);

            return new WaveConfig(waveNumber, spawnInterval, enemyDistribution);
        }

        private float CalculateSpawnInterval(int waveNumber)
        {
            // Spawn interval decreases as wave number increases
            return Math.Max(0.2f, BASE_SPAWN_INTERVAL - (waveNumber - 1) * 0.1f);
        }

        private int CalculateEnemyCount(int waveNumber)
        {
            // Enemy count increases as wave number increases
            return BASE_ENEMY_COUNT + (waveNumber - 1) * 2;
        }

        private Dictionary<EnemyType, int> GenerateEnemyDistribution(int waveNumber)
        {
            var distribution = new Dictionary<EnemyType, int>();
            var totalEnemies = CalculateEnemyCount(waveNumber);

            // Basic enemies are always present
            distribution[EnemyType.Basic] = (int)(totalEnemies * 0.4f);

            // Add other enemy types based on wave number
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

            // Boss waves every 5 waves
            if (waveNumber % 5 == 0)
            {
                distribution[EnemyType.Boss] = 1;
            }

            // Adjust counts to match total
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
    }
} 