using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class UI_TextBlinkFX : MonoBehaviour
{
    [SerializeField] private float cycleDuration;
    [SerializeField] private TextMeshProUGUI text;

    private Color transperentColor = new Color(1, 1, 1, 0.2f);
    private System.IDisposable inputEventListener;// Adds

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        StartCoroutine(BlinkCoroutine());
    }

    // Adds
    private void OnEnable()
    {
        // Mulai coroutine kedip setiap kali GameObject ini aktif
        StartCoroutine(BlinkCoroutine());

        // Dengarkan input tombol dari device APAPUN (Keyboard, Gamepad, Controller)
        inputEventListener = InputSystem.onAnyButtonPress.Call(control => DisableText());
    }

    private void DisableText()
    {
        gameObject.SetActive(false);
    }

    // 
    private IEnumerator BlinkCoroutine()
    {
        float halfCycle = cycleDuration / 2;

        while (true)
        {
            ToggleColor(Color.white);
            yield return new WaitForSeconds(halfCycle);

            ToggleColor(transperentColor);
            yield return new WaitForSeconds(halfCycle);
        }
    }

    // 
    private void ToggleColor(Color color)
    {
        text.color = color; 
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        // Adds
        if (inputEventListener != null)
        {
            inputEventListener.Dispose();
        }
    }
}
