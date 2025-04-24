using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Domain.Core.ValueObjects
{
    public class Position : ValueObject
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Position(float x, float z)
        {
            X = x;
            Y = 0;
            Z = z;
        }
        
        public Position(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Position(Vector3 vector3)
        {
            X = vector3.x;
            Y = vector3.y;
            Z = vector3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public float DistanceTo(Position other)
        {
            return Vector3.Distance(ToVector3(), other.ToVector3());
        }
    }
} 