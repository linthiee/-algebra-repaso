using CustomMath;
using System.Collections.Generic;
using UnityEngine;

public class TP2Thiessen : MonoBehaviour
{
    [SerializeField] private float boundingPlaneSize = 5.0f;

    [SerializeField] private int numberOfPoints = 5;

    private List<ThiessenPoints> myPoints = new List<ThiessenPoints>();

    [SerializeField] private GameObject planePrefab;

    private GameObject myPlaneInstance;

    private const int size = 6;

    private MyPlane[] boundingPlanes = new MyPlane[size];
  

    private void Start()
    {
        boundingPlanes[0] = CreatePlane(Vec3.Up, boundingPlaneSize);
        boundingPlanes[1] = CreatePlane(Vec3.Left, boundingPlaneSize);
        boundingPlanes[2] = CreatePlane(Vec3.Back, boundingPlaneSize);

        for (int i = 0; i < size / 2; i++)
        {
            boundingPlanes[i + 3] = boundingPlanes[i].flipped();
        }

        for (int i = 0; i < size; i++)
        {
            DrawPlane(boundingPlanes[i]);
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            float limit = boundingPlaneSize * 0.9f;
            float randomX = UnityEngine.Random.Range(-limit, limit);
            float randomY = UnityEngine.Random.Range(-limit, limit);
            float randomZ = UnityEngine.Random.Range(-limit, limit);

            ThiessenPoints newPoint = new ThiessenPoints();
            newPoint.position = new Vec3(randomX, randomY, randomZ);

            for (int j = 0; j < size; j++)
            {
                newPoint.boundingPlanes.Add(boundingPlanes[j]);
            }

            myPoints.Add(newPoint);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.blue;

        for (int i = 0; i < size; i++)
        {
            Vec3 center = -boundingPlanes[i].normal * boundingPlanes[i].distance;
            Gizmos.DrawLine(center, center + (boundingPlanes[i].normal * 2.0f));
        }

        Gizmos.color = Color.green;

        foreach (ThiessenPoints point in myPoints)
        {
            Gizmos.DrawSphere(point.position, 0.5f);
        }
    }
    private MyPlane CreatePlane(Vec3 normal, float d)
    {
        MyPlane customPlane = new MyPlane(normal, d);

        return customPlane;
    }
    private void DrawPlane(MyPlane plane)
    {
        Vec3 customCenter = -new Vec3(plane.normal.x, plane.normal.y, plane.normal.z) * plane.distance;
        Quaternion customRotation = Quaternion.FromToRotation(Vector3.up, plane.normal);

        if (planePrefab != null)
        {
            myPlaneInstance = Instantiate(planePrefab, customCenter, customRotation);
        }
    }
}