using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource bgmSource;
    public AudioSource[] sfxSources;  // 다양한 효과음 소스 배열

    private float masterVolume = 1f;
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환해도 유지

        // PlayerPrefs에서 이전 설정 불러오기
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        AudioListener.volume = masterVolume;

        if (bgmSource != null)
            bgmSource.volume = bgmVolume;

        foreach (AudioSource sfx in sfxSources)
        {
            if (sfx != null)
                sfx.volume = sfxVolume;
        }
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetBgmVolume(float value)
    {
        bgmVolume = value;
        if (bgmSource != null)
            bgmSource.volume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = value;
        foreach (AudioSource sfx in sfxSources)
        {
            if (sfx != null)
                sfx.volume = value;
        }
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void ToggleMute(bool isMuted)
    {
        AudioListener.volume = isMuted ? 0f : masterVolume;
    }

    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxSources.Length)
        {
            sfxSources[index].Play();
        }
    }
}

