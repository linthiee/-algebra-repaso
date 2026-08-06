using CustomMath;
using System;
using UnityEngine;

[Serializable]
public class Door
{
    public Transform doorTransform;
    public Room connectedRoom;

    public MyPlane dividingPlane; 

    public MyPlane[] framePlanes;

    public void Init()
    {
        framePlanes = new MyPlane[4];

        Vec3 doorPos = new Vec3(doorTransform.position);
        Vec3 forwardDirection = new Vec3(doorTransform.forward);
        Vec3 rightDirection = new Vec3(doorTransform.right);
        Vec3 upDirection = new Vec3(doorTransform.up);

        dividingPlane = new MyPlane(forwardDirection, doorPos);

        float halfWidth = doorTransform.localScale.x * 0.5f;
        float halfHeight = doorTransform.localScale.y * 0.5f;

        framePlanes[0] = new MyPlane(-rightDirection, doorPos + (rightDirection * halfWidth));
        framePlanes[1] = new MyPlane(rightDirection, doorPos - (rightDirection * halfWidth));
        framePlanes[2] = new MyPlane(-upDirection, doorPos + (upDirection * halfHeight));
        framePlanes[3] = new MyPlane(upDirection, doorPos - (upDirection * halfHeight));
    }

    public bool IsPointInsideFrame(Vec3 intersectionPoint)
    {
        foreach (MyPlane plane in framePlanes)
        {
            if (!plane.GetSide(intersectionPoint))
            {
                return false; 
            }
        }
        return true;
    }

    public void DrawGizmos()
    {
        Vec3 pos = new Vec3(doorTransform.position);
        Vec3 right = new Vec3(doorTransform.right);
        Vec3 up = new Vec3(doorTransform.up);

        float halfWidth = doorTransform.localScale.x * 0.5f;
        float halfHeight = doorTransform.localScale.y * 0.5f;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(pos, new Vec3(dividingPlane.normal.x, dividingPlane.normal.y, dividingPlane.normal.z));

        Gizmos.color = Color.magenta;

        Gizmos.DrawRay(pos + right * halfWidth, new Vec3(framePlanes[0].normal.x, framePlanes[0].normal.y, framePlanes[0].normal.z));
        Gizmos.DrawRay(pos - right * halfWidth, new Vec3(framePlanes[1].normal.x, framePlanes[1].normal.y, framePlanes[1].normal.z));
        Gizmos.DrawRay(pos + up * halfHeight, new Vec3(framePlanes[2].normal.x, framePlanes[2].normal.y, framePlanes[2].normal.z));
        Gizmos.DrawRay(pos - up * halfHeight, new Vec3(framePlanes[3].normal.x, framePlanes[3].normal.y, framePlanes[3].normal.z));
    }
}