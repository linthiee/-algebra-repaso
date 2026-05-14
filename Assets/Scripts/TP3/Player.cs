using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] Camera mainCamera;

    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private float speed = 5.0f;

    private Vector3 moveDirection;

    private void Update()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += -transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += -transform.right;
        }

        moveDirection = direction.normalized;

        Vector3 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        float deltaX = Input.mousePosition.x - playerScreenPos.x;
        float deltaY = Input.mousePosition.y - playerScreenPos.y;
        float angle = Mathf.Atan2(deltaX, deltaY) * Mathf.Rad2Deg;

        rb.rotation = Quaternion.Euler(0f, -angle * mouseSensitivity, 0f);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }
}