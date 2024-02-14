using UnityEngine;
using UnityEngine.EventSystems;

public class Events : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool myBool;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        myBool = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        myBool = false;
    }

    public bool GetButtonDown() { return myBool; }
}