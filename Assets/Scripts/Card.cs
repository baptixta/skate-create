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

    [Header("Variables")]
    public Vector2 offset;

    public virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        InteractionManager.instance.selectedCard = this;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {

    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        InteractionManager.instance.selectedCard = null;
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
