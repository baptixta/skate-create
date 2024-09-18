using UnityEngine;

[CreateAssetMenu(fileName = "Element", menuName = "Alchemy/Element")]
[System.Serializable]
public class Element : ScriptableObject
{
    public string elementName;
    public Sprite icon;
}