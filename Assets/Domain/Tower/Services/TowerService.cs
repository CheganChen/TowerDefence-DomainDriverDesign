using System;
using System.Collections.Generic;
using TowerDefense.Domain.Core.ValueObjects;
using TowerDefense.Domain.Tower.Entities;
using TowerDefense.Domain.Tower.ValueObjects;
using Range = TowerDefense.Domain.Core.ValueObjects.Range;

namespace Domain.Tower.Services
{
    public class TowerService
    {
        private readonly Dictionary<TowerType, TowerStats> _defaultStats;
        private readonly Dictionary<TowerType, Cost> _defaultCosts;

        public TowerService()
        {
            _defaultStats = new Dictionary<TowerType, TowerStats>
            {
                { TowerType.Basic, new TowerStats(
                    new Damage(10),
                    new Range(5),
                    1f
                )},
                { TowerType.Sniper, new TowerStats(
                    new Damage(30),
                    new Range(10),
                    0.5f
                )},
                { TowerType.Splash, new TowerStats(
                    new Damage(15),
                    new Range(3),
                    0.8f
                )},
                { TowerType.Slow, new TowerStats(
                    new Damage(5),
                    new Range(4),
                    1.2f
                )},
                { TowerType.Buff, new TowerStats(
                    new Damage(0),
                    new Range(6),
                    2f
                )}
            };

            _defaultCosts = new Dictionary<TowerType, Cost>
            {
                { TowerType.Basic, new Cost(100) },
                { TowerType.Sniper, new Cost(200) },
                { TowerType.Splash, new Cost(150) },
                { TowerType.Slow, new Cost(120) },
                { TowerType.Buff, new Cost(180) }
            };
        }

        public TowerEntity CreateTower(TowerType type, Position position)
        {
            if (!_defaultStats.ContainsKey(type))
                throw new ArgumentException($"Invalid tower type: {type}");

            var stats = _defaultStats[type];
            var cost = _defaultCosts[type];

            return new TowerEntity(type, position, stats, cost);
        }

        public bool CanUpgradeTower(TowerEntity tower, Cost playerGold)
        {
            return playerGold >= tower.UpgradeCost;
        }

        public float CalculateTowerDamage(TowerEntity tower, float distance)
        {
            if (!tower.IsInRange(new Position(distance, 0, 0)))
                return 0;

            var damage = tower.Stats.Damage;

            // Apply distance-based damage reduction
            if (tower.Type == TowerType.Sniper)
            {
                var distanceFactor = 1f - (distance / tower.Stats.Range.MaxRange) * 0.5f;
                damage = damage * distanceFactor;
            }

            return damage.CalculateFinalDamage();
        }
    }
} 