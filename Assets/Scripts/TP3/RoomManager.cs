using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class RoomManager : MonoBehaviour
{
    public List<Room> allRooms;
    public BSPNode bspRoot;

    public Room GetRoomAtPoint(Vec3 point, BSPNode node, bool debug = false)
    {
        if (node == null) 
            return null;
        if (node.isLeaf)
            return node.room;

        bool isFront = node.partitionPlane.GetSide(point);

        if (isFront)
            return GetRoomAtPoint(point, node.frontNode, debug);
        else
            return GetRoomAtPoint(point, node.backNode, debug);
    }

    public void BSPSearch(Vec3 rayOrigin, Vec3 rayDir, float maxDistance, BSPNode node, HashSet<Room> visibleRooms, bool debug = false)
    {
        if (node == null)
            return;

        if (node.isLeaf)
        {
            if (debug)
                Debug.Log($"Ray hit Leaf Room! {node.room.name}");

            visibleRooms.Add(node.room);
            return;
        }

        float denom = Vec3.Dot(rayDir, node.partitionPlane.normal);
        bool originIsFront = node.partitionPlane.GetSide(rayOrigin);

        BSPNode nearNode = originIsFront ? node.frontNode : node.backNode;
        BSPNode farNode = originIsFront ? node.backNode : node.frontNode;

        BSPSearch(rayOrigin, rayDir, maxDistance, nearNode, visibleRooms, debug);

        if (Mathf.Abs(denom) > 0.0001f)
        {
            float distToPlane = node.partitionPlane.GetDistanceToPoint(rayOrigin);
            float t = -distToPlane / denom;

            if (t > 0 && t <= maxDistance)
            {
                Vec3 intersectionPoint = rayOrigin + (rayDir * t);

                bool insideFrame = false;
                if (node.portalDoors != null)
                {
                    foreach (Door door in node.portalDoors)
                    {
                        if (door.IsPointInsideFrame(intersectionPoint))
                        {
                            insideFrame = true;
                            if (debug)
                                Debug.Log($"Ray intersects door plane '{door.doorTransform.name}' at distance {t}. inside frame true");
                            break;
                        }
                    }
                }

                if (insideFrame)
                {            
                    Vec3 newOrigin = intersectionPoint + (rayDir * 0.01f);

                    float newMaxDistance = maxDistance - t;

                    BSPSearch(newOrigin, rayDir, newMaxDistance, farNode, visibleRooms, debug);
                }
                else
                {
                    return;
                }
            }
        }
    }

    public void UpdateRoomVisibility(HashSet<Room> visibleRooms)
    {

        foreach (Room room in allRooms)
        {
            room.SetVisible(false);
        }

        foreach (Room room in visibleRooms)
        {
            room.SetVisible(true);
        }
    }
}