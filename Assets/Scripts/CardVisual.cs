using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class CardVisual : MonoBehaviour
{
    private Card parentCard;
    private TextMeshProUGUI cardLabel;
    public GameObject newElementIndicator;

    void Start()
    {
        parentCard = GetComponentInParent<ElementCard>();
        cardLabel = GetComponentInChildren<TextMeshProUGUI>();

        OnElementChange();

        //Event Listening
        parentCard.OnHover.AddListener(OnHover);
        parentCard.OnSelect.AddListener(OnSelect);
        parentCard.OnElementChange.AddListener(OnElementChange);
        parentCard.OnCombination.AddListener(OnCombination);

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
        cardLabel.text = parentCard.GetComponent<ElementCard>().element.name;
    }

    private void OnCombination()
    {
        transform.DOPunchRotation(Vector3.one * 60, .2f, 10, 1);
    }

    private void OnDestroy()
    {
        DOTween.Complete(transform);
    }

}
