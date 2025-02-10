using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SaveSlotUI : MonoBehaviour
{
    public CharacterSaveManager saveManager;
    public TMPro.TMP_Dropdown saveSlotDropdown;
    public TMPro.TMP_InputField saveSlotInputField;

    void Start()
    {
        UpdateSaveSlots();
    }

    public void UpdateSaveSlots()
    {
        saveSlotDropdown.ClearOptions();
        List<string> saveSlots = saveManager.GetSaveSlots();
        saveSlotDropdown.AddOptions(saveSlots);
    }

    public void SaveCurrentCharacter()
    {
        saveManager.SaveCharacter(saveSlotInputField.text);
        UpdateSaveSlots();
    }

    public void LoadSelectedCharacter()
    {
        if (saveSlotDropdown.options.Count > 0)
        {
            saveManager.LoadCharacter(saveSlotDropdown.options[saveSlotDropdown.value].text);
            saveSlotInputField.text = saveSlotDropdown.options[saveSlotDropdown.value].text;
        }
    }
}
