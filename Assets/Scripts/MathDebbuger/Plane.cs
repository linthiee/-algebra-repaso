using System;
using UnityEngine;
namespace CustomMath
{
    public struct MyPlane : IEquatable<MyPlane>
    {
        public Vec3 normal;
        public float distance;
        public MyPlane flipped()
        {
            return new MyPlane(-normal, -distance);
        }

        public MyPlane(Vec3 inNormal, Vec3 inPoint)
        {
            normal = inNormal.normalized;
            distance = -Vec3.Dot(normal, inPoint);
        }
        public MyPlane(Vec3 inNormal, float d)
        {
            normal = inNormal.normalized;
            distance = d;
        }
        public MyPlane(Vec3 a, Vec3 b, Vec3 c)
        {
            normal = Vec3.Cross(b - a, c - a).normalized;
            distance = -Vec3.Dot(normal, a);
        }

        public static bool operator ==(MyPlane lhs, MyPlane rhs)
        {
            return (lhs.normal == rhs.normal && lhs.distance == rhs.distance);
        }
        public static bool operator !=(MyPlane lhs, MyPlane rhs)
        {
            return !(lhs == rhs);
        }
        public void Translate(MyPlane plane, Vec3 translation)
        {
            plane.distance -= Vec3.Dot(plane.normal, translation);
        }
        public Vec3 ClosestPointOnPlane(Vec3 point)
        {
            float dist = GetDistanceToPoint(point);
            return point - normal * dist;
        }
        public bool Equals(MyPlane other)
        {
            return (this.normal == other.normal && this.distance == other.distance);
        }
        public override bool Equals(object other)
        {
            if (!(other is MyPlane))
                return false;

            return (other.Equals(this));
        }
        public override int GetHashCode()
        {
            return normal.GetHashCode() ^ distance.GetHashCode();
        }

        public void Flip()
        {
            normal *= -1;
            distance *= -1;
        }
        public float GetDistanceToPoint(Vec3 point)
        {
            return Vec3.Dot(normal, point) + distance;
        }
        public bool GetSide(Vec3 point)
        {
            return GetDistanceToPoint(point) > 0f;
        }
        public bool SameSide(Vec3 inPt0, Vec3 inPt1)
        {
            float d0 = GetDistanceToPoint(inPt0);
            float d1 = GetDistanceToPoint(inPt1);

            return (d0 > 0f && d1 > 0f) || (d0 <= 0f && d1 <= 0f);
        }
        public void Set3Points(Vec3 a, Vec3 b, Vec3 c)
        {
            normal = Vec3.Cross(b - a, c - a).normalized;
            distance = -Vec3.Dot(normal, a);
        }

        public void SetNormalAndPosition(Vec3 inNormal, Vec3 inPoint)
        {
            normal = inNormal.normalized;
            distance = -Vec3.Dot(normal, inPoint);
        }
        public void Translate(Vec3 translation)
        {
            distance -= Vec3.Dot(normal, translation);
        }
        public override string ToString()
        {
            string planeInfo = "Plane normal: " + normal.ToString() + "  Distance: " + distance.ToString();
            return planeInfo;
        }
    }
}
