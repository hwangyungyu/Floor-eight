using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Area : MonoBehaviour
{
    [Header("초기화에 사용할 지역 데이터")]
    public AreaData template;

    [Header("런타임 상태")]
    public string areaID;
    public bool isEnabled;
    public List<string> availableEvents;
    public int maxCitizenCapacity;
    public int currentCitizenAmount = 0;
    [Header("런타임 상태 - 자원 관련")]
    [Header("{음식, 잡동사니, 의약품, 방어, 정신력, 광기, 인구}")]
    public List<int> currentBonus; //음식, 잡동사니, 의약품, 방어, 정신력, 광기, 인구 순서
    public List<int> currentPenalty;


    [Header("UI관련")]
    public Text countText;

    private void Start()
    {
        if (template == null)
        {
            Debug.LogError($"{name} 지역: 초기 데이터가 비어 있습니다.");
            return;
        }

        InitializeFromTemplate(template);
        AreaManager.Instance.RegisterArea(areaID, this);
        UpdateCountText();
    }

    public void InitializeFromTemplate(AreaData data)
    {
        areaID = data.areaID;
        isEnabled = data.enabled;
        maxCitizenCapacity = data.maxCapacity;
        availableEvents = new List<string>(data.events);
        if (data.bonus != null)
            currentBonus = new List<int>(data.bonus);
        else
            currentBonus = new List<int> { 0, 0, 0, 0, 0, 0 };

        if (data.penalty != null)
            currentPenalty = new List<int>(data.penalty);
        else
            currentPenalty = new List<int> { 0, 0, 0, 0, 0, 0 };
    }
    public void OnCitizenAssigned(CitizenDrag citizen)
    {
        currentCitizenAmount += 1;
        Debug.Log($"{areaID}: 시민 {citizen.name} 배치됨");
        UpdateCountText();
    }

    public void OnCitizenUnassigned(CitizenDrag citizen)
    {
        currentCitizenAmount -= 1;
        Debug.Log($"{areaID}: 시민 {citizen.name} 해제됨");
        UpdateCountText();
    }

    public List<EventCard> GetRandomEventCards(int amount)
    {
        if (availableEvents == null || availableEvents.Count == 0)
        {
            Debug.LogWarning($"지역 {areaID}에 등록된 이벤트 ID가 없습니다.");
            return null;
        }

        // 이벤트 ID 리스트 복사 및 셔플
        List<string> shuffledIDs = new List<string>(availableEvents);
        for (int i = 0; i < shuffledIDs.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledIDs.Count);
            (shuffledIDs[i], shuffledIDs[randomIndex]) = (shuffledIDs[randomIndex], shuffledIDs[i]);
        }

        // 개수만큼 EventCard 로드
        List<EventCard> result = new List<EventCard>();
        int loadCount = Mathf.Min(amount, shuffledIDs.Count);

        for (int i = 0; i < loadCount; i++)
        {
            string id = shuffledIDs[i];
            EventCard card = GameManager.Instance.eventCardManager.GetEventCardById(id);

            if (card != null)
            {
                result.Add(card);
            }
            else
            {
                Debug.LogError($"이벤트 ID '{id}'에 해당하는 EventCard 리소스를 찾을 수 없습니다.");
            }
        }

        return result;
    }

    private void UpdateCountText() //드롭존에 있는 수량 텍스트 업데이트
    {
        countText.text = $"{currentCitizenAmount} / {maxCitizenCapacity}";
        AreaManager.Instance.UpdateTotal();
    }
}