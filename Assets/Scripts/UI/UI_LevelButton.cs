using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNumberText;

    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI fruitsText;

    private int levelIndex;
    public string sceneName;

    // This method is called to set up the button with the appropriate level index and scene name.
    public void SetUpButton(int newLevelIndex)
    {
        levelIndex = newLevelIndex;

        levelNumberText.text = "Level " + levelIndex; // Set the button text to display the level number
        sceneName = "Level_" + levelIndex; // Assuming the scene names follow the format "Level_1", "Level_2", etc.

        bestTimeText.text = TimerInfoText();
        fruitsText.text = FruitsInfoText();
    }

    // This method is called when the button is clicked to load the corresponding level.
    public void LoadLevel()
    {
        AudioManager.instance.PlaySFX(4);

        int difficultyIndex = ((int)DifficultyManager.Instance.difficulty); // 
        PlayerPrefs.SetInt("GameDifficulty", difficultyIndex); // 
        SceneManager.LoadScene(sceneName);
    }

    private string FruitsInfoText() 
    { 
        int totalFruits = PlayerPrefs.GetInt("Level" + levelIndex + "TotalFruits", 0); // Save the total number of fruits for the current level in PlayerPrefs
        string totalFruitsText = totalFruits == 0 ? "?" : totalFruits.ToString();

        int fruitsCollected = PlayerPrefs.GetInt("Level" + levelIndex + "FruitsCollected"); // 
        return "Fruits: " + fruitsCollected + " / " + totalFruitsText; // 
    }

    private string TimerInfoText()
    {
        float timerValue = PlayerPrefs.GetFloat("Level" + levelIndex + "BestTime", 99); // 
        return "Best Time: " + timerValue.ToString("00"); // 
    }
}
