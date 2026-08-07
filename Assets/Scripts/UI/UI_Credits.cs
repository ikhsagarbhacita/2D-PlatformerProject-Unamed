using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Credits : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    [SerializeField] private RectTransform rectT;
    [SerializeField] private float scrollSpeed = 200f;
    [SerializeField] private float offScreenPositionY = 1600f;

    [SerializeField] private string mainMenuSceneName = "MainMenu";
    private bool creditsSkipped;

    private void Awake()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
        fadeEffect.ScreenFade(0f, 2f); // Fade in from black when the credits scene is loaded.
    }

    private void Update()
    {
        rectT.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Check if the credits have scrolled off the screen, and if so, transition to the Main Menu Scene.
        if (rectT.anchoredPosition.y > offScreenPositionY)
            GoToMainMenu();
    }

    // This method is called when the player chooses to skip the credits.
    public void SkipCredits() 
    {
        if (creditsSkipped == false)
        {
            scrollSpeed *= 10;
            creditsSkipped = true;
        }
        else
        {
            GoToMainMenu();
        }
    }

    // This method is called to initiate the transition to the main menu scene with a fade effect.
    private void GoToMainMenu() => fadeEffect.ScreenFade(1f, 1f, SwitchToMenuScene);

    // This method is called to load the main menu scene after the fade effect is complete.
    private void SwitchToMenuScene()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
