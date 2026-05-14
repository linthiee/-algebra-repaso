using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class RoomManager : MonoBehaviour
{
    public List<Room> allRooms;

    public Camera playerCamera;
    public Transform player;

    private Room currentRoom;

    public Frustum frustum;
    private void Start()
    {
        HideAllRooms();
    }

    private void Update()
    {
        foreach (Room room in allRooms)
        {
            if (room.ContainsPlayer(new Vec3(player.position)))
            {
                Debug.Log($"{room.name} is the current room");
                currentRoom = room;

                foreach (GameObject obj in currentRoom.insideObjects)
                {
                    obj.SetActive(true);
                }

                break;
            }
        }

        foreach (Door door in currentRoom.doors)
        {
            if (IsDoorVisible(door, new Vec3(playerCamera.transform.position), new Vec3(playerCamera.transform.forward)))
            {
                Debug.Log($"{door.name} is visible");
                foreach (GameObject obj in door.connectedRoom.insideObjects)
                {
                    obj.SetActive(true);
                }
            }
        }
    }

    public bool IsDoorVisible(Door door, Vec3 camPosition, Vec3 camForward)
    {
        Vec3 normalDoor = new Vec3(door.transform.forward);
        Vec3 doorPos = new Vec3(door.transform.position);
        MyPlane doorPlane = new MyPlane(normalDoor, doorPos);

        //is the doors normal pointing towards the camera? if not, we are behind the door
        if (!doorPlane.GetSide(camPosition))
        {
            if (door.name == "DoorTest")
                Debug.Log($"1 {door.name} not on the same side");
            return false;
        }

        //are we in front of the door or are we facing backwards?
        float dotView = Vec3.Dot(camForward, normalDoor);
        if (dotView > 0)
        {
            if (door.name == "DoorTest")

                Debug.Log($"2 {door.name} facing backwards from camera");

            return false;
        }

        //is door in frustum?
        if (!frustum.IsPointInside(doorPos))
        {
            if (door.name == "DoorTest")
                Debug.Log($"3 {door.name} not in frustum");

            return false;
        }

        if (door.name == "DoorTest")
            Debug.Log($" {door.name} true");

        return true;
    }
    private void HideAllRooms()
    {
        foreach (Room room in allRooms)
        {
            foreach (GameObject obj in room.insideObjects)
            {
                obj.SetActive(false);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (currentRoom == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(currentRoom.transform.position, 1.0f);

        foreach (Door door in currentRoom.doors)
        {
            if (door == null)
                continue;

            Vec3 posDoor = new Vec3(door.transform.position);

            if (IsDoorVisible(door, new Vec3(player.transform.position), new Vec3(player.transform.forward)))
            {

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(new Vec3(player.transform.position), posDoor);
                Gizmos.DrawSphere(posDoor, 0.2f);

                if (door.connectedRoom != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(door.connectedRoom.transform.position, Vector3.one * 2f);

                    Gizmos.color = new Color(0, 1, 0, 0.5f);
                    Gizmos.DrawLine(posDoor, door.connectedRoom.transform.position);
                }
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vec3(player.transform.position), posDoor);

                Gizmos.DrawRay(posDoor, Vector3.up * 0.5f);
                Gizmos.DrawRay(posDoor, Vector3.left * 0.5f);
            }
        }
    }
}