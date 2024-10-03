using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    private Card parentCard;
    public Image icon;
    public GameObject newElementIndicator;

    [Header("Icons")]
    public Sprite bodySprite;
    public Sprite skateSprite;
    public Sprite trickSprite;
    public Sprite jokerSprite;
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI cardLabel;
    [SerializeField] private TextMeshProUGUI multiplierLabel;


    void Awake()
    {
        parentCard = GetComponentInParent<Card>();

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
        {
            SwitchIcon(elementCard.element);
        }
    }

    private void ActivateOverlays(Element element)
    {
        string[] overlays = element.overlays.Split(';');

        foreach (string overlay in overlays)
        {
            //print(overlay);
            if (icon.transform.Find(overlay) != null)
            {
                icon.transform.Find(overlay).GetComponent<Image>().enabled = true;
            }
        }
    }

    private void SetMultiplierLabel(Element element)
    {
        string baseText = "<mark=#FF9090 padding=“20, 20, 0, 0”>";
        baseText += element.multiplier;
        if (element.multiplier != string.Empty)
            baseText += 'X';
        multiplierLabel.text = baseText;
    }


    private void SwitchIcon(Element element)
    {
        ActivateOverlays(element);
        SetMultiplierLabel(element);
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
