using DG.Tweening;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance { get; private set; }

    public ElementCard selectedCard = null;
    public ElementCard hoveredCard = null;

    public Transform elementsContainer;
    private ElementCard[] availableElements;
    public GameObject cardPrefab;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        availableElements = transform.GetComponentsInChildren<ElementCard>();

    }

    internal void TryInteraction()
    {
        if (selectedCard != null && hoveredCard != null)
        {
            if (selectedCard.element == null || hoveredCard.element == null)
            {
                Debug.LogError("One of the elements is null!");
                return;
            }

            Element combination = CombinationManager.instance.GetResult(selectedCard.element, hoveredCard.element);

            if (combination != null)
            {
                Destroy(hoveredCard.gameObject);
                selectedCard.UpdateElement(combination);
                selectedCard.CombinationComplete();

                //check if element exists in list
                for (int i = 0; i < availableElements.Length; i++)
                {
                    if (availableElements[i].element == combination)
                        return;
                }

                //unlock
                print("unlocked combination");
                ElementCard unlockedCombination = Instantiate(cardPrefab, elementsContainer).GetComponent<ElementCard>();
                unlockedCombination.UpdateElement(combination);
            }
            else
            {
                print("no combination");

                // Generate a random offset to push the card in a random direction
                Vector3 randomDirection = new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), 0);

                // Get the local position of hoveredCard relative to the MixArea
                Vector3 localPos = MixArea.instance.rectTransform.InverseTransformPoint(hoveredCard.transform.position);

                // Apply the random direction in local space
                Vector3 targetLocalPos = localPos + randomDirection;

                // Get the size of the hoveredCard RectTransform
                RectTransform hoveredCardRect = hoveredCard.GetComponent<RectTransform>();
                Vector2 cardSize = hoveredCardRect.rect.size;

                // Get the boundaries of the MixArea in local space
                Rect mixRect = MixArea.instance.rectTransform.rect;

                // Calculate the min and max bounds for clamping, considering the card size
                float clampedX = Mathf.Clamp(targetLocalPos.x, mixRect.xMin + cardSize.x / 2, mixRect.xMax - cardSize.x / 2);
                float clampedY = Mathf.Clamp(targetLocalPos.y, mixRect.yMin + cardSize.y / 2, mixRect.yMax - cardSize.y / 2);

                // Convert the clamped local position back to world space
                Vector3 clampedWorldPos = MixArea.instance.rectTransform.TransformPoint(new Vector3(clampedX, clampedY, 0));

                // Move hoveredCard to the clamped world position
                hoveredCard.transform.DOMove(clampedWorldPos, .4f);


            }
        }
    }
}