using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ElementCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Element cardElement;
    private CanvasGroup canvasGroup;

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

        if (InteractionManager.instance.hoveredCard != null)
        {

            Element combination = CombinationManager.instance.GetCombinationResult(cardElement, InteractionManager.instance.hoveredCard.cardElement);
            print(combination);
            if (combination != null)
            {
                InteractionManager.instance.hoveredCard.UpdateElement(combination);
                Destroy(gameObject);
            }
        }
        InteractionManager.instance.selectedCard = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InteractionManager.instance.selectedCard != null)
        {
            InteractionManager.instance.hoveredCard = this;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InteractionManager.instance.selectedCard != null)
        {
            InteractionManager.instance.hoveredCard = null;
        }
    }

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        GetComponentInChildren<TextMeshProUGUI>().text = cardElement.name;
    }

    public void UpdateElement(Element element)
    {
        cardElement = element;
        GetComponentInChildren<TextMeshProUGUI>().text = cardElement.name;
    }
}
