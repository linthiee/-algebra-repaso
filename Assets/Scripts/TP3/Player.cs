using CustomMath;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Serializable]
    public class Line
    {
        public Vec3 originalPos;
        public Vec3 direction;
        public Vec3 originalDirection;
    }

    [SerializeField] private int xMaxRays = 6;
    [SerializeField] private int yMaxRays = 6;

    [SerializeField] private Rigidbody rb;
    [SerializeField] Camera mainCamera;
    [SerializeField] Frustum frustum;

    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private float speed = 5.0f;

    public RoomManager roomManager;

    private Vec3 moveDirection;

    [SerializeField] private List<Line> playerRay = new List<Line>();
    [SerializeField] private float rayLength = 15.0f;

    [SerializeField] private bool showGrid = true;

    private float pitch;
    private float yaw;

    private void Start()
    {
        float tanHalfFov = Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float yMax = tanHalfFov;
        float xMax = yMax * mainCamera.aspect;

        for (int i = 0; i <= xMaxRays; i++)
        {
            float xAngle = Mathf.Lerp(-xMax, xMax, (float)i / xMaxRays);

            for (int j = 0; j <= yMaxRays; j++)
            {
                float yAngle = Mathf.Lerp(-yMax, yMax, (float)j / yMaxRays);

                Vector3 dir = new Vector3(xAngle, yAngle, 1f).normalized;

                playerRay.Add(new Line
                {
                    originalPos = Vec3.Zero,
                    direction = new Vec3(dir),
                    originalDirection = new Vec3(dir),
                });
            }
        }
    }

    private void Update()
    {
        Vec3 direction = Vec3.Zero;

        if (Input.GetKey(KeyCode.W)) 
            direction += new Vec3(transform.forward);
        if (Input.GetKey(KeyCode.S))
            direction += new Vec3(-transform.forward);
        if (Input.GetKey(KeyCode.A))
            direction += new Vec3(-transform.right);
        if (Input.GetKey(KeyCode.D))
            direction += new Vec3(transform.right);

        moveDirection = direction.normalized;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -89, 89);

        this.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        mainCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        foreach (Line line in playerRay)
        {
            line.direction = new Vec3(mainCamera.transform.rotation * line.originalDirection);
        }

        HashSet<Room> visibleRooms = new HashSet<Room>();
        
        if (roomManager != null && roomManager.bspRoot != null)
        {
            bool debugThisFrame = Input.GetKeyDown(KeyCode.Space);

            Vec3 origin = new Vec3(mainCamera.transform.position);

            Room currentRoom = roomManager.GetRoomAtPoint(origin, roomManager.bspRoot, debugThisFrame);
            if (currentRoom != null)
            {
                if (debugThisFrame)
                    Debug.Log($"player cam is currently inside: {currentRoom.name}");
                visibleRooms.Add(currentRoom);
            }
            else
            {
                if (debugThisFrame)
                    Debug.Log("player cam is not inside any room");
            }

            int centerRayIndex = playerRay.Count / 2;

            for (int i = 0; i < playerRay.Count; i++)
            {
                Line line = playerRay[i];
                bool isCenterRay = debugThisFrame && (i == centerRayIndex);

                if (isCenterRay)
                    Debug.Log($"player casting center ray: dir {line.direction}");

                roomManager.BSPSearch(origin, line.direction, rayLength, roomManager.bspRoot, visibleRooms, isCenterRay);
            }

            roomManager.UpdateRoomVisibility(visibleRooms);
        }


        frustum.UpdateFrustum();

        foreach(Room room in visibleRooms)
        {
            foreach(GameObject obj in room.insideObjects)
            {
                MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
                
                if(frustum.IsPointInside(new Vec3(obj.transform.position))) mesh.enabled = true;
                else mesh.enabled = false;
            }
        }

    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    private void OnDrawGizmos()
    {
        if (!showGrid) return;
        Gizmos.color = Color.green;
        Vec3 origin = new Vec3(mainCamera.transform.position);

        foreach (Line line in playerRay)
        {
            Gizmos.DrawRay(origin + line.originalPos, line.direction * rayLength);
        }
    }
}