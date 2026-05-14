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
    }

    [SerializeField] private Rigidbody rb;
    [SerializeField] Camera mainCamera;

    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private float speed = 5.0f;

    public RoomManager roomManager;

    private Vec3 moveDirection;

    [SerializeField] private List<Line> playerRay;

    [SerializeField] private float rayLength = 15.0f;

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

        Vec3 playerScreenPos = new Vec3(mainCamera.WorldToScreenPoint(transform.position));
        float deltaX = Input.mousePosition.x - playerScreenPos.x;
        float deltaY = Input.mousePosition.y - playerScreenPos.y;
        float angle = Mathf.Atan2(deltaX, deltaY) * Mathf.Rad2Deg;

        rb.rotation = Quaternion.Euler(0f, -angle * mouseSensitivity, 0f);

        foreach(Line line in playerRay)
        {
            line.direction = new Vec3(mainCamera.transform.rotation * Vec3.Forward);
        }

        foreach (Line line in playerRay)
        {
            Vec3 origin = new Vec3(mainCamera.transform.position);
            Vec3 point = Vec3.Lerp(line.originalPos, line.direction * rayLength, 0.5f);
            roomManager.CheckPointOnCurrentRoom(point + origin);
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
            Vec3 point = Vec3.Lerp(line.originalPos, line.direction * rayLength, 0.5f);

            Gizmos.DrawRay(origin + line.originalPos, line.direction * rayLength);
            Gizmos.DrawSphere(point + origin, 0.5f);
        }
    }
}