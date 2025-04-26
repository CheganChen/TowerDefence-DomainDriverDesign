using System.Collections.Generic;
using TowerDefense.Domain.Core.ValueObjects;

namespace TowerDefense.Domain.Tower.ValueObjects
{
    public class TowerStats : ValueObject
    {
        public Damage Damage { get; }
        public Range Range { get; }
        public float AttackSpeed { get; }
        public int Level { get; }

        public TowerStats(Damage damage, Range range, float attackSpeed, int level = 1)
        {
            Damage = damage;
            Range = range;
            AttackSpeed = attackSpeed;
            Level = level;
        }

        public TowerStats Upgrade()
        {
            return new TowerStats(
                Damage * 1.5f,
                Range * 1.2f,
                AttackSpeed * 1.1f,
                Level + 1
            );
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Damage;
            yield return Range;
            yield return AttackSpeed;
            yield return Level;
        }
    }
} 