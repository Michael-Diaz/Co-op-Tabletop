using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class SliderData
{
    public string attributeName;
    public float value;
}

[System.Serializable]
public class BoneTransformData
{
    public string boneName;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 localScale;  // Store the actual local scale
    public Vector3 worldScale;  // Store lossyScale (for reference, but not to be applied directly)
}

[System.Serializable]
public class CharacterSaveSlot
{
    public string characterPrefabName; // Reference to the character prefab
    public List<BoneTransformData> bones = new List<BoneTransformData>();
}

public class CharacterSaveManager : MonoBehaviour
{
    private string saveDirectory;
    private static CharacterSaveManager instance;
    public GameObject characterPrefab;

    private void Awake()
    {
        // Init the singleton instance
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

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
    public void SaveCharacter(string slotName, GameObject character)
    {
        if (string.IsNullOrEmpty(slotName))
        {
            Debug.LogWarning("Invalid save slot name.");
            return;
        }

        string savePath = Path.Combine(saveDirectory, slotName + ".json");
        CharacterSaveSlot saveData = new CharacterSaveSlot();
        saveData.characterPrefabName = characterPrefab.name; //"Assets/Prefabs/Alex/Default";//character.name; // Store prefab name

        // Save bone transform data
        SkinnedMeshRenderer skinnedMeshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            foreach (Transform bone in skinnedMeshRenderer.bones)
            {
                saveData.bones.Add(new BoneTransformData
                {
                    boneName = bone.name,
                    position = bone.localPosition,
                    rotation = bone.localEulerAngles,
                    localScale = bone.localScale, // Store actual local scale
                    worldScale = bone.lossyScale  // Store lossyScale for debugging/reference
                });
            }
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Character saved to: " + savePath);
    }

    // Function that loads the currently selected save slot
    // shown on the dropdown
    public GameObject LoadCharacter(string slotName, Transform spawnPoint)
    {
        string savePath = Path.Combine(saveDirectory, slotName + ".json");

        if (!File.Exists(savePath)) 
        {
            Debug.LogWarning("Save slot not found: " + slotName);
            return null;
        }

        string json = File.ReadAllText(savePath);
        CharacterSaveSlot saveData = JsonUtility.FromJson<CharacterSaveSlot>(json);

        GameObject character = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
        if (character == null)
        {
            Debug.LogError("Character failed to spawn!");
        }

        // Load bone transforms
        SkinnedMeshRenderer skinnedMeshRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            foreach (BoneTransformData boneData in saveData.bones)
            {
                foreach (Transform bone in skinnedMeshRenderer.bones)
                {
                    if (bone.name == boneData.boneName)
                    {
                        // Restore local position and rotation
                        bone.localPosition = boneData.position;
                        bone.localEulerAngles = boneData.rotation;

                        // Simply reapply the saved local scale data to our freshly spawned bone
                        bone.localScale = boneData.localScale;

                        break;
                    }
                }
            }
        }

        Debug.Log("Character loaded from: " + savePath);
        return character;
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