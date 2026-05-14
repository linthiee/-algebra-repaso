using CustomMath;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] Camera mainCamera;

    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private float speed = 5.0f;

    public RoomManager currentRoom;
    public Transform testDoor;

    private Vec3 moveDirection;

    private Vec3[] playerRay = new Vec3[1];

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

        Vec3 forwardDirection = new Vec3(mainCamera.transform.rotation * Vec3.Forward);

        playerRay[0] = forwardDirection;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vec3 origin = new Vec3(mainCamera.transform.position);
        Gizmos.DrawLine(origin, origin + (playerRay[0] * 5.0f));
    }
}