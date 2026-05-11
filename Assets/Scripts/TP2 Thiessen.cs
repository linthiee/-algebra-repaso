using CustomMath;
using UnityEngine;

public class TP2Thiessen : MonoBehaviour
{
    [SerializeField] private Vec3 normal = new Vec3(5.0f, 5.0f, 5.0f);
    [SerializeField] private float d = 5.0f;

    [SerializeField] private GameObject planePrefab;

    private GameObject myPlaneInstance;

    private MyPlane customPlane;

    private void Start()
    {
        customPlane = CreatePlane(normal, d);
    }

    private MyPlane CreatePlane(Vec3 normal, float d)
    {
        MyPlane customPlane = new MyPlane(normal, d);

        Vec3 customCenter = -new Vec3(customPlane.normal.x, customPlane.normal.y, customPlane.normal.z) * customPlane.distance;
        Quaternion customRotation = Quaternion.FromToRotation(Vector3.up, customPlane.normal);

        if (planePrefab != null)
        {
            myPlaneInstance = Instantiate(planePrefab, customCenter, customRotation);
        }

        return customPlane;
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Vec3 center = -customPlane.normal * customPlane.distance;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(center, center + (customPlane.normal * 2.0f));

    }
}