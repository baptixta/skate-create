using UnityEngine;
using UnityEngine.EventSystems;

[SelectionBase]
public class ElementCard : Card
{
    public Element element;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        if (GetComponentInParent<CardContainer>() != null)
        {
            GameObject clone = Instantiate(gameObject, transform.parent);
            clone.transform.SetSiblingIndex(transform.GetSiblingIndex());
            clone.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        transform.position = eventData.position + offset;
    }

    override public void OnEndDrag(PointerEventData eventData)
    {

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
}
