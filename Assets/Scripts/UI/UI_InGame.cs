using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private PlayerInputSet playerInput;
    private List<Player> playerList;
    public static UI_InGame instance;
    public UI_FadeEffect fadeEffect { get; private set; } // read-only property for fadeEffect

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI fruitText;
    [SerializeField] private TextMeshProUGUI lifePointsText;

    [SerializeField] private GameObject pauseUI;
    private bool isPaused;

    private void Awake()
    {
        instance = this;

        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
        // Debug*
        //if (fadeEffect == null)
        //    fadeEffect = GetComponentInChildren<UI_FadeEffect>(true);

        playerInput = new PlayerInputSet();
    }

    // 
    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.UI.Pause.performed += ctx => PauseButton();
        playerInput.UI.Navigate.performed += ctx => UpdateSelected();
    }

    // 
    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.UI.Pause.performed -= ctx => PauseButton();
        playerInput.UI.Navigate.performed -= ctx => UpdateSelected();
    }

    private void Start()
    {
        fadeEffect.ScreenFade(0f, 1f); // Fade in from black when the in-game UI is loaded.
        GameObject pressJoinText = FindAnyObjectByType<UI_TextBlinkFX>().gameObject;
        PlayerManager.instance.objectsToDisable.Add(pressJoinText);

        // Debug*
        //if (fadeEffect != null)
        //{
        //    fadeEffect.ScreenFade(0f, 1f); // Fade in from black when the in-game UI is loaded
        //}
        //else
        //{
        //    Debug.LogError("UI_FadeEffect couldnt found UI_InGame!");
        //}
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        //{
        //    PauseButton();
        //}
    }

    // 
    private void UpdateSelected()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    // 
    public void PauseButton()
    {
        playerList = PlayerManager.instance.GetPlayerList();

        if (isPaused)
            UnpauseGame();
        else
            PauseGame();

    }

    // 
    private void PauseGame()
    {
        foreach (var player in playerList)
        {
            player.playerInput.Disable();
        }

        EventSystem.current.SetSelectedGameObject(firstSelected);
        isPaused = true;
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
    }

    // 
    private void UnpauseGame()
    {
        foreach (var player in playerList)
        {
            player.playerInput.Enable();
        }

        isPaused = false;
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
    }

    // 
    public void GoToMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    // This method updates the fruit UI text to show the number of collected fruits out of the total fruits
    public void UpdateFruitUI(int collectedFruits, int totalFruits)
    {
        fruitText.text = collectedFruits + "/" + totalFruits; // Update the fruit UI text to show the number of collected fruits out of the total fruits
    }

    // This method updates the timer UI text to show the remaining time in seconds
    public void UpdateTimerUI(float timer)
    {
        timerText.text = timer.ToString("00" + "s"); // "00" formats the timer to always show two digits, adding a leading zero if necessary.
    }

    // 
    public void UpdateLifePointsUI(int lifePoints, int maxLlifePoints)
    {
        if (DifficultyManager.Instance.difficulty == DifficultyType.Easy)
        {
            lifePointsText.transform.parent.gameObject.SetActive(false);
            return;
        }

        lifePointsText.text = lifePoints + "/" + maxLlifePoints;
    }
}
