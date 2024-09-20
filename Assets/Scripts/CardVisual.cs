using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardVisual : MonoBehaviour
{
    private Card parentCard;
    private TextMeshProUGUI cardLabel;
    public GameObject newElementIndicator;

    void Awake()
    {
        parentCard = GetComponentInParent<Card>();
        cardLabel = GetComponentInChildren<TextMeshProUGUI>();

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

    private void OnDestroy()
    {
        DOTween.Complete(transform);
    }

    private void OnElementChange(string elementName)
    {
        cardLabel.text = elementName;
    }

}
