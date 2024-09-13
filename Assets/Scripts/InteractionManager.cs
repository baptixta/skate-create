using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance { get; private set; }

    public ElementCard selectedCard = null;
    public ElementCard hoveredCard = null;

    private void Awake()
    {
        instance = this;
    }

    internal void TryInteraction()
    {
        if (selectedCard != null && hoveredCard != null)
        {
            Element combination = CombinationManager.instance.GetCombinationResult(selectedCard.element, hoveredCard.element);
            if (combination != null)
            {
                Destroy(hoveredCard.gameObject);
                selectedCard.UpdateElement(combination);
            }
            else
            {
                print("no combination");
            }
        }
    }
}
