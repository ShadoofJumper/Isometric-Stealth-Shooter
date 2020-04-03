using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private float offsetX = -1;
    private float openX;
    private float closedX;
    private LTDescr tweenDoorMove;
    public LTDescr TweenDoorMove { get { return tweenDoorMove; } }

    public int doorId;

    private void Start()
    {
        GameManager.instance.onDoorwayTriggerEnter += DoorOpen;
        GameManager.instance.onDoorwayTriggerExit  += CloseDoor;

        openX   = transform.localPosition.x + offsetX;
        closedX = transform.localPosition.x;
    }

    public void DoorOpen(int doorId, float timeOpen)
    {
        if(doorId == this.doorId)
        {
            tweenDoorMove = LeanTween.moveLocalX(gameObject, openX, timeOpen).setEaseInQuad();
        }
    }

    public void CloseDoor(int doorId, float timeClose)
    {
        if (doorId == this.doorId)
        {
            tweenDoorMove = LeanTween.moveLocalX(gameObject, closedX, timeClose).setEaseInQuad();
        }
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
        if (GameManager.instance != null)
        {
            GameManager.instance.onDoorwayTriggerEnter  -= DoorOpen;
            GameManager.instance.onDoorwayTriggerExit   -= CloseDoor;
        }
    }
}
