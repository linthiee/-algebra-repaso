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

    private void Update()
    {
        HideAllRooms();

        foreach (Room room in allRooms)
        {
            if (room.ContainsPlayer(new Vec3(player.position)))
            {
                room.gameObject.SetActive(true);
                room.hasBeenChecked = true;

                Debug.Log($"{room.name} is the current room");
                currentRoom = room;

                foreach (GameObject obj in currentRoom.insideObjects)
                {
                    obj.SetActive(true);
                }

                break;
            }
        }

        CheckConnectedRooms();
    }

    public void CheckPointOnAdjacentRooms(Room room, Vec3 pointToCheck, HashSet<Room> visitedRooms)
    {
        foreach (Door door in room.doors)
        {
            if (IsDoorVisible(door, new Vec3(playerCamera.transform.position), new Vec3(playerCamera.transform.forward)))
            { 
                if (!visitedRooms.Contains(door.connectedRoom))
                {
                    visitedRooms.Add(door.connectedRoom);

                    if (door.connectedRoom != null)
                    {
                        if (door.connectedRoom.ContainsPlayer(pointToCheck))
                        {
                            door.connectedRoom.hasBeenChecked = true;
                            door.connectedRoom.gameObject.SetActive(true);

                            foreach (GameObject obj in door.connectedRoom.insideObjects)
                            {
                                obj.SetActive(true);
                            }

                            CheckPointOnAdjacentRooms(door.connectedRoom, pointToCheck, visitedRooms);
                        }
                    }
                }
            }
        }

        //foreach (Room roomToCheck in allRooms)
        //{
        //    if (roomToCheck.ContainsPlayer(pointToCheck))
        //    {
        //        if (!roomToCheck.hasBeenChecked)
        //        {
        //            roomToCheck.hasBeenChecked = true;
        //            roomToCheck.gameObject.SetActive(true);
        //            foreach (GameObject obj in roomToCheck.insideObjects)
        //            {
        //                obj.SetActive(true);
        //            }
        //        }
        //    }
        //}
    }

    public void CheckPointOnCurrentRoom(Vec3 pointToCheck, HashSet<Room> visitedRooms)
    {
        CheckPointOnAdjacentRooms(currentRoom, pointToCheck, visitedRooms);
    }
    private void CheckConnectedRooms()
    {
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
        //CHECK
        //chequear con el lookingAt de la camara
        
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

        if (door.name == "DoorTest")
            Debug.Log($" {door.name} true");

        return true;
    }
    private void HideAllRooms()
    {
        foreach (Room room in allRooms)
        {
            room.gameObject.SetActive(false);
            room.hasBeenChecked = false;
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