using System.Collections.Generic;
using TowerDefense.Domain.Enemy.Entities;

namespace TowerDefense.Domain.Wave.ValueObjects
{
    public class WaveConfig
    {
        public int WaveNumber { get; }
        public float SpawnInterval { get; }
        public int TotalEnemyCount { get; }
        public Dictionary<EnemyType, int> EnemyDistribution { get; }

        public WaveConfig(int waveNumber, float spawnInterval, int totalEnemyCount, Dictionary<EnemyType, int> enemyDistribution)
        {
            WaveNumber = waveNumber;
            SpawnInterval = spawnInterval;
            TotalEnemyCount = totalEnemyCount;
            EnemyDistribution = new Dictionary<EnemyType, int>(enemyDistribution);
        }
    }
} 