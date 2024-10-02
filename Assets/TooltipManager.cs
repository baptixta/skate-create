using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class TooltipManager : MonoBehaviour
{

    // Reference to the localized string
    public LocalizedString localizedString;

    // Reference to the Localize String Event
    public LocalizeStringEvent localizeStringEvent;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        InteractionManager.instance.OnHover.AddListener(OnHover);

        // Set the localized string reference, typically done via inspector or dynamically
        // You can set the localization key dynamically too
        localizedString.TableReference = "UI Table"; // Reference to the Localization Table
        localizedString.TableEntryReference = "delete-action";

        // Register for when the localized string updates
        localizedString.StringChanged += OnLocalizedStringChanged;

        // Trigger the Localize String Event to update UI with the localized string
        localizeStringEvent.StringReference = localizedString;
    }

    private void OnLocalizedStringChanged(string value)
    {
        //
    }

    private void OnHover(Card card)
    {
        if (card != null)
        {
            if (card.GetComponent<ActionCard>() != null)
            {
                string key = string.Empty;

                switch (card.GetComponent<ActionCard>().actionType)
                {
                    case ActionCard.ActionType.delete:
                        key = "delete-action";
                        break;
                    case ActionCard.ActionType.divide:
                        key = "split-action";
                        break;
                    case ActionCard.ActionType.clean:
                        key = "clear-action";
                        break;
                    case ActionCard.ActionType.filter:
                        key = "filter-action";
                        break;
                }

                if (key == String.Empty)
                {
                    return;
                }
                localizedString.TableEntryReference = key;

                rectTransform.sizeDelta = card.GetComponent<RectTransform>().sizeDelta;
                transform.position = card.transform.position;
                gameObject.SetActive(card != null);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        localizedString.StringChanged -= OnLocalizedStringChanged;
    }
}
