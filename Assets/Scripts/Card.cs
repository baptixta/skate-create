using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[SelectionBase]
[RequireComponent(typeof(CanvasGroup))]
public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("References")]
    [HideInInspector] public CanvasGroup canvasGroup;

    [Header("Events")]
    [HideInInspector] public UnityEvent OnSelect;
    [HideInInspector] public UnityEvent<bool> OnHover;
    [HideInInspector] public UnityEvent<string> OnElementChange;
    [HideInInspector] public UnityEvent OnCombination;
    [HideInInspector] public UnityEvent<bool> OnUnlock;

    private bool isDragging;

    [Header("Variables")]
    public Vector2 offset;

    public virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (isDragging)
            ClampPosition();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        InteractionManager.instance.selectedCard = this;
        isDragging = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {

    }

    void ClampPosition()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        // Get the pivot-adjusted size of the RectTransform
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        // Get screen dimensions in screen space
        Vector2 screenBounds = new Vector2(Screen.width, Screen.height);

        // Get the current position of the RectTransform in screen space
        Vector3 clampedPosition = rectTransform.position;

        // Calculate clamping ranges based on pivot and size
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, width / 2, screenBounds.x - width / 2);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, height / 2, screenBounds.y - height / 2);

        // Apply the clamped position back to the RectTransform
        rectTransform.position = clampedPosition;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        InteractionManager.instance.selectedCard = null;
        isDragging = false;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        OnHover.Invoke(true);
        if (InteractionManager.instance.selectedCard != null)
            InteractionManager.instance.hoveredCard = this;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        OnHover.Invoke(false);
        if (InteractionManager.instance.selectedCard != null)
            InteractionManager.instance.hoveredCard = null;
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        offset = (Vector2)transform.position - eventData.position;
    }
}
