using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject elementCardPrefab;
    [SerializeField] private Transform elementContainer;
    [SerializeField] private TextMeshProUGUI unlockedLabel;

    [Header("Unlocked")]
    [SerializeField] private Element[] unlockedElements;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        LoadInitialElementsFromCSV("initialElements");

        if (ES3.KeyExists("unlockedElements"))
            Load();
        else
            Save();

        //Create card gameobjects
        for (int i = 0; i < unlockedElements.Length; i++)
        {
            ElementCard card = Instantiate(elementCardPrefab, elementContainer).GetComponent<ElementCard>();
            card.UpdateElement(unlockedElements[i]);
            card.gameObject.name = card.element.elementName;
        }

        UpdateDiscoveryLabel();

    }

    private void Load()
    {
        print("save file exists");

        string[] elementNames = ES3.Load<string>("unlockedElements").Split(',');
        List<Element> elementsList = new List<Element>();

        foreach (string elementName in elementNames)
        {

            string trimmedName = elementName.Trim();
            Element element = Resources.Load<Element>("Elements/" + trimmedName);

            if (element != null)
                elementsList.Add(element);
            else
                Debug.LogError($"Element '{trimmedName}' not found in Resources.");
        }

        unlockedElements = elementsList.ToArray();
    }

    private void Save()
    {
        string initialElements = string.Empty;

        for (int i = 0; i < unlockedElements.Length; i++)
        {
            initialElements += unlockedElements[i].elementName;
            if (i != unlockedElements.Length - 1)
                initialElements += ",";
        }
        //print(initialElements);

        ES3.Save<string>("unlockedElements", initialElements);
    }

    public void TryUnlockCombination(Element combination)
    {
        for (int i = 0; i < unlockedElements.Length; i++)
        {
            if (unlockedElements[i] == combination)
                return;
        }

        print("new combination");

        Element[] newArray = new Element[unlockedElements.Length + 1];
        Array.Copy(unlockedElements, newArray, unlockedElements.Length);
        newArray[unlockedElements.Length] = combination;
        unlockedElements = newArray;

        ElementCard unlockedCard = Instantiate(elementCardPrefab, elementContainer).GetComponent<ElementCard>();
        unlockedCard.UpdateElement(combination, true);

        UpdateDiscoveryLabel();

        Save();
    }

    private void UpdateDiscoveryLabel()
    {
        unlockedLabel.text = unlockedElements.Length.ToString() + "/" + GetUniqueElementCountFromCSV("combinations").ToString();
    }

    public void LoadInitialElementsFromCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName); // Load the CSV file
        if (csvFile == null)
        {
            Debug.LogError("Initial elements CSV file not found in Resources folder.");
            return;
        }

        StringReader reader = new StringReader(csvFile.text);

        string line;
        List<Element> elementList = new List<Element>();

        while ((line = reader.ReadLine()) != null)
        {

            string elementName = line.Trim();

            // Load the ScriptableObject for the element
            Element element = Resources.Load<Element>("Elements/" + elementName);

            if (element != null)
            {
                elementList.Add(element);
            }
            else
            {
                Debug.LogError($"Element '{elementName}' not found in Resources.");
            }
        }

        // Convert the list to an array and store it in initialElements
        unlockedElements = elementList.ToArray();

        Debug.Log($"Loaded {unlockedElements.Length} initial elements.");
    }

    public int GetUniqueElementCountFromCSV(string fileName)
    {
        // Load the CSV file from Resources
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found.");
            return 0;
        }

        StringReader reader = new StringReader(csvFile.text);
        HashSet<string> uniqueElements = new HashSet<string>();

        string line;
        bool firstLine = true;
        while ((line = reader.ReadLine()) != null)
        {
            // Skip the first line if it's a header
            if (firstLine)
            {
                firstLine = false;
                continue;
            }

            // Split the line by comma to get the element names (Assuming CSV format is: Element1,Element2,Result)
            string[] parts = line.Split(',');

            if (parts.Length >= 2)
            {
                string element1 = parts[0].Trim();
                string element2 = parts[1].Trim();

                // Add element1 and element2 to the HashSet
                uniqueElements.Add(element1);
                uniqueElements.Add(element2);
            }
        }

        // Return the count of unique elements
        return uniqueElements.Count;
    }

}
