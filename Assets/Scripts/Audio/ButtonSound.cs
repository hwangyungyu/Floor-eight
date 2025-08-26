using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [Tooltip("AudioManager.sfxSources에서 재생할 인덱스")]
    public int sfxIndex = 0;

    private Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(Play);
    }

    void OnDestroy()
    {
        if (btn != null) btn.onClick.RemoveListener(Play);
    }

    private void Play()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager.Instance 를 찾을 수 없습니다.");
            return;
        }

        AudioManager.Instance.PlaySFX(sfxIndex);
    }
}
