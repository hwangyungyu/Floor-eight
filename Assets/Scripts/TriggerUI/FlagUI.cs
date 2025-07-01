using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagUI : MonoBehaviour
{
    public Text flagTextUI;

    public GameObject textPrefab; // 텍스트 프리팹
    public Transform contentParent; // 텍스트들이 들어갈 부모 오브젝트

    public void UpdateFlagText()
    {
        if (FlagManager.Instance == null)
            return;

        ClearChildren(); // 기존 텍스트 제거

        var flags = GetActiveFlags();
        if (flags.Count == 0)
            return;

        foreach (var flag in flags)
        {
            GameObject obj = Instantiate(textPrefab, contentParent);
            Text text = obj.GetComponent<Text>();
            if (text != null)
                text.text = flag;
        }
    }

    private void ClearChildren()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    // FlagManager에서 활성화된 플래그 이름만 추출
    private List<string> GetActiveFlags()
    {
        var activeFlags = new List<string>();
        Dictionary<string, bool> allFlags = FlagManager.Instance.GetAllFlags();
        foreach (var pair in allFlags)
        {
            if (pair.Value)
                activeFlags.Add(pair.Key);
        }

        return activeFlags;
    }
}
