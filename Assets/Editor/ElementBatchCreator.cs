using UnityEngine;
using UnityEditor;
using System.IO;

public class ElementBatchCreator : EditorWindow
{
    private TextAsset csvFile;  // CSV file reference
    private string folderPath = "Assets/Resources/Elements/"; // Default folder to save ScriptableObjects
    private bool deletePreviousObjects = false; // Toggle to delete previous ScriptableObjects

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

        // Toggle for deleting previous objects
        deletePreviousObjects = EditorGUILayout.Toggle("Delete Previous Objects", deletePreviousObjects);

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

        // Delete existing ScriptableObjects if the toggle is enabled
        if (deletePreviousObjects)
        {
            DeleteAllScriptableObjectsInFolder();
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

            // Ensure the row contains 5 elements (elementName, description, category, overlays, multiplier)
            if (row.Length != 5)
            {
                Debug.LogError("Invalid row in CSV: " + line);
                continue;
            }

            string elementName = row[0].Trim();
            string description = row[1].Trim();
            string category = row[2].Trim();
            string overlays = row[3];
            string multiplier = row[4].Trim();

            CreateScriptableObject(elementName, description, category, overlays, multiplier);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void CreateScriptableObject(string elementName, string description, string category, string overlays, string multiplier)
    {
        // Check if the ScriptableObject already exists
        Element existingElement = Resources.Load<Element>("Elements/" + elementName);
        if (existingElement != null)
        {
            Debug.Log(elementName + " already exists, skipping.");
            return;
        }

        // Create a new Element ScriptableObject
        Element newElement = ScriptableObject.CreateInstance<Element>();
        newElement.elementName = elementName;
        newElement.description = description;
        newElement.category = category;
        newElement.overlays = overlays;
        newElement.multiplier = multiplier;

        // Save the ScriptableObject as an asset
        string assetPath = Path.Combine(folderPath, elementName + ".asset");
        AssetDatabase.CreateAsset(newElement, assetPath);
    }

    private void DeleteAllScriptableObjectsInFolder()
    {
        // Get all assets in the folder
        string[] assetPaths = AssetDatabase.FindAssets("t:Element", new[] { folderPath });

        foreach (string assetPath in assetPaths)
        {
            string fullPath = AssetDatabase.GUIDToAssetPath(assetPath);
            AssetDatabase.DeleteAsset(fullPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Deleted all previous scriptable objects in folder: " + folderPath);
    }
}
