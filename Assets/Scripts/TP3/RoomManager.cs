using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class RoomManager : MonoBehaviour
{
    public List<Room> allRooms;

    public Camera playerCamera;
    public Transform player;

    private void Update()
    {
        foreach (Room room in allRooms)
        {
            if (room.ContainsPlayer(new Vec3(player.position)))
            {
                Debug.Log($"{room.name} is the current room");
            }
        }
    }
}