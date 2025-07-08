using System.Collections.Generic;
using UnityEngine;

public class EventCardTextLoader
{
    private Dictionary<string, EventCardTextData> textDataMap;

    public EventCardTextLoader()
    {
        LoadJson();
    }

    private void LoadJson()
    {
        TextAsset json = Resources.Load<TextAsset>("EventCardTexts");
        if (json == null)
        {
            Debug.LogError("EventCardTexts.json 파일을 Resources 폴더에 넣어주세요.");
            return;
        }

        EventCardTextData[] dataArray = JsonUtility.FromJson<Wrapper>(json.text).data;
        textDataMap = new Dictionary<string, EventCardTextData>();

        foreach (var data in dataArray)
        {
            textDataMap[data.EventID] = data;
        }
    }

    public void ApplyTextToCard(EventCard card)
    {
        if (textDataMap != null && textDataMap.TryGetValue(card.EventID, out var data))
        {
            card.EventText = data.EventText;
            card.ChoiceText1 = data.ChoiceText1;
            card.ChoiceText2 = data.ChoiceText2;
            card.ChoiceText3 = data.ChoiceText3;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public EventCardTextData[] data;
    }
}

