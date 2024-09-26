using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    private Card parentCard;
    private TextMeshProUGUI cardLabel;
    public Image icon;
    public GameObject newElementIndicator;

    [Header("Icons")]
    public Sprite bodySprite;
    public Sprite skateSprite;
    public Sprite trickSprite;
    public Sprite jokerSprite;

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

    private void Start()
    {
        ElementCard elementCard = parentCard.GetComponent<ElementCard>();

        if (elementCard != null)
            SwitchIcon(elementCard.element);
    }

    private void SwitchIcon(Element element)
    {
        switch (element.category)
        {
            case "Body":
                icon.sprite = bodySprite;
                return;
            case "Skate":
                icon.sprite = skateSprite;
                return;
            case "Trick":
                icon.sprite = trickSprite;
                return;
            case "Joker":
                icon.sprite = jokerSprite;
                return;
        }
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

        ElementCard elementCard = parentCard.GetComponent<ElementCard>();
        if (elementCard != null)
            SwitchIcon(elementCard.element);
    }

    private void OnCombination()
    {
        transform.DOPunchRotation(Vector3.one * 60, .2f, 10, 1);
    }

}
