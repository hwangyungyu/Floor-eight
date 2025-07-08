using UnityEngine;

public class SettingsUIController : MonoBehaviour
{
    public GameObject settingsPanel;       // 전체 설정 UI
    public GameObject audioSettingsPanel;  // 오디오 설정 UI
    public GameObject graphicSettingsPanel;// 그래픽 설정 UI

    // 뒤로가기 버튼 눌렀을 때
    public void OnSettingButtonClicked()
    {
        settingsPanel.SetActive(true);
    }

    // 뒤로가기 버튼 눌렀을 때
    public void OffSettingButtonClicked()
    {
        settingsPanel.SetActive(false);
    }

    // 오디오 설정 진입
    public void OnAudioSettingsButtonClicked()
    {
        audioSettingsPanel.SetActive(true);
        graphicSettingsPanel.SetActive(false);
    }

    // 오디오 설정 진입
    public void OffAudioSettingsButtonClicked()
    {
        audioSettingsPanel.SetActive(false);
    }

    // 그래픽 설정 진입
    public void OnGraphicSettingsButtonClicked()
    {
        graphicSettingsPanel.SetActive(true);
        audioSettingsPanel.SetActive(false);
    }

    // 그래픽 설정 진입
    public void OffGraphicSettingsButtonClicked()
    {
        graphicSettingsPanel.SetActive(false);
    }
}