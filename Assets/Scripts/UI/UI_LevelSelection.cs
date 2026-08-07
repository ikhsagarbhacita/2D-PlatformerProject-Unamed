using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_LevelSelection : MonoBehaviour
{
    private UI_MainMenu mainMenuUI;
    [SerializeField] private GameObject firstSelected;

    [SerializeField] private UI_LevelButton buttonPrefab;
    [SerializeField] private Transform buttonParents;

    [SerializeField] bool[] levelsUnlocked;

    private void Awake()
    {
        mainMenuUI = GetComponentInParent<UI_MainMenu>();

        LoadLevelsInfo();
        CreateLevelButtons();
    }

    private void OnEnable()
    {
        mainMenuUI.UpdateLastSelected(firstSelected);

        GameObject firstLevelButton = buttonParents.GetChild(0).gameObject;

        if (firstLevelButton != null)
            EventSystem.current.SetSelectedGameObject(firstLevelButton);
        else
            EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    // This method is called when the script instance is being loaded. It initializes the level selection UI by creating buttons for each level in the game.
    private void CreateLevelButtons() 
    {
        int levelsCount = SceneManager.sceneCountInBuildSettings - 1; // Get the total number of levels in the build settings, excluding the 'The End' scene

        // Loop through the levels and create buttons for each level
        for (int i = 1; i < levelsCount; i++) // Start from 1 to skip the 'Main Menu' scene (cuz it index 0)
        {
            if (IsLevelUnlocked(i) == false) // Check if the level is unlocked
                return;

            UI_LevelButton newButton = Instantiate(buttonPrefab, buttonParents);
            newButton.SetUpButton(i); // Set up the button with the level index
        }
    }

    private bool IsLevelUnlocked(int levelIndex) => levelsUnlocked[levelIndex];

    private void LoadLevelsInfo()
    {
        int levelsCount = SceneManager.sceneCountInBuildSettings - 1; // Get the total number of levels in the build settings, excluding the 'The End' scene

        levelsUnlocked = new bool[levelsCount]; // Initialize the levelsUnlocked array with the correct size

        for (int i = 1; i < levelsCount; i++) // Start from 1 to skip the 'Main Menu' scene (cuz it index 0)
        {
            bool levelUnlocked = PlayerPrefs.GetInt("Level" + i + "Unlocked", 0) == 1; // Check if the level is unlocked in PlayerPrefs

            if (levelUnlocked)
                levelsUnlocked[i] = true;
        }

        levelsUnlocked[1] = true; // Ensure that the first level is always unlocked
    }
}
