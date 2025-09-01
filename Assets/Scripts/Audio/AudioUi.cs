using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;

    void Start()
    {
        masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSfxVolume);
        muteToggle.onValueChanged.AddListener(AudioManager.Instance.ToggleMute);

        // UI 초기값 세팅 (AudioManager에서 가져온 값으로 하는 것임)
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        muteToggle.isOn = false;
    }
}
