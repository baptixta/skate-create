using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

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
        parentCard.OnCombination.AddListener(OnCombination);
        parentCard.OnUnlock.AddListener(OnUnlock);
    }

    private void OnUnlock(bool unlocked)
    {
        newElementIndicator.SetActive(unlocked);
        if (unlocked)
            transform.DOScale(0, .2f).SetEase(Ease.OutBack).From();
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

    private void OnCombination()
    {
        transform.DOPunchRotation(Vector3.one * 60, .2f, 10, 1);
    }

}
