using UnityEngine;

[CreateAssetMenu(fileName = "SimPreferences", menuName = "Simulation/Preferences")]
public class SimPreferences : ScriptableObject
{
    // Simulation preferences
    public string simulationName;
    public int difficultyLevel;
    public float simulationSpeed;
    public bool enableSound;

    // Save/Load methods
    public void SavePreferences(string fileName)
    {
        string json = JsonUtility.ToJson(this, true); // Convert to JSON
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".json", json);
    }

    public void LoadPreferences(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName + ".json";

        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath); // Read JSON
            JsonUtility.FromJsonOverwrite(json, this); // Load into object
        }
        else
        {
            Debug.LogWarning("Save file not found: " + filePath);
        }
    }
}
