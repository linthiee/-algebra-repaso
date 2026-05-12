using CustomMath;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ThiessenPoints
{
    public Vec3 position;
    public List<MyPlane> boundingPlanes = new List<MyPlane>();

    public bool ContainsPoint(Vec3 checkPosition)
    {
        foreach (MyPlane plane in boundingPlanes)
        {
            if (!plane.GetSide(checkPosition))
            {
                return false;
            }
        }
        return true;
    }

    public void DrawPoints()
    {
        Gizmos.DrawSphere(position, 0.5f);
    }
}
