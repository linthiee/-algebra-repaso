using UnityEngine;
using CustomMath;

public class Player : MonoBehaviour
{
    [SerializeField] Transform playerPos;
    [SerializeField] Camera mainCamera;

    private float mouseSensitivity = 3.0f;
    private float speed = 5.0f;
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            playerPos.position += speed * Time.deltaTime * -playerPos.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerPos.position += speed * Time.deltaTime * playerPos.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerPos.position += speed * Time.deltaTime * playerPos.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerPos.position += speed * Time.deltaTime * -playerPos.right;
        }

        Vector3 playerScreenPos = mainCamera.WorldToScreenPoint(transform.position);

        float deltaX = Input.mousePosition.x - playerScreenPos.x;
        float deltaY = Input.mousePosition.y - playerScreenPos.y;

        float angle = Mathf.Atan2(deltaX, deltaY) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, -angle * mouseSensitivity, 0f);
    }
}