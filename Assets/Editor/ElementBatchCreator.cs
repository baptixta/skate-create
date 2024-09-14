using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ElementBatchCreator : EditorWindow
{
    private TextAsset csvFile;  // CSV file reference
    private string folderPath = "Assets/Resources/Elements/"; // Default folder to save ScriptableObjects

    [MenuItem("Tools/Batch Create Elements and Results")]
    public static void ShowWindow()
    {
        GetWindow<ElementBatchCreator>("Batch Create Elements and Results");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Create Element and Result ScriptableObjects", EditorStyles.boldLabel);

        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);
        folderPath = EditorGUILayout.TextField("Save Path", folderPath);

        if (GUILayout.Button("Create Elements and Results"))
        {
            if (csvFile == null)
            {
                Debug.LogError("Please assign a CSV file.");
                return;
            }

            CreateScriptableObjectsFromCSV(csvFile);
        }
    }

    private void CreateScriptableObjectsFromCSV(TextAsset csvFile)
    {
        // Ensure the folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Read the CSV file line by line
        StringReader reader = new StringReader(csvFile.text);
        bool firstLine = true;

        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] row = line.Split(',');

            if (firstLine)
            {
                // Skip header
                firstLine = false;
                continue;
            }

            string element1Name = row[0].Trim();
            string element2Name = row[1].Trim();
            string resultName = row[2].Trim();

            CreateScriptableObject(element1Name);
            CreateScriptableObject(element2Name);
            CreateScriptableObject(resultName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    private void CreateScriptableObject(string elementName)
    {
        // Check if the ScriptableObject already exists
        Element existingElement = Resources.Load<Element>("Elements/" + elementName);
        if (existingElement != null)
        {
            Debug.Log(elementName + " already exists, skipping.");
            return;
        }

        // Create the ScriptableObject for the element
        Element newElement = CreateInstance<Element>();
        newElement.elementName = elementName;

        // Save it to the specified path
        string assetPath = folderPath + elementName + ".asset";
        AssetDatabase.CreateAsset(newElement, assetPath);
        Debug.Log("Created Element/Result: " + elementName);
    }
}
