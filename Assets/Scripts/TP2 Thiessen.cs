using CustomMath;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TP2Thiessen : MonoBehaviour
{
    [SerializeField] private float boundingPlaneSize = 5.0f;

    [SerializeField] private int numberOfPoints;

    private List<ThiessenPoints> myPoints = new List<ThiessenPoints>();

    [SerializeField] private GameObject planePrefab;

    private GameObject myPlaneInstance;

    private const int size = 6;

    private MyPlane[] boundingPlanes = new MyPlane[size];

    [SerializeField] private Transform playerTransform;
    private ThiessenPoints activePoint = null;

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
            DrawPlane(boundingPlanes[i], Color.cyan);
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

            float r = UnityEngine.Random.Range(0f, 1f);
            float g = UnityEngine.Random.Range(0f, 1f);
            float b = UnityEngine.Random.Range(0f, 1f);

            newPoint.color = new Color(r, g, b, 0.4f);

            myPoints.Add(newPoint);
        }

        for (int i = 0; i < myPoints.Count; i++)
        {
            for (int j = 0; j < myPoints.Count; j++)
            {
                if (i == j)
                    continue;

                Vec3 pA = myPoints[i].position;
                Vec3 pB = myPoints[j].position;

                Vec3 midPoint = (pA + pB) * 0.5f;

                Vec3 normalPointingTo = (pA - pB).normalized;

                MyPlane mediatrixPlane = new MyPlane(normalPointingTo, midPoint);

                myPoints[i].boundingPlanes.Add(mediatrixPlane);
            }
        }

        OptimizePlanes();

        foreach (ThiessenPoints point in myPoints)
        {
            for (int p = size; p < point.boundingPlanes.Count; p++)
            {
                DrawPlane(point.boundingPlanes[p], point.color);
            }
        }
    }

    private void Update()
    {
        if (playerTransform == null)
            return;

        Vec3 playerPos = new Vec3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);
        activePoint = null;

        foreach (ThiessenPoints point in myPoints)
        {
            if (point.ContainsPoint(playerPos))
            {
                activePoint = point;
                break;
            }
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
            Gizmos.color = (point == activePoint) ? Color.yellow : Color.green;

            Gizmos.DrawSphere(point.position, 0.5f);

            if (point == activePoint)
            {
                Gizmos.color = Color.magenta;
                for (int p = size; p < point.boundingPlanes.Count; p++)
                {
                    MyPlane activePlane = point.boundingPlanes[p];
                    Vec3 center = -activePlane.normal * activePlane.distance;

                    Gizmos.DrawLine(center, center + (activePlane.normal * 3.0f));
                }
            }
        }
    }

    private MyPlane CreatePlane(Vec3 normal, float d)
    {
        MyPlane customPlane = new MyPlane(normal, d);

        return customPlane;
    }

    private void DrawPlane(MyPlane plane, Color color)
    {
        Vec3 customCenter = -new Vec3(plane.normal.x, plane.normal.y, plane.normal.z) * plane.distance;
        Quaternion customRotation = Quaternion.FromToRotation(Vector3.up, plane.normal);

        if (planePrefab != null)
        {
            myPlaneInstance = Instantiate(planePrefab, customCenter, customRotation);

            Renderer planeRenderer = myPlaneInstance.GetComponent<Renderer>();

            if (planeRenderer != null)
            {
                planeRenderer.material.color = color;
            }
        }
    }

    private void OptimizePlanes()
    {
        int totalPlanesBefore = 0;
        int totalPlanesAfter = 0;

        foreach (ThiessenPoints point in myPoints)
        {
            List<MyPlane> globalPlanes = point.boundingPlanes.GetRange(0, size);
            List<MyPlane> bisectorPlanes = point.boundingPlanes.GetRange(size, point.boundingPlanes.Count - size);

            totalPlanesBefore += point.boundingPlanes.Count;

            bisectorPlanes = bisectorPlanes.OrderBy(p => Mathf.Abs(Vec3.Dot(p.normal, point.position) + p.distance)).ToList();

            List<MyPlane> optimizedBisectors = new List<MyPlane>();

            foreach (MyPlane candidatePlane in bisectorPlanes)
            {
                bool isRedundant = false;

                float candidateDist = Mathf.Abs(Vec3.Dot(candidatePlane.normal, point.position) + candidatePlane.distance);

                foreach (MyPlane acceptedPlane in optimizedBisectors)
                {
                    float normalDot = Vec3.Dot(candidatePlane.normal, acceptedPlane.normal);

                    if (normalDot > 0.85f)
                    {
                        float acceptedDist = Mathf.Abs(Vec3.Dot(acceptedPlane.normal, point.position) + acceptedPlane.distance);

                        if (candidateDist >= acceptedDist)
                        {
                            isRedundant = true;
                            break;
                        }
                    }
                }

                if (!isRedundant)
                {
                    optimizedBisectors.Add(candidatePlane);
                }
            }

            point.boundingPlanes.Clear();
            point.boundingPlanes.AddRange(globalPlanes);
            point.boundingPlanes.AddRange(optimizedBisectors);
            totalPlanesAfter += point.boundingPlanes.Count;
        }

        Debug.Log($"reduced total planes from {totalPlanesBefore} to {totalPlanesAfter}.");
    }
}
