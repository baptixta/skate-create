using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndMove : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Vector2 lastMousePosition;

    private void Awake()
    {
        // Ensure that the GameObject has a RectTransform component
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("This script requires a RectTransform component.");
        }
    }

    // Called when the mouse pointer clicks down on the UI element
    public void OnPointerDown(PointerEventData eventData)
    {
        lastMousePosition = eventData.position;
    }

    // Called when the mouse pointer is dragged
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentMousePosition = eventData.position;
        Vector2 difference = currentMousePosition - lastMousePosition;

        // Move the UI element by the difference
        rectTransform.anchoredPosition += difference;

        // Update the last mouse position
        lastMousePosition = currentMousePosition;
    }

    // Called when the mouse pointer is released
    public void OnPointerUp(PointerEventData eventData)
    {
        // You can add any additional logic when the user stops dragging, if needed
    }
}
