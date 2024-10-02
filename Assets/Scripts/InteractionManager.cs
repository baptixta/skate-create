using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance { get; private set; }

    [Header("Card Logic")]
    public Card topCard = null;
    public Card bottomCard = null;
    public Card hoveredCard = null;

    [HideInInspector] public UnityEvent OnCombination;
    [HideInInspector] public UnityEvent<Card> OnHover;

    private void Awake()
    {
        instance = this;
    }

    public void SetHoveredCard(Card card)
    {
        OnHover.Invoke(card);
        hoveredCard = card;
    }

    //Try Interaction between two cards
    internal void TryInteraction()
    {
        if (topCard != null && bottomCard != null)
        {

            //If both cards are elements
            if (topCard.GetComponent<ElementCard>() != null && bottomCard.GetComponent<ElementCard>() != null)
            {
                if (bottomCard.GetComponentInParent<CardContainer>() != null)
                {
                    Destroy(topCard.gameObject);
                    return;
                }

                ElementCard selectedElementCard = topCard.GetComponent<ElementCard>();
                ElementCard hoveredElementCard = bottomCard.GetComponent<ElementCard>();
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
                    Destroy(bottomCard.gameObject);
                }
                else
                {
                    print("no combination");
                    NegativeFeedback(bottomCard.transform);
                }
            }

            //If we are dragging an Element to the delete card
            if (bottomCard.GetComponent<ActionCard>() && topCard.GetComponent<ElementCard>())
            {
                ActionCard actionCard = bottomCard.GetComponent<ActionCard>();

                if (actionCard.actionType == ActionCard.ActionType.delete)
                {
                    Destroy(topCard.gameObject);
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
            if (topCard.GetComponent<ActionCard>() && bottomCard.GetComponent<ElementCard>())
            {
                ActionCard actionCard = topCard.GetComponent<ActionCard>();
                ElementCard elementCard = bottomCard.GetComponent<ElementCard>();

                if (elementCard.GetComponentInParent<CardContainer>() != null)
                    return;

                if (actionCard.actionType == ActionCard.ActionType.divide)
                {
                    if (elementCard.element.originCombination != string.Empty)
                    {
                        SplitElementCard(elementCard.element.originCombination);
                        Destroy(bottomCard.gameObject);
                    }
                }
            }
        }
    }

    public void SplitElementCard(string combination)
    {
        string[] elementNames = combination.Split(';');
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
            GameObject clone = Instantiate(bottomCard, MixArea.instance.transform).gameObject;
            clone.GetComponent<ElementCard>().UpdateElement(element);
            clone.GetComponent<ElementCard>().canvasGroup.blocksRaycasts = true;
            NegativeFeedback(clone.transform);
        }
    }

    void NegativeFeedback(Transform target)
    {
        // Generate a random direction
        Vector3 randomDirection = new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), 0);

        // Convert the hovered card position to local space relative to MixArea
        Vector3 localPos = MixArea.instance.rectTransform.InverseTransformPoint(bottomCard.transform.position);

        // Calculate target local position
        Vector3 targetLocalPos = localPos + randomDirection;

        // Get the size of the hovered card
        RectTransform hoveredCardRect = bottomCard.GetComponent<RectTransform>();
        Vector2 cardSize = hoveredCardRect.rect.size;

        // Get the Rect of the MixArea
        Rect mixRect = MixArea.instance.rectTransform.rect;

        // Clamp the X and Y positions based on the MixArea size and the card size
        float clampedX = Mathf.Clamp(targetLocalPos.x, mixRect.xMin + cardSize.x / 2, mixRect.xMax - cardSize.x / 2);
        float clampedY = Mathf.Clamp(targetLocalPos.y, mixRect.yMin + cardSize.y / 2, mixRect.yMax - cardSize.y / 2);

        // Convert the clamped local position back to world space
        Vector3 clampedWorldPos = MixArea.instance.rectTransform.TransformPoint(new Vector3(clampedX, clampedY, 0));

        // Move the target card to the clamped position with a smooth transition
        target.DOMove(clampedWorldPos, 0.4f);
    }

}
