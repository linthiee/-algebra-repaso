using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class LevelGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public GameObject roomPrefab;
    public int gridWidth = 3;
    public int gridDepth = 3;
    public float roomSpacing = 10f;

    [Header("Door Settings")]
    public Vector2 doorSize = new Vector2(2f, 3f);

    [Header("References")]
    public RoomManager roomManager;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        Room[,] grid = new Room[gridWidth, gridDepth];
        List<Room> generatedRooms = new List<Room>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vec3 position = new Vec3(x * roomSpacing, 0, z * roomSpacing);

                GameObject roomGo;

                if (roomPrefab != null)
                {
                    roomGo = Instantiate(roomPrefab, position, Quaternion.identity);
                    roomGo.name = $"Room_{x}_{z}";
                    roomGo.transform.SetParent(this.transform);
                }
                else
                {
                    roomGo = new GameObject($"Room_{x}_{z}");
                    roomGo.transform.position = position;
                    roomGo.transform.SetParent(this.transform);
                }

                Room newRoom = roomGo.GetComponent<Room>();
                if (newRoom == null)
                {
                    newRoom = roomGo.AddComponent<Room>();
                }

                newRoom.doors = new List<Door>();

                if (newRoom.roomVolume == null)
                {
                    GameObject volumeObj = new GameObject("Volume");
                    volumeObj.transform.SetParent(roomGo.transform);
                    volumeObj.transform.localPosition = Vec3.Zero;
                    volumeObj.transform.localScale = new Vec3(roomSpacing, roomSpacing, roomSpacing);
                    newRoom.roomVolume = volumeObj.transform;
                }

                grid[x, z] = newRoom;
                generatedRooms.Add(newRoom);
            }
        }

        float halfSpacing = roomSpacing / 2f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Room currentRoom = grid[x, z];

                if (x < gridWidth - 1)
                {
                    Room eastRoom = grid[x + 1, z];
                    CreateDoorConnection(currentRoom, eastRoom, new Vec3(halfSpacing, 0, 0), Quaternion.Euler(0, 90, 0));
                }

                if (x > 0)
                {
                    Room westRoom = grid[x - 1, z];
                    CreateDoorConnection(currentRoom, westRoom, new Vec3(-halfSpacing, 0, 0), Quaternion.Euler(0, -90, 0));
                }

                if (z < gridDepth - 1)
                {
                    Room northRoom = grid[x, z + 1];
                    CreateDoorConnection(currentRoom, northRoom, new Vec3(0, 0, halfSpacing), Quaternion.Euler(0, 0, 0));
                }

                if (z > 0)
                {
                    Room southRoom = grid[x, z - 1];
                    CreateDoorConnection(currentRoom, southRoom, new Vec3(0, 0, -halfSpacing), Quaternion.Euler(0, 180, 0));
                }
            }
        }

        foreach (Room room in generatedRooms)
        {
            room.InitializeDoors();
        }

        BSPNode rootNode = BuildBSPTree(generatedRooms);

        roomManager.allRooms = generatedRooms;
        roomManager.bspRoot = rootNode;
    }

    private void CreateDoorConnection(Room fromRoom, Room toRoom, Vec3 offset, Quaternion rotation)
    {
        GameObject doorObj = new GameObject($"PlaneTo_{toRoom.name}");
        doorObj.transform.SetParent(fromRoom.transform);
        doorObj.transform.position = fromRoom.transform.position + offset;
        doorObj.transform.rotation = rotation;

        doorObj.transform.localScale = new Vec3(doorSize.x, doorSize.y, 1f);

        Door newDoor = new Door();
        newDoor.doorTransform = doorObj.transform;
        newDoor.connectedRoom = toRoom;

        fromRoom.doors.Add(newDoor);
    }

    private BSPNode BuildBSPTree(List<Room> remainingRooms)
    {
        if (remainingRooms == null || remainingRooms.Count == 0)
            return null;

        if (remainingRooms.Count == 1)
            return new BSPNode(remainingRooms[0]);

        Door splitDoor = null;
        foreach (Room room in remainingRooms)
        {
            foreach (Door door in room.doors)
            {
                int frontCount = 0;
                int backCount = 0;

                foreach (Room testRoom in remainingRooms)
                {
                    bool isFront = door.dividingPlane.GetSide(new Vec3(testRoom.transform.position));

                    if (isFront)
                        frontCount++;
                    else 
                        backCount++;
                }

                if (frontCount > 0 && backCount > 0)
                {
                    splitDoor = door;
                    break; 
                }
            }
            if (splitDoor != null)
                break;
        }

        List<Door> coplanarDoors = new List<Door>();
        foreach (Room room in remainingRooms)
        {
            foreach (Door door in room.doors)
            {
                float normalDot = Vec3.Dot(splitDoor.dividingPlane.normal, door.dividingPlane.normal);
                if (Mathf.Abs(normalDot) > 0.99f)
                {
                    float dist = splitDoor.dividingPlane.GetDistanceToPoint(new Vec3(door.doorTransform.position));
                    if (Mathf.Abs(dist) < 0.05f)
                    {
                        coplanarDoors.Add(door);
                    }
                }
            }
        }

        List<Room> frontRooms = new List<Room>();
        List<Room> backRooms = new List<Room>();

        foreach (Room room in remainingRooms)
        {
            bool isFront = splitDoor.dividingPlane.GetSide(new Vec3(room.transform.position));
            if (isFront)
                frontRooms.Add(room);
            else
                backRooms.Add(room);
        }

        BSPNode frontNode = BuildBSPTree(frontRooms);
        BSPNode backNode = BuildBSPTree(backRooms);

        return new BSPNode(splitDoor.dividingPlane, frontNode, backNode, coplanarDoors);
    }
}