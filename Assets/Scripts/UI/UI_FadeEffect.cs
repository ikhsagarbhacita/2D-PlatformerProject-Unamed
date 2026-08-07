using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeEffect : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    public void ScreenFade(float targetAlpha, float duration, System.Action onComplete = null)
    {
        //gameObject.SetActive(true); //  Adds -> Ensure the fade effect GameObject is active before starting the fade
        StartCoroutine(FadeCoroutine(targetAlpha, duration, onComplete));
    }

    private IEnumerator FadeCoroutine(float targetAlpha, float duration, System.Action onComplete)
    {
        float time = 0f;
        Color currentColor  = fadeImage.color;

        float startAlpha = currentColor.a; // Store the initial alpha value (opacity) of the image

        // Interpolate the alpha value over time using Mathf.Lerp
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration); // Calculate the new alpha value based on the elapsed time and duration

            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha); // Update the image color with the new alpha value
            yield return null; // Wait for the next frame
        }

        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha); // Ensure the final alpha value is set

        onComplete?.Invoke(); // Invoke the onComplete callback if it's not null after the fade effect is complete
    }
}
