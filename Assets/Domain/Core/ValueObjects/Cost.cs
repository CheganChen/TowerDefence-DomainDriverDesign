using System.Collections.Generic;

namespace TowerDefense.Domain.Core.ValueObjects
{
    public class Cost : ValueObject
    {
        public int Amount { get; }

        public Cost(int amount)
        {
            Amount = amount;
        }

        public static Cost operator +(Cost a, Cost b)
        {
            return new Cost(a.Amount + b.Amount);
        }

        public static Cost operator -(Cost a, Cost b)
        {
            return new Cost(a.Amount - b.Amount);
        }

        public static Cost operator *(Cost cost, int multiplier)
        {
            return new Cost(cost.Amount * multiplier);
        }

        public static bool operator >(Cost a, Cost b)
        {
            return a.Amount > b.Amount;
        }

        public static bool operator <(Cost a, Cost b)
        {
            return a.Amount < b.Amount;
        }

        public static bool operator >=(Cost a, Cost b)
        {
            return a.Amount >= b.Amount;
        }

        public static bool operator <=(Cost a, Cost b)
        {
            return a.Amount <= b.Amount;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
        }
    }
} 