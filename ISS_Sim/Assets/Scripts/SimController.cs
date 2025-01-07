using UnityEngine;

public class SimController : MonoBehaviour
{
    public SimPreferences preferences; // Reference the ScriptableObject

    private void Start()
    {
        // Load saved preferences or default values
        preferences.LoadPreferences("SimPreferences");

        // Apply preferences
        Debug.Log("Simulation Name: " + preferences.simulationName);
        Debug.Log("Difficulty Level: " + preferences.difficultyLevel);
        Debug.Log("Simulation Speed: " + preferences.simulationSpeed);
        Debug.Log("Sound Enabled: " + preferences.enableSound);

        // Example: Update simulation speed
        Time.timeScale = preferences.simulationSpeed;
    }

    private void OnApplicationQuit()
    {
        // Save preferences when exiting
        preferences.SavePreferences("SimPreferences");
    }
}
