using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[SelectionBase]
public class ElementCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Element element;
    private CanvasGroup canvasGroup;

    [Header("Events")]
    [HideInInspector] public UnityEvent OnSelect;
    [HideInInspector] public UnityEvent<bool> OnHover;
    [HideInInspector] public UnityEvent OnElementChange;
    [HideInInspector] public UnityEvent OnCombination;
    [HideInInspector] public UnityEvent<bool> OnUnlock;

    private Vector2 offset;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //TODO Do this with events dummy
        if (GetComponentInChildren<CardVisual>() != null)
        {
            if (GetComponentInChildren<CardVisual>().newElementIndicator.activeSelf)
                GetComponentInChildren<CardVisual>().newElementIndicator.SetActive(false);
        }

        offset = (Vector2)transform.position - Mouse.current.position.value;

        if (GetComponentInParent<CardContainer>() != null)
        {
            GameObject clone = Instantiate(gameObject, transform.parent);
            clone.transform.SetSiblingIndex(transform.GetSiblingIndex());
        }

        transform.SetParent(GetComponentInParent<Canvas>().transform);
        canvasGroup.blocksRaycasts = false;
        InteractionManager.instance.selectedCard = this;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Mouse.current.position.value + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(MixArea.instance.transform);


        if (InteractionManager.instance.hoveredCard != null)
        {
            if (InteractionManager.instance.hoveredCard.GetComponentInParent<MixArea>() == null)
            {
                Destroy(gameObject);
                InteractionManager.instance.selectedCard = null;
                return;
            }
        }

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

    public void UpdateElement(Element element, bool unlocked = false)
    {
        this.element = element;
        OnElementChange.Invoke();

        if (!unlocked)
            return;

        //TODO Do this with events dummy
        if (GetComponentInChildren<CardVisual>() != null)
        {
            GetComponentInChildren<CardVisual>().newElementIndicator.SetActive(true);
        }
    }

    public void CombinationComplete()
    {
        OnCombination.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponentInParent<CardContainer>() == null && eventData.button == PointerEventData.InputButton.Right)
        {
            Destroy(gameObject);
        }
    }
}
