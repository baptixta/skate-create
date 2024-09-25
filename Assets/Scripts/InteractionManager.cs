using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance { get; private set; }

    [Header("Card Logic")]
    public Card selectedCard = null;
    public Card hoveredCard = null;

    [HideInInspector] public UnityEvent OnCombination;

    private void Awake()
    {
        instance = this;
    }

    //Try Interaction between two cards
    internal void TryInteraction()
    {
        if (selectedCard != null && hoveredCard != null)
        {

            //If both cards are elements
            if (selectedCard.GetComponent<ElementCard>() != null && hoveredCard.GetComponent<ElementCard>() != null)
            {
                if (hoveredCard.GetComponentInParent<CardContainer>() != null)
                {
                    Destroy(selectedCard.gameObject);
                    return;
                }

                ElementCard selectedElementCard = selectedCard.GetComponent<ElementCard>();
                ElementCard hoveredElementCard = hoveredCard.GetComponent<ElementCard>();
                if (selectedElementCard.element == null || hoveredElementCard.element == null)
                {
                    Debug.LogError("One of the elements is null!");
                    return;
                }

                Element combination = CombinationManager.instance.GetResult(selectedElementCard.element, hoveredElementCard.element);

                if (combination != null)
                {
                    print("combination");
                    OnCombination.Invoke();

                    GameManager.instance.TryUnlockCombination(combination);
                    selectedElementCard.UpdateElement(combination);
                    selectedElementCard.CombinationComplete();
                    Destroy(hoveredCard.gameObject);
                }
                else
                {
                    print("no combination");
                    NegativeFeedback(hoveredCard.transform);
                }
            }

            //If we are dragging an Element to the delete card
            if (hoveredCard.GetComponent<ActionCard>() && selectedCard.GetComponent<ElementCard>())
            {
                ActionCard actionCard = hoveredCard.GetComponent<ActionCard>();

                if (actionCard.actionType == ActionCard.ActionType.delete)
                {
                    Destroy(selectedCard.gameObject);
                }

                if (actionCard.actionType == ActionCard.ActionType.divide)
                {
                    // if (elementCard.element.originCombination != string.Empty)
                    // {
                    //     print(elementCard.element.originCombination);
                    // }
                }
            }

            //If we are dragging an action card to an element card
            if (selectedCard.GetComponent<ActionCard>() && hoveredCard.GetComponent<ElementCard>())
            {
                ActionCard actionCard = selectedCard.GetComponent<ActionCard>();
                ElementCard elementCard = hoveredCard.GetComponent<ElementCard>();

                if (elementCard.GetComponentInParent<CardContainer>() != null)
                    return;

                if (actionCard.actionType == ActionCard.ActionType.divide)
                {
                    if (elementCard.element.originCombination != string.Empty)
                    {
                        SplitElementCard(elementCard.element.originCombination);
                        Destroy(hoveredCard.gameObject);
                    }
                }
            }
        }
    }

    public void SplitElementCard(string combination)
    {
        string[] elementNames = combination.Split(',');
        List<Element> elementsList = new List<Element>();

        foreach (string elementName in elementNames)
        {
            string trimmedName = elementName.Trim();
            Element element = Resources.Load<Element>("Elements/" + trimmedName);

            if (element != null)
                elementsList.Add(element);
            else
                Debug.LogError($"Element '{trimmedName}' not found in Resources.");
        }

        foreach (Element element in elementsList)
        {
            //TODO do this on GameManager
            GameObject clone = Instantiate(hoveredCard, MixArea.instance.transform).gameObject;
            clone.GetComponent<ElementCard>().UpdateElement(element);
            clone.GetComponent<ElementCard>().canvasGroup.blocksRaycasts = true;
            NegativeFeedback(clone.transform);
        }
    }

    void NegativeFeedback(Transform target)
    {
        Vector3 randomDirection = new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), 0);
        Vector3 localPos = MixArea.instance.rectTransform.InverseTransformPoint(hoveredCard.transform.position);
        Vector3 targetLocalPos = localPos + randomDirection;
        RectTransform hoveredCardRect = hoveredCard.GetComponent<RectTransform>();
        Vector2 cardSize = hoveredCardRect.rect.size;
        Rect mixRect = MixArea.instance.rectTransform.rect;
        float clampedX = Mathf.Clamp(targetLocalPos.x, mixRect.xMin + cardSize.x / 2, mixRect.xMax - cardSize.x / 2);
        float clampedY = Mathf.Clamp(targetLocalPos.y, mixRect.yMin + cardSize.y / 2, mixRect.yMax - cardSize.y / 2);
        Vector3 clampedWorldPos = MixArea.instance.rectTransform.TransformPoint(new Vector3(clampedX, clampedY, 0));
        target.DOMove(clampedWorldPos, .4f);
    }
}
