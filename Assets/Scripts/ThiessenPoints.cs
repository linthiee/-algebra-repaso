using CustomMath;
using System.Collections.Generic;
using UnityEngine;

public class ThiessenPoints
{
    public Vec3 position;
    public List<MyPlane> boundingPlanes = new List<MyPlane>();
    public Color color;

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
}
