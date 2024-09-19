using System.Collections;
using DG.Tweening;
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

    public void Clean()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).DOScale(0, .1f).SetEase(Ease.InBack);
            StartCoroutine(DeleteGameobject(transform.GetChild(i).gameObject));
        }
    }

    private IEnumerator DeleteGameobject(GameObject gameObject)
    {
        yield return new WaitForSeconds(.2f);
        Destroy(gameObject);
    }
}
