using CustomMath;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform doorTransform;

    public Room connectedRoom;

    public MyPlane[] planes;

    private void Start()
    {
        planes = new MyPlane[2];

        Vec3 rightDirection = new Vec3(doorTransform.rotation * Vec3.Right);

        planes[0] = new MyPlane(rightDirection, new Vec3(doorTransform.position + (rightDirection * 0.5f))); //right
        planes[1] = new MyPlane(-rightDirection, new Vec3(doorTransform.position - (rightDirection * 0.5f))); //left
    }
    public bool PassThrough(Vec3 checkPosition)
    {
        foreach (MyPlane plane in planes)
        {
            if (!plane.GetSide(checkPosition))
            {
                return false;
            }
        }
        return true;
    }
}
