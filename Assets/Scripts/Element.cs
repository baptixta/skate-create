using UnityEngine;

[CreateAssetMenu(fileName = "Element", menuName = "Alchemy/Element")]
public class Element : ScriptableObject
{
    public string elementName;
    public string description;
    public string category;
    public string overlays;
    public string multiplier;
    public string originCombination = string.Empty;
}