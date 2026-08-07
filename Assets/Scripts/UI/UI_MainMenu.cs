using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject lastSelected;
    private DefaultInputActions defaultInput;
    private UI_FadeEffect fadeEffect;
    public string FirstLevelName;

    [SerializeField] private GameObject[] uiElements;
    [SerializeField] private GameObject continueButton;

    [Header("Interactive Camera")]
    [SerializeField] private MenuCharacter menuCharacter;
    [SerializeField] private CinemachineCamera cinemachine;
    [SerializeField] private Transform mainMenuPoint;
    [SerializeField] private Transform skinSelectionPoint;

    private void Awake()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();

        defaultInput = new DefaultInputActions();
    }

    private void Start()
    {
        //if (HasLevelProgression())
        //    continueButton.SetActive(true); // Show the continue button if there is level progression.

        fadeEffect.ScreenFade(0f, 1.5f); // Fade in from black when the main menu is loaded.
    }

    // 
    private void OnEnable()
    {
        defaultInput.Enable();
        defaultInput.UI.Navigate.performed += ctx => UpdateSelected();
    }

    // 
    private void OnDisable()
    {
        defaultInput.Disable();
        defaultInput.UI.Navigate.performed -= ctx => UpdateSelected();
    }

    // 
    public void UpdateLastSelected(GameObject newLastSelected)
    {
        lastSelected = newLastSelected;
    }

    // 
    private void UpdateSelected()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastSelected);
    }


    // 
    public void SwitchUI(GameObject uiToEnable)
    {
        foreach (GameObject ui in uiElements)
        {
            ui.SetActive(false); // Disable all UI elements in the array.
        }

        uiToEnable.SetActive(true); // Enable the specified UI element.

        AudioManager.instance.PlaySFX(4); // 
    }

    public void NewGame()
    {
        fadeEffect.ScreenFade(1f, 1.5f, LoadLevelScene); // Fade to black and then load the level scene after the fade effect is complete.
        AudioManager.instance.PlaySFX(4);
    }

    // This method is called after the fade effect is complete to load the specified level scene.
    private void LoadLevelScene() => SceneManager.LoadScene(FirstLevelName);

    private bool HasLevelProgression()
    {
        bool hasLevelProgression = PlayerPrefs.GetInt("ContinueLevelNumber", 0) > 0;
        return hasLevelProgression;
    }

    public void ContinueGame() 
    {
        int difficultyIndex = PlayerPrefs.GetInt("GameDifficulty", 1); // 
        int levelToLoad = PlayerPrefs.GetInt("ContinueLevelNumber", 0); // Get the level index to load from PlayerPrefs (default to 0 if not found)
        int lastSavedSkin = PlayerPrefs.GetInt("LastUsedSkin"); // 

        //SkinManager.Instance.SetSkinId(lastSavedSkin); // 

        DifficultyManager.Instance.LoadDifficulty(difficultyIndex); // 
        SceneManager.LoadScene("Level_" + levelToLoad); // Load the level scene based on the saved level index in PlayerPrefs
        AudioManager.instance.PlaySFX(4);
    }

    // 
    public void MoveCameraToMainMenu()
    {
        menuCharacter.MoveTo(mainMenuPoint);
        cinemachine.Follow = mainMenuPoint;
    }

    // 
    public void MoveCameraToSkinMenu()
    {
        menuCharacter.MoveTo(skinSelectionPoint);
        cinemachine.Follow = skinSelectionPoint;
    }

    //
    public void QuitButton()
    {
        //if (EditorApplication.isPlaying)
        //    EditorApplication.isPlaying = false;
        //else
            Application.Quit();
    }
}
