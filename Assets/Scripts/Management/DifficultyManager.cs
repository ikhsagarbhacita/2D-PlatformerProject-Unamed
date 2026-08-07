using UnityEngine;

//
public enum DifficultyType { Easy = 1, Normal, Hard}

public class DifficultyManager : MonoBehaviour
{
    // 
    public static DifficultyManager Instance;

    public DifficultyType difficulty;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // Prevents the SkinManager from being destroyed when loading a new scene

        // Singleton pattern implementation to ensure only one instance of SkinManager exists
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 
    public void SetDifficulty(DifficultyType newDifficulty) => difficulty = newDifficulty;

    //
    public void LoadDifficulty(int difficultyIndex)
    {
        difficulty = (DifficultyType)difficultyIndex; // 
    } 
}