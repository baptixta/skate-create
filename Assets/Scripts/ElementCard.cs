using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[SelectionBase]
public class ElementCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Element element;
    private CanvasGroup canvasGroup;

    [Header("Events")]
    [HideInInspector] public UnityEvent OnSelect;
    [HideInInspector] public UnityEvent<bool> OnHover;
    [HideInInspector] public UnityEvent OnElementChange;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        InteractionManager.instance.selectedCard = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Mouse.current.position.value;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        InteractionManager.instance.TryInteraction();
        InteractionManager.instance.selectedCard = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover.Invoke(true);
        if (InteractionManager.instance.selectedCard != null)
            InteractionManager.instance.hoveredCard = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHover.Invoke(false);
        if (InteractionManager.instance.selectedCard != null)
            InteractionManager.instance.hoveredCard = null;
    }


    public void UpdateElement(Element element)
    {
        this.element = element;
        OnElementChange.Invoke();
    }
}
