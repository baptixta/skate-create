using UnityEngine;
using UnityEngine.EventSystems;

public class ActionCard : Card
{

    public enum ActionType { delete, clean, divide, inspec, filter }
    public ActionType actionType;

    private Transform originParent;

    public override void Start()
    {
        base.Start();
        originParent = transform.parent;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (actionType == ActionType.clean)
        {
            MixArea.instance.Clean();
        }

        if (actionType == ActionType.filter)
        {
            GameManager.instance.ChangeFilter(!GameManager.instance.categoryFilter);
        }
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        if (actionType != ActionType.divide)
            return;

        //Make canvas direct parent to overlay everything
        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (actionType == ActionType.divide)
            transform.position = eventData.position + offset;
    }

    override public void OnEndDrag(PointerEventData eventData)
    {
        InteractionManager.instance.TryInteraction();

        base.OnEndDrag(eventData);

        if (actionType != ActionType.divide)
            return;

        transform.SetParent(originParent);
        transform.localPosition = Vector2.zero;

    }
}
