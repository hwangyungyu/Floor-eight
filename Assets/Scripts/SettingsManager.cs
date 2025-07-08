using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;

    private Resolution[] allResolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();
    private float targetAspect;

    void Start()
    {
        // 현재 화면 비율 계산하기
        targetAspect = (float)Screen.currentResolution.width / Screen.currentResolution.height;

        // 전체 해상도 리스트
        allResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            Resolution res = allResolutions[i];
            float resAspect = (float)res.width / res.height;

            // 비율 차이가 너무 크면 제외
            if (Mathf.Abs(resAspect - targetAspect) > 0.01f)
                continue;

            filteredResolutions.Add(res);
            string option = res.width + " x " + res.height;
            options.Add(option);

            if (res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                currentResIndex = filteredResolutions.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();

        // Toggle 초기값 설정
        fullscreenToggle.isOn = Screen.fullScreen;

        // 리스너 등록
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    void SetResolution(int index)
    {
        Resolution res = filteredResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }
}

