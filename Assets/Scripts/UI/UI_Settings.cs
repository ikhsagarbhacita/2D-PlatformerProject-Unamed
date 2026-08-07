using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25f;

    [Header("SFX Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxSliderText;
    [SerializeField] private string sfxParam;

    [Header("BGM Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private TextMeshProUGUI bgmSliderText;
    [SerializeField] private string bgmParam;

    // 
    public void SFXSliderValue(float value)
    {
        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(sfxParam, newValue);
    }

    // 
    public void BgmSliderValue(float value)
    {
        bgmSliderText.text = Mathf.RoundToInt(value * 100) + "%";
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(bgmParam, newValue);
    }

    // 
    private void OnDisable()
    {
        PlayerPrefs.SetFloat(sfxParam, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParam, bgmSlider.value);
    }

    // 
    private void OnEnable()
    {
        GetComponentInParent<UI_MainMenu>().UpdateLastSelected(firstSelected);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        sfxSlider.value = PlayerPrefs.GetFloat(sfxParam, 0.75f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParam, 0.75f);
    }
}
