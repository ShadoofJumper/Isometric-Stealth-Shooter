using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private float offsetX = -1;
    private float openX;
    private float closedX;

    public float timeOpen = 1;
    public int doorId;

    private void Start()
    {
        GameManager.instance.onDoorwayTriggerEnter += DoorOpen;
        GameManager.instance.onDoorwayTriggerExit  += CloseDoor;

        openX   = transform.localPosition.x + offsetX;
        closedX = transform.localPosition.x;
    }

    public void DoorOpen(int doorId)
    {
        if(doorId == this.doorId)
            LeanTween.moveLocalX(gameObject, openX, timeOpen).setEaseInQuad();
    }

    public void CloseDoor(int doorId)
    {
        if (doorId == this.doorId)
            LeanTween.moveLocalX(gameObject, closedX, timeOpen).setEaseInQuad();
    }

    public bool IsOpened()
    {
        if (transform.localPosition.x == openX)
        {
            return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        GameManager.instance.onDoorwayTriggerEnter -= DoorOpen;
        GameManager.instance.onDoorwayTriggerExit -= CloseDoor;
    }
}
