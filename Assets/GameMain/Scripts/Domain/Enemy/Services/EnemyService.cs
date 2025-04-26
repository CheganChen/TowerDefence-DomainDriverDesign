using System;
using System.Collections.Generic;
using TowerDefense.Domain.Core.ValueObjects;
using TowerDefense.Domain.Enemy.Entities;
using TowerDefense.Domain.Enemy.ValueObjects;

namespace TowerDefense.Domain.Enemy.Services
{
    public interface IEnemyService
    {
        EnemyEntity SpawnEnemy(EnemyType type, Position startPosition);
        Position CalculateNextPosition(EnemyEntity enemy, Position targetPosition, float deltaTime);
        float CalculatePathLength(List<Position> path);
    }

    public class EnemyService : IEnemyService
    {
        private readonly Dictionary<EnemyType, EnemyStats> _enemyStatsConfig;

        public EnemyService()
        {
            _enemyStatsConfig = new Dictionary<EnemyType, EnemyStats>
            {
                { EnemyType.Basic, new EnemyStats(new Health(100),1, new Cost(10)) },
                { EnemyType.Fast, new EnemyStats(new Health(50), 1, new Cost(15)) },
                { EnemyType.Tank, new EnemyStats(new Health(200), 1, new Cost(20)) }
            };
        }

        public EnemyEntity SpawnEnemy(EnemyType type, Position startPosition)
        {
            if (!_enemyStatsConfig.TryGetValue(type, out var stats))
            {
                throw new ArgumentException($"Invalid enemy type: {type}", nameof(type));
            }

            return new EnemyEntity(type, startPosition, stats);
        }

        public Position CalculateNextPosition(EnemyEntity enemy, Position targetPosition, float deltaTime)
        {
            if (enemy == null)
                throw new ArgumentNullException(nameof(enemy));

            var direction = new Position(
                targetPosition.X - enemy.Position.X,
                targetPosition.Y - enemy.Position.Y
            );

            var distance = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            if (distance < 0.001f)
                return enemy.Position;

            var normalizedDirection = new Position(
                direction.X / distance,
                direction.Y / distance
            );

            var moveSpeed = 5f; // 这个值应该从配置中读取
            var moveDistance = moveSpeed * deltaTime;

            return new Position(
                enemy.Position.X + normalizedDirection.X * moveDistance,
                enemy.Position.Y + normalizedDirection.Y * moveDistance
            );
        }

        public float CalculatePathLength(List<Position> path)
        {
            if (path == null || path.Count < 2)
                return 0f;

            float length = 0f;
            for (int i = 1; i < path.Count; i++)
            {
                var dx = path[i].X - path[i - 1].X;
                var dy = path[i].Y - path[i - 1].Y;
                length += (float)Math.Sqrt(dx * dx + dy * dy);
            }

            return length;
        }
    }
} 