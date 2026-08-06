using System.Collections.Generic;
using CustomMath;

public class BSPNode
{
    public bool isLeaf;
    public Room room;
    public MyPlane partitionPlane;
    public BSPNode frontNode;
    public BSPNode backNode;

    public List<Door> portalDoors;

    public BSPNode(Room leafRoom)
    {
        isLeaf = true;
        room = leafRoom;
        portalDoors = new List<Door>();
    }

    public BSPNode(MyPlane plane, BSPNode front, BSPNode back, List<Door> doors)
    {
        isLeaf = false;
        partitionPlane = plane;
        frontNode = front;
        backNode = back;
        portalDoors = doors;
    }
}