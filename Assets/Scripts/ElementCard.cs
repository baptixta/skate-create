using UnityEngine;
using UnityEngine.EventSystems;

[SelectionBase]
public class ElementCard : Card
{
    [Header("Scriptable")]
    public Element element;

    public override void Start()
    {
        base.Start();

        OnElementChange.Invoke(element.elementName);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        //Create clone
        if (GetComponentInParent<CardContainer>() != null)
        {
            Card clone = Instantiate(gameObject, transform.parent).GetComponent<Card>();
            clone.transform.SetSiblingIndex(transform.GetSiblingIndex());
            clone.canvasGroup.blocksRaycasts = true;
        }

        //Make canvas direct parent to overlay everything
        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        transform.position = eventData.position + offset;
    }

    override public void OnEndDrag(PointerEventData eventData)
    {
        InteractionManager.instance.TryInteraction();

        transform.SetParent(MixArea.instance.transform);

        //TODO Do this with events dummy
        if (GetComponentInChildren<CardVisual>() != null)
        {
            if (GetComponentInChildren<CardVisual>().newElementIndicator.activeSelf)
                GetComponentInChildren<CardVisual>().newElementIndicator.SetActive(false);
        }

        base.OnEndDrag(eventData);

    }

    public void UpdateElement(Element element, bool unlocked = false)
    {
        this.element = element;

        OnElementChange.Invoke(element.elementName);

        if (!unlocked)
            return;

        //TODO Do this with events dummy
        if (GetComponentInChildren<CardVisual>() != null)
        {
            GetComponentInChildren<CardVisual>().newElementIndicator.SetActive(true);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (GetComponentInParent<CardContainer>() == null && eventData.button == PointerEventData.InputButton.Right)
            Destroy(gameObject);
    }

    public void CombinationComplete()
    {
        OnCombination.Invoke();
    }
}
