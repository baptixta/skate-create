using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject elementCardPrefab;
    [SerializeField] private Transform elementContainer;
    [SerializeField] private Scrollbar verticalScroll;
    [SerializeField] private TextMeshProUGUI unlockedLabel;

    [Header("Unlocked")]
    [SerializeField] private Element[] unlockedElements;
    [SerializeField] private Element[] arrangedElements;

    public bool categoryFilter = false;

    [Header("Debug Input")]
    public InputActionReference resetAction;

    [Header("Version Settings")]
    public TextMeshProUGUI versionLabel;

    private void Awake()
    {
        instance = this;

        resetAction.action.performed += ResetAction_performed;
    }

    private void OnDisable()
    {
        resetAction.action.performed -= ResetAction_performed;
    }

    private void ResetAction_performed(InputAction.CallbackContext context)
    {
        LoadInitialElementsFromCSV("initialElements");
        Save();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    void Start()
    {

        if (ES3.KeyExists("unlockedElements"))
        {
            string[] elementNames = ES3.Load<string>("unlockedElements").Split(',');
            if (elementNames[0] != "Ollie")
            {
                LoadInitialElementsFromCSV("initialElements");
                Save();
            }
            Load();
        }
        else
        {
            LoadInitialElementsFromCSV("initialElements");
            Save();
        }

        CreateCards(unlockedElements);

        UpdateDiscoveryLabel();

        if (versionLabel != null)
            versionLabel.text = Application.version;

    }

    void CreateCards(Element[] elementList)
    {
        //Create card gameobjects
        for (int i = 0; i < elementList.Length; i++)
        {
            ElementCard card = Instantiate(elementCardPrefab, elementContainer).GetComponent<ElementCard>();
            card.UpdateElement(elementList[i]);
            card.gameObject.name = card.element.elementName;
        }
    }

    private void Load()
    {
        try
        {
            print("Attempting to load save file...");

            // Check if the save file exists and load the string
            if (ES3.KeyExists("unlockedElements"))
            {
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
            else
            {
                Debug.LogWarning("Save file not found. Initializing new save file...");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load save data: {ex.Message}. Creating a new save file...");
            ES3.DeleteKey("unlockedElements");
            Start();
        }
    }


    public void ChangeFilter(bool category = true)
    {
        categoryFilter = category;

        arrangedElements = new Element[unlockedElements.Length];

        // Sort the elements by their category
        arrangedElements = unlockedElements
            .OrderBy(e => e.category)  // Assuming 'category' is a property in Element
            .ToArray();

        if (!category)
        {
            RearrangeElementsInContainer(unlockedElements);
            return;
        }

        // Rearrange the elements inside the container without destruction
        RearrangeElementsInContainer(arrangedElements);
    }

    private void RearrangeElementsInContainer(Element[] elements)
    {

        foreach (Transform child in elementContainer)
            Destroy(child.gameObject);

        CreateCards(elements);
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
        StartCoroutine(ScrollToUnlockedElement());

        Save();
    }

    IEnumerator ScrollToUnlockedElement()
    {
        yield return new WaitForEndOfFrame();
        DOVirtual.Float(verticalScroll.value, 0, .2f, SetVerticalScroll);
    }

    public void SetVerticalScroll(float value)
    {
        verticalScroll.value = value;
    }

    private void UpdateDiscoveryLabel()
    {
        unlockedLabel.text = unlockedElements.Length.ToString() + "/" + GetUniqueElementCountFromCSV("elements").ToString();
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

            // Split the line by comma (Assuming the format is: ElementName,Description,Category)
            string[] parts = line.Split(',');

            if (parts.Length >= 1)
            {
                string elementName = parts[0].Trim();

                // Add the element name to the HashSet (only unique values will be kept)
                uniqueElements.Add(elementName);
            }
        }

        // Return the count of unique elements
        return uniqueElements.Count;
    }


}
