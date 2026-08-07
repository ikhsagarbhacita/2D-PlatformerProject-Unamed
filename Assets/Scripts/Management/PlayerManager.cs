using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static event Action OnPlayerRespawn;
    public static event Action OnPlayerDeath;

    public PlayerInputManager playerInputManager {  get; private set; }
    public static PlayerManager instance;

    public List<GameObject> objectsToDisable;

    public int lifePoints;
    public int maxPlayerCount = 1;
    public int playerCountWinCondition;
    [Header("Player")]
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private string[] playerDevice;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        // Singleton pattern implementation to ensure only one instance of GameManager exists
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerLeft += RemovePlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerLeft -= RemovePlayer;
    }

    // 
    public void SetUpMaxPlayersCount(int newPlayersCount)
    {
        maxPlayerCount = newPlayersCount;
        playerInputManager.SetMaxPlayerCount(maxPlayerCount);   
    }


    // 
    public void EnableJoinAndUpdateLifePoints()
    {
        playerInputManager.EnableJoining();
        playerCountWinCondition = maxPlayerCount;
        lifePoints = maxPlayerCount;
        UI_InGame.instance.UpdateLifePointsUI(lifePoints, maxPlayerCount);
    }

    // 
    private void AddPlayer(PlayerInput newPlayer)
    {
        Player playerScript = newPlayer.GetComponent<Player>();
        playerList.Add(playerScript);

        OnPlayerRespawn?.Invoke();
        RespawnNewPlayer(newPlayer.transform);

        // Debug* (check kinda dewvices)
        //foreach (var device in player.devices)
        //{
        //    Debug.Log(device.name);
        //}

        int newPlayerNumber = GetPlayerNumber(newPlayer);
        int newPlayerSkinId = SkinManager.Instance.GetSkinId(newPlayerNumber);

        playerScript.UpdateSkin(newPlayerSkinId);

        foreach (GameObject gameObject in objectsToDisable)
        {
            gameObject.SetActive(false);
        }
    }

    // 
    private void RemovePlayer(PlayerInput player)
    {
        Player playerScript = player.GetComponent<Player>();
        playerList.Remove(playerScript);

        if (CanRemoveLifePoints() && lifePoints > 0)
            lifePoints--;

        if (lifePoints <= 0)
        {
            playerCountWinCondition--;
            playerInputManager.DisableJoining();

            if (playerList.Count <= 0)
                GameManager.Instance.RestartLevel();
        }

        UI_InGame.instance.UpdateLifePointsUI(lifePoints, maxPlayerCount);
        OnPlayerDeath?.Invoke();
    }

    // 
    private bool CanRemoveLifePoints()
    {
        if (DifficultyManager.Instance.difficulty == DifficultyType.Hard)
        {
            return true;
        }

        if (GameManager.Instance.fruitsCollected <= 0 && DifficultyManager.Instance.difficulty == DifficultyType.Normal)
        {
            return true;
        }

        return false;
    }

    // 
    private int GetPlayerNumber(PlayerInput newPlayer)
    {
        int newPlayerNumber = 0;

        foreach (var device in newPlayer.devices)
        {
            for (var i = 0; i < playerDevice.Length; i++)
            {
                //Debug.Log("Player " + (i + 1) + " has Joined the Game!");
                newPlayerNumber = i;
                if (playerDevice[i] == "Empty")
                {
                    playerDevice[i] = device.name;
                    break;
                }
                else if (playerDevice[i] == device.name)
                {
                    //Debug.Log("Player " + (i + 1) + " has Re-Joined the Game!");
                    newPlayerNumber = i;
                    break;
                }
            }
        }

        return newPlayerNumber;
    }

    // 
    public List<Player> GetPlayerList() => playerList;

    // Updates the player respawn position
    public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;

    // 
    private void RespawnNewPlayer(Transform newPlayer)
    {
        if (respawnPoint == null)
            respawnPoint = FindAnyObjectByType<StartPoint>().transform;

        newPlayer.position = respawnPoint.position;
    }
}
