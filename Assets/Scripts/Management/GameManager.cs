using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private UI_InGame inGameUI;

    [Header("Level Management")]
    [SerializeField] private float levelTimer;
    [SerializeField] private int currentLevelIndex;
    private int nextLevelIndex;

    [Header("Fruits Management")]
    public bool fruitRandomize;
    public int fruitsCollected;
    public int totalFruits;
    public Transform fruitParent;
    private List<FruitType> collectedFruitHistory = new List<FruitType>(); // Adds

    [Header("Checkpoints")]
    public bool canReactivate;

    [Header("Managers")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private SkinManager skinManager;
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private ObjectCreator objectCreator;

    private void Awake()
    {
        // Singleton pattern implementation to ensure only one instance of GameManager exists
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }

    private void Start()
    {
        inGameUI = UI_InGame.instance; // Cache the reference to the in-game UI instance
        
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex; // Store the index of the current level

        nextLevelIndex = currentLevelIndex + 1; // Calculate the index of the next level
        CollectFruitsInfo();
        CreateManagersOn();

        PlayerManager.instance.EnableJoinAndUpdateLifePoints();
    }

    private void Update()
    {
        levelTimer += Time.deltaTime;

        inGameUI.UpdateTimerUI(levelTimer); // Update the in-game UI with the current level timer
    }

    // Makes them when needed
    private void CreateManagersOn()
    {
        if (AudioManager.instance == null)
            Instantiate(audioManager);

        if (PlayerManager.instance == null)
            Instantiate(playerManager);

        if (SkinManager.Instance == null)
            Instantiate(skinManager);

        if (DifficultyManager.Instance == null)
            Instantiate(difficultyManager);

        if (ObjectCreator.Instance == null)
            Instantiate(objectCreator);
    }

    // Counts all fruit objects present in the current scene
    private void CollectFruitsInfo()
    {
        Fruit[] fruits = FindObjectsByType<Fruit>(); 
        totalFruits = fruits.Length;

        inGameUI.UpdateFruitUI(fruitsCollected, totalFruits);

        PlayerPrefs.SetInt("Level" + currentLevelIndex + "TotalFruits", totalFruits); // Save the total number of fruits for the current level in PlayerPrefs
    }

    // 
    [ContextMenu("Parent All Fruits")]
    private void FruitParentz()
    {
        if (fruitParent == null)
            return;

        Fruit[] fruits = FindObjectsByType<Fruit>();

        foreach (Fruit fruit in fruits)
        {
            fruit.transform.parent = fruitParent;
        }
    }


    // 
    public void CollectFruit(FruitType fruitType) 
    {
        fruitsCollected++;
        collectedFruitHistory.Add(fruitType); // Adds
        inGameUI.UpdateFruitUI(fruitsCollected, totalFruits);
    }

    // 
    public FruitType RemoveFruit()
    {
        fruitsCollected--;
        inGameUI.UpdateFruitUI(fruitsCollected, totalFruits);

        // Adds
        if (collectedFruitHistory.Count > 0)
        {
            int lastIndex = collectedFruitHistory.Count - 1;
            FruitType lastFruit = collectedFruitHistory[lastIndex];
            collectedFruitHistory.RemoveAt(lastIndex); // Remove last Fruit from Index
            return lastFruit;
        }

        return FruitType.Apple; 
    }

    // 
    public int FruitsCollected() => fruitsCollected;
    
    // 
    public bool FruitRandomize() => fruitRandomize;

    // 
    public void LevelFinished()
    {
        SaveLevelProgression();
        SaveBestTime();
        SaveFruitsInfo();

        LoadNextScene(); // Initiates the process to load the next scene when the level is finished
    }

    // 
    private void SaveFruitsInfo()
    {
        int fruitsCollectedBefore  = PlayerPrefs.GetInt("Level" + currentLevelIndex + "FruitsCollected"); // Retrieves the previously saved number of collected fruits for the current level from PlayerPrefs

        if (fruitsCollectedBefore < fruitsCollected)
            PlayerPrefs.SetInt("Level" + currentLevelIndex + "FruitsCollected", fruitsCollected); // Saves the number of collected fruits for the current level in PlayerPrefs

        int totalTotalFruitsStored = PlayerPrefs.GetInt("TotalFruitsAmount"); // 
        PlayerPrefs.SetInt("TotalFruitsAmount", totalTotalFruitsStored + fruitsCollected); // 
    }

    //
    private void SaveBestTime()
    {
        float lastTime = PlayerPrefs.GetFloat("Level" + currentLevelIndex + "BestTime", 99); // 

        if (levelTimer < lastTime)
            PlayerPrefs.SetFloat("Level" + currentLevelIndex + "BestTime", levelTimer); // Saves the best time for the current level in PlayerPrefs
    }

    // 
    private void SaveLevelProgression()
    {
        PlayerPrefs.SetInt("Level" + nextLevelIndex + "Unlocked", 1); // Unlocks the next level in PlayerPrefs)

        if (NoMoreLevels() == false)
        {
            PlayerPrefs.SetInt("ContinueLevelNumber", nextLevelIndex); // Saves the next level index for continuation purposes

            SkinManager skinManager = SkinManager.Instance;

            if (skinManager != null)
                PlayerPrefs.SetInt("LastUsedSkin", SkinManager.Instance.GetSkinId(0));
        }
    }

    //
    public void RestartLevel()
    {
        UI_InGame.instance.fadeEffect.ScreenFade(1f, 0.75f, LoadCurrentScene);
    }

    // 
    private void LoadCurrentScene() => SceneManager.LoadScene("Level_" + currentLevelIndex);

    // Loads the "TheEnd" scene, which is assumed to be the final scene of the game
    private void LoadTheEndScene() => SceneManager.LoadScene("TheEnd");

    // Loads the next level based on the current level index
    private void LoadNextLevel()
    {
        SceneManager.LoadScene("Level_" + nextLevelIndex); // Load the next level based on the current level index)
    }

    private void LoadNextScene()
    {
        // Debugging*
        //if (UI_InGame.instance == null || UI_InGame.instance.fadeEffect == null)
        //{
        //    Debug.LogError("UI_InGame or UI_FadeEffect is null!");
        //    return;
        //}

        UI_FadeEffect fadeEffect = UI_InGame.instance.fadeEffect;

        if (NoMoreLevels())
            fadeEffect.ScreenFade(1f, 1.5f, LoadTheEndScene); // If there are no more levels, fade out and load the "TheEnd" scene
        else
            fadeEffect.ScreenFade(1f, 1.5f, LoadNextLevel); // If there are more levels, fade out and load the next level
    }

    // 
    private bool NoMoreLevels()
    {
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 2; // Calculate the index of the build settings' last level (excluding the "TheEnd" scene)
        bool noMoreLevels = currentLevelIndex == lastLevelIndex; // Check if the current level is the last level

        return noMoreLevels;
    }
}
