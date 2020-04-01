using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonPause : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform button;
    public float sizeOnHover = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // change size to 
        button.localScale = sizeOnHover * Vector3.one;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.localScale = Vector3.one;
    }

    private void OnDisable()
    {
        button.localScale = Vector3.one;
    }






}
