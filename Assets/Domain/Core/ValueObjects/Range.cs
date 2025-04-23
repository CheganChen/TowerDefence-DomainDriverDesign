using System.Collections.Generic;

namespace TowerDefense.Domain.Core.ValueObjects
{
    public class Range : ValueObject
    {
        public float MinRange { get; }
        public float MaxRange { get; }

        public Range(float maxRange, float minRange = 0)
        {
            MaxRange = maxRange;
            MinRange = minRange;
        }

        public bool IsInRange(float distance)
        {
            return distance >= MinRange && distance <= MaxRange;
        }

        public static Range operator +(Range a, Range b)
        {
            return new Range(a.MaxRange + b.MaxRange, a.MinRange + b.MinRange);
        }

        public static Range operator -(Range a, Range b)
        {
            return new Range(a.MaxRange - b.MaxRange, a.MinRange - b.MinRange);
        }

        public static Range operator *(Range range, float multiplier)
        {
            return new Range(range.MaxRange * multiplier, range.MinRange * multiplier);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return MinRange;
            yield return MaxRange;
        }
    }
} 