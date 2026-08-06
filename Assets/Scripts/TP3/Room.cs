using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class Room : MonoBehaviour
{
    public List<GameObject> insideObjects;
    public List<Door> doors;

    public bool hasBeenChecked = false;

    public Transform roomVolume;

    public bool isVisible = false;

    public GameObject visibleChildren;

    public void InitializeDoors()
    {
        if (doors != null)
        {
            foreach (Door door in doors)
            {
                door.Init();
            }
        }
    }

    public void SetVisible(bool state)
    {
        isVisible = state;

        visibleChildren.SetActive(isVisible);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isVisible ? Color.green : new Color(1f, 0f, 0f, 0.3f);

        if (roomVolume != null)
        {
            Gizmos.DrawWireCube(roomVolume.position, roomVolume.localScale);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, Vec3.One * 10f); 
        }
        if (doors != null)
        {
            foreach (Door door in doors)
            {
                door.DrawGizmos();
            }
        }
    }
}