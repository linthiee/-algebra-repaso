using CustomMath;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<GameObject> insideObjects;

    public List<Door> doors;

    public Transform roomVolume;

    public MyPlane[] boundingPlanes = new MyPlane[6];

    private Vec3 extents;

    public bool hasBeenChecked = false;

    private void Start()
    {
        extents = new Vec3(roomVolume.localScale.x / 2, roomVolume.localScale.y / 2, roomVolume.localScale.z / 2);

        boundingPlanes[0] = new MyPlane(Vec3.Down, new Vec3(roomVolume.position + (Vec3.Up * extents.y))); //up
        boundingPlanes[1] = new MyPlane(Vec3.Up, new Vec3(roomVolume.position + (Vec3.Down * extents.y))); //down

        boundingPlanes[2] = new MyPlane(Vec3.Right, new Vec3(roomVolume.position + (Vec3.Left * extents.x))); //left
        boundingPlanes[3] = new MyPlane(Vec3.Left, new Vec3(roomVolume.position + (Vec3.Right * extents.x))); //right

        boundingPlanes[4] = new MyPlane(Vec3.Forward, new Vec3(roomVolume.position + (Vec3.Back * extents.z))); //back
        boundingPlanes[5] = new MyPlane(Vec3.Back, new Vec3(roomVolume.position + (Vec3.Forward * extents.z))); //forward
    }

    public bool ContainsPlayer(Vec3 checkPosition)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vec3 center = new Vec3(transform.position);

        for (int i = 0; i < 6; i++)
        {
            MyPlane plane = boundingPlanes[i];

            float distanceToPlane = plane.GetDistanceToPoint(center);
            Vec3 pointOnPlane = center - (plane.normal * distanceToPlane);

            Gizmos.DrawLine(pointOnPlane, pointOnPlane + (plane.normal * 2.0f));
            Gizmos.DrawSphere(pointOnPlane, 0.2f);
        }

    }
}