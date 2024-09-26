using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CombinationManager : MonoBehaviour
{
    public static CombinationManager instance { get; private set; }
    private Dictionary<(Element, Element), Element> combinations = new Dictionary<(Element, Element), Element>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadCombinationsFromCSV("combinations");
    }

    // Load the combinations from the simplified CSV format
    private void LoadCombinationsFromCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName); // Load the CSV file
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found in Resources folder.");
            return;
        }

        StringReader reader = new StringReader(csvFile.text);

        string line;
        bool firstLine = true;

        while ((line = reader.ReadLine()) != null)
        {
            // Skip the first line if it's the header
            if (firstLine)
            {
                firstLine = false;
                continue;
            }

            // Split the line into three parts: Element1, Element2, Result
            string[] row = line.Split(',');

            if (row.Length != 3)
            {
                Debug.LogError("Invalid row in CSV: " + line);
                continue;
            }

            string element1Name = row[0].Trim();
            string element2Name = row[1].Trim();
            string resultName = row[2].Trim();

            // Load the ScriptableObjects for the elements and the result
            Element element1 = Resources.Load<Element>("Elements/" + element1Name);
            Element element2 = Resources.Load<Element>("Elements/" + element2Name);
            Element result = Resources.Load<Element>("Elements/" + resultName);

            // Log errors if any element or result is missing
            if (element1 == null || element2 == null || result == null)
            {
                Debug.LogWarning($"Error loading elements from CSV: {element1Name}, {element2Name}, or {resultName} is missing.");
                continue;
            }

            // Add the combination to the dictionary
            AddCombination(element1, element2, result);
        }
    }

    // Adds the combination to the dictionary, with debug logs
    private void AddCombination(Element element1, Element element2, Element result)
    {
        if (element1 == null || element2 == null || result == null)
        {
            Debug.LogError($"Null Element passed to AddCombination: {element1?.elementName} + {element2?.elementName} = {result?.elementName}");
            return;
        }

        // Add combination in both directions (e.g., Fire + Water = Steam, Water + Fire = Steam)
        combinations[(element1, element2)] = result;
        combinations[(element2, element1)] = result;

        // Set the originCombination field in the result element
        result.originCombination = $"{element1.elementName},{element2.elementName}";

        // Debug logs for tracking which combinations were added
        Debug.Log($"Combination added: {element1.elementName} + {element2.elementName} = {result.elementName}");
    }

    // Check for the result of combining two elements
    public Element GetResult(Element element1, Element element2)
    {
        if (combinations.TryGetValue((element1, element2), out Element result))
        {
            return result;
        }
        return null; // No combination found
    }
}
