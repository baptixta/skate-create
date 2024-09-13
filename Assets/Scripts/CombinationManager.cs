using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CombinationManager : MonoBehaviour
{
    public static CombinationManager instance { get; private set; }
    private Dictionary<string, Element> combinations = new Dictionary<string, Element>();

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        LoadCombinationsFrom2DMatrixCSV("combinations2");
    }

    // Load CSV from Resources folder (2D matrix format)
    private void LoadCombinationsFrom2DMatrixCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName); // Load the CSV file
        StringReader reader = new StringReader(csvFile.text);

        List<string> headers = new List<string>();
        bool firstLine = true;

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] row = line.Split(',');

            // If it's the first line, we are reading the headers (element names)
            if (firstLine)
            {
                for (int i = 1; i < row.Length; i++) // Skip first empty cell
                {
                    headers.Add(row[i].Trim()); // Add element names (Fire, Water, etc.)
                }
                firstLine = false;
            }
            else
            {
                // The first column is the row's element (e.g., Fire, Water, etc.)
                string rowElementName = row[0].Trim();
                Element rowElement = Resources.Load<Element>("Elements/" + rowElementName);

                // Loop through the rest of the columns
                for (int i = 1; i < row.Length; i++)
                {
                    if (!string.IsNullOrEmpty(row[i].Trim()))
                    {
                        string columnElementName = headers[i - 1];
                        Element columnElement = Resources.Load<Element>("Elements/" + columnElementName);
                        Element resultElement = Resources.Load<Element>("Elements/" + row[i].Trim());

                        // Add the combination to the dictionary
                        AddCombination(rowElement, columnElement, resultElement);
                    }
                }
            }
        }
    }

    public void AddCombination(Element element1, Element element2, Element result)
    {
        if (element1 == null || element2 == null || result == null)
        {
            Debug.LogError("Null Element passed to AddCombination: " +
                (element1 != null ? element1.elementName : "null") + " + " +
                (element2 != null ? element2.elementName : "null") + " = " +
                (result != null ? result.elementName : "null"));
            return;
        }

        string key1 = element1.elementName + "+" + element2.elementName;
        string key2 = element2.elementName + "+" + element1.elementName;

        if (!combinations.ContainsKey(key1))
        {
            Debug.Log("Adding combination: " + key1 + " = " + result.elementName);
            combinations.Add(key1, result);
        }

        if (!combinations.ContainsKey(key2))
        {
            Debug.Log("Adding combination: " + key2 + " = " + result.elementName);
            combinations.Add(key2, result);
        }
    }

    public Element GetCombinationResult(Element element1, Element element2)
    {
        string key = element1.elementName + "+" + element2.elementName;
        if (combinations.ContainsKey(key))
        {
            return combinations[key];
        }
        return null;
    }
}
