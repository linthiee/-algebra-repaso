using CustomMath;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
public class Frustum : MonoBehaviour 
{
    [SerializeField] float nearClip = 0.3f;
    [SerializeField] float farClip = 1000f;

    [SerializeField] Camera cam;

    public MyPlane[] planes = new MyPlane[6];

    public void UpdateFrustum()
    {
        float halfFov = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;

        float halfHeight = Mathf.Tan(halfFov);
        float halfWidth = halfHeight * cam.aspect;

        Vec3 position = new Vec3(cam.transform.position);
        Vec3 forwardDirection = new Vec3(cam.transform.rotation * Vec3.Forward);
        Vec3 rightDirection = new Vec3(cam.transform.rotation * Vec3.Right);
        Vec3 upDirection = new Vec3(cam.transform.rotation * Vec3.Up);

        planes[0] = new MyPlane(forwardDirection, position + forwardDirection * nearClip); 
        planes[1] = new MyPlane(-forwardDirection, position + forwardDirection * farClip); 

        Vec3 topNormal = Vec3.Cross(rightDirection, forwardDirection + upDirection * halfHeight).normalized;
        planes[2] = new MyPlane(topNormal, position);

        Vec3 bottomNormal = Vec3.Cross(forwardDirection - upDirection * halfHeight, rightDirection).normalized;
        planes[3] = new MyPlane(bottomNormal, position);

        Vec3 leftNormal = Vec3.Cross(upDirection, forwardDirection - rightDirection * halfWidth).normalized;
        planes[4] = new MyPlane(leftNormal, position);

        Vec3 rightNormal = Vec3.Cross(forwardDirection + rightDirection * halfWidth, upDirection).normalized;
        planes[5] = new MyPlane(rightNormal, position);
    }

    public bool IsPointInside(Vec3 point)
    {
        for (int i = 0; i < 6; i++)
        {
            if (!planes[i].GetSide(point))
            {
                return false;
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < 6; i++)
        {
            MyPlane plane = planes[i];
            Vec3 planeCenter = plane.normal * -plane.distance;
            Gizmos.DrawLine(planeCenter, planeCenter + plane.normal);
        }
    }
}