using UnityEngine;
using CustomMath;

public class TestBall : MonoBehaviour
{
    public RoomManager roomManager;

    Room currentRoom = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Room testRoom = roomManager.GetRoomAtPoint(new Vec3(this.transform.position),roomManager.bspRoot);
        if (testRoom != currentRoom)
        {
            currentRoom = testRoom;

            if (currentRoom != null)
            {
                Debug.Log($"NEW ROOM!!! {currentRoom.name}");
            }
            else
            {
                Debug.Log($"NUll :(((((((");
            }
        }
    }
}
