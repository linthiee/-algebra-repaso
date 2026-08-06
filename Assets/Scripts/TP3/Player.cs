using CustomMath;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Player;

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

    [SerializeField] private int maxCheckPerRay = 6;

    [SerializeField] private Rigidbody rb;
    [SerializeField] Camera mainCamera;

    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private float speed = 5.0f;

    public RoomManager roomManager;

    private Vec3 moveDirection;

    [SerializeField] private List<Line> playerRay;

    [SerializeField] private float rayLength = 15.0f;

    private float xAngle;
    private float yAngle;

    private float pitch;
    private float yaw;
    
    private void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        
        float tanHalfFov = Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float yMax = tanHalfFov; 
        float xMax = yMax * mainCamera.aspect;

        for (int i = 0; i <= xMaxRays; i++)
        {
            xAngle = Mathf.Lerp(-xMax, xMax, (float)i / xMaxRays);

            for (int j = 0; j <= yMaxRays; j++)
            {
                yAngle = Mathf.Lerp(-yMax, yMax, (float)j / yMaxRays);

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
        {
            direction += new Vec3(-transform.forward);
        }

        if (Input.GetKey(KeyCode.S))
        {
            direction += new Vec3(transform.forward);
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction += new Vec3(transform.right);
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction += new Vec3(-transform.right);
        }

        moveDirection = direction.normalized;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        yaw += mouseX * mouseSensitivity;
        pitch += mouseY * mouseSensitivity; 

        pitch = Mathf.Clamp(pitch, -89, 89);

        rb.rotation = Quaternion.Euler(pitch, yaw, 0f);
        
        foreach (Line line in playerRay)
        {
            line.direction = new Vec3(mainCamera.transform.rotation * line.originalDirection);
        }

        foreach (Line line in playerRay)
        {
            Vec3 origin = new Vec3(mainCamera.transform.position);

            for (int i = 0; i <= maxCheckPerRay; i++)
            {
                HashSet<Room> alreadyCheckedRooms = new HashSet<Room>();
                Vec3 point = Vec3.Lerp(line.originalPos, line.direction * rayLength, (float)i / maxCheckPerRay);
                roomManager.CheckPointOnCurrentRoom(point + origin, alreadyCheckedRooms);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vec3 origin = new Vec3(mainCamera.transform.position);

        foreach (Line line in playerRay)
        {
            Gizmos.DrawRay(origin + line.originalPos, line.direction * rayLength);

            for (int i = 0; i <= maxCheckPerRay; i++)
            {
                Vec3 pointToCheck = Vec3.Lerp(line.originalPos, line.direction * rayLength, (float)i / maxCheckPerRay);
                Gizmos.DrawWireSphere(pointToCheck + origin, 0.5f);
            }
        }
    }
}