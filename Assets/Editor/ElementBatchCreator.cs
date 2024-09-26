using UnityEngine;
using UnityEditor;
using System.IO;

public class ElementBatchCreator : EditorWindow
{
    private TextAsset csvFile;  // CSV file reference
    private string folderPath = "Assets/Resources/Elements/"; // Default folder to save ScriptableObjects

    [MenuItem("Tools/Batch Create Elements")]
    public static void ShowWindow()
    {
        GetWindow<ElementBatchCreator>("Batch Create Elements");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Create Element ScriptableObjects", EditorStyles.boldLabel);

        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);
        folderPath = EditorGUILayout.TextField("Save Path", folderPath);

        if (GUILayout.Button("Create Elements"))
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

            if (row.Length != 3)
            {
                Debug.LogError("Invalid row in CSV: " + line);
                continue;
            }

            string elementName = row[0].Trim();
            string description = row[1].Trim();
            string category = row[2].Trim();

            CreateScriptableObject(elementName, description, category);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void CreateScriptableObject(string elementName, string description, string category)
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
        newElement.description = description;
        newElement.category = category;

        // Save it to the specified path
        string assetPath = folderPath + elementName + ".asset";
        AssetDatabase.CreateAsset(newElement, assetPath);
        Debug.Log("Created Element: " + elementName);
    }
}
