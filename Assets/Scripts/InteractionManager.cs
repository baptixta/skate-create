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

}
