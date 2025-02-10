using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class SliderData
{
    public string attributeName;
    public float value;
}

[System.Serializable]
public class CharacterSaveSlot
{
    public List<SliderData> sliders = new List<SliderData>();
}

public class CharacterSaveManager : MonoBehaviour
{
    private string saveDirectory;

    void Start()
    {
        // Ensure saveDirectory string exists
        if (saveDirectory == null)
        {
            saveDirectory = Application.persistentDataPath + "/CharacterSaves/";
        }

        // Ensure actual save directory exists
        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(saveDirectory);
    }

    // Function to save the current character under a written name
    public void SaveCharacter(string slotName)
    {
        if (string.IsNullOrEmpty(slotName))
        {
            Debug.LogWarning("Invalid save slot name.");
            return;
        }

        string savePath = Path.Combine(saveDirectory, slotName + ".json");
        CharacterSaveSlot saveData = new CharacterSaveSlot();
        UI_AttributeSlider[] sliders = FindObjectsOfType<UI_AttributeSlider>();

        foreach (UI_AttributeSlider slider in sliders)
        {
            SliderData sliderData = new SliderData
            {
                attributeName = slider.name,
                value = slider.GetSliderValue()
            };
            saveData.sliders.Add(sliderData);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Character saved to: " + savePath);
    }

    // Function that loads the currently selected save slot
    // shown on the dropdown
    public void LoadCharacter(string slotName)
    {
        string savePath = Path.Combine(saveDirectory, slotName + ".json");

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            CharacterSaveSlot saveData = JsonUtility.FromJson<CharacterSaveSlot>(json);
            UI_AttributeSlider[] sliders = FindObjectsOfType<UI_AttributeSlider>();

            foreach (SliderData sliderData in saveData.sliders)
            {
                foreach (UI_AttributeSlider slider in sliders)
                {
                    if (slider.name == sliderData.attributeName)
                    {
                        slider.SetSliderValue(sliderData.value);
                        break;
                    }
                }
            }

            Debug.Log("Character loaded from: " + savePath);
        }
        else
        {
            Debug.LogWarning("Save slot not found: " + slotName);
        }
    }

    // Function to retrieve all named character save slots.
    // Currently used to populate a dropdown menu.
    // This runs before CharacterSaveManager.Start()!
    public List<string> GetSaveSlots()
    {
        if (saveDirectory == null)
        {
            saveDirectory = Application.persistentDataPath + "/CharacterSaves/";
        }

        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        List<string> saveSlots = new List<string>();

        string[] files = Directory.GetFiles(saveDirectory, "*.json");
        foreach (string file in files)
        {
            saveSlots.Add(Path.GetFileNameWithoutExtension(file));
        }

        return saveSlots;
    }
}
