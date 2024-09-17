using UnityEngine;

public class MixArea : MonoBehaviour
{

    public static MixArea instance { get; private set; }
    public RectTransform rectTransform;

    private void Awake()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
    }
}
