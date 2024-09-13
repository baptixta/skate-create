using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardVisual : MonoBehaviour
{
    private ElementCard parentCard;
    private TextMeshProUGUI cardLabel;

    void Start()
    {
        parentCard = GetComponentInParent<ElementCard>();
        cardLabel = GetComponentInChildren<TextMeshProUGUI>();

        OnElementChange();

        //Event Listening
        parentCard.OnHover.AddListener(OnHover);
        parentCard.OnSelect.AddListener(OnSelect);
        parentCard.OnElementChange.AddListener(OnElementChange);

    }

    private void OnHover(bool hover)
    {
        transform.DOScale(hover ? 1.2f : 1, .2f).SetEase(Ease.OutBack);
    }

    private void OnSelect()
    {
        transform.DOScale(1.2f, .2f).SetEase(Ease.OutBack);
    }

    private void OnElementChange()
    {
        cardLabel.text = parentCard.element.name;
    }
}
