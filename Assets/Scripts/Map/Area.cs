using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Area : MonoBehaviour
{
    [Header("관리 지역 여부")]
    public bool isManage = false;
    [Header("초기화에 사용할 지역 데이터")]
    public AreaData template;
    [Header("연결된 드롭존")]
    public DropZone linkedDropZone;
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
    public Image image;

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

    public void AreaActive(bool tf) //지역 활성화 및 비활성화
    {
        isEnabled = tf;
        countText.enabled = isEnabled;
        image.enabled = isEnabled;
        if (!isEnabled)
        {
            if(linkedDropZone != null) linkedDropZone.ReturnAllCitizen();
        }
        UpdateCountText();
    }

    public void InitializeFromTemplate(AreaData data)
    {
        areaID = data.areaID;
        isEnabled = data.enabled;
        maxCitizenCapacity = data.maxCapacity;
        availableEvents = new List<string>(data.events);
        if (data.bonus != null && data.bonus.Count == 7)
            currentBonus = new List<int>(data.bonus);
        else
            currentBonus = new List<int> { 0, 0, 0, 0, 0, 0, 0};

        if (data.penalty != null && data.penalty.Count == 7)
            currentPenalty = new List<int>(data.penalty);
        else
            currentPenalty = new List<int> { 0, 0, 0, 0, 0, 0, 0};

        AreaActive(isEnabled);
    }
    public void OnCitizenAssigned(CitizenDrag citizen) //시민 수량 증가
    {
        currentCitizenAmount += 1;
        Debug.Log($"{areaID}: 시민 {citizen.name} 배치됨");
        UpdateCountText();
    }

    public void OnCitizenUnassigned(CitizenDrag citizen) //시민 수량 감소
    {
        currentCitizenAmount -= 1;
        Debug.Log($"{areaID}: 시민 {citizen.name} 해제됨");
        UpdateCountText();
    }

    public List<EventCardInfo> GetRandomEventCards(int amount)
    {
        if (availableEvents == null || availableEvents.Count == 0)
        {
            Debug.LogWarning($"지역 {areaID}에 등록된 이벤트 ID가 없습니다.");
            return null;
        }

        // 조건을 만족하는 이벤트 ID만 필터링
        List<string> validEventIDs = new List<string>();
        foreach (string id in availableEvents)
        {
            EventCard card = GameManager.Instance.eventCardManager.GetEventCardById(id);
            if (card == null)
            {
                Debug.LogError($"이벤트 ID '{id}'에 해당하는 EventCard 리소스를 찾을 수 없습니다.");
                continue;
            }

            if (CheckEventCardConditions(card))
            {
                validEventIDs.Add(id);
            }
        }

        if (validEventIDs.Count == 0)
        {
            Debug.LogWarning($"지역 {areaID}에서 등장 조건을 만족하는 이벤트가 없습니다.");
            return null;
        }

        // 유효한 ID 리스트 셔플
        for (int i = 0; i < validEventIDs.Count; i++)
        {
            int randomIndex = Random.Range(i, validEventIDs.Count);
            (validEventIDs[i], validEventIDs[randomIndex]) = (validEventIDs[randomIndex], validEventIDs[i]);
        }

        // 개수만큼 EventCard 로드
        List<EventCardInfo> result = new List<EventCardInfo>();
        int loadCount = Mathf.Min(amount, validEventIDs.Count);

        for (int i = 0; i < loadCount; i++)
        {
            string id = validEventIDs[i];
            EventCard card = GameManager.Instance.eventCardManager.GetEventCardById(id);
            EventCardInfo info = new EventCardInfo(areaID, card);
            result.Add(info);
        }

        return result;
    }

    private void UpdateCountText() //드롭존에 있는 수량 텍스트 업데이트
    {
        countText.text = $"{currentCitizenAmount} / {maxCitizenCapacity}";
        AreaManager.Instance.UpdateTotal();
    }
    private bool CheckEventCardConditions(EventCard card) //카드 트리거 조건 검사하기
    {
        ResourceManager resourceManager = ResourceManager.Instance;
        FlagManager flagManager = FlagManager.Instance;

        // 자원 조건 검사
        foreach (ItemTrigger trigger in card.ItemTrigger)
        {
            int current = resourceManager.GetResourceByName(trigger.itemName);
            if (current < trigger.requiredAmount)
            {
                return false;
            }
        }

        //특수 자원 조건 검사
        foreach (SpecialResourceTrigger trigger in card.SpecialResourceTrigger)
        {
            int current = resourceManager.GetResourceByName(trigger.resourceName);
            if (current < trigger.requiredAmount)
            {
                return false;
            }
        }

        // 플래그 조건 검사
        foreach (FlagTrigger trigger in card.FlagTrigger)
        {
            bool value = flagManager.GetFlag(trigger.flagName);
            if (value != trigger.requiredValue)
            {
                return false;
            }
        }

        return true;
    }
}