using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    public int[] skinID;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // Prevents the SkinManager from being destroyed when loading a new scene

        // Singleton pattern implementation to ensure only one instance of SkinManager exists
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetSkinId(int id, int playerNumber) => skinID[playerNumber] = id;
    public int GetSkinId(int playerNumber) => skinID[playerNumber];
}
