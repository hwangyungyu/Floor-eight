using System.Collections.Generic;
using UnityEngine;

public class EventCardManager //우선 MonoBehavior로 작성하라는 부분이 없어서 GameManager에 연결되서 관리하는 형식입니다.
{
    private static List<EventCardDeck> eventCardDeckList = new List<EventCardDeck>(); //이 리스트에서 0번 인덱스는 사용하지 않습니다. 1일차는 1번 인덱스로 생각해주세요.

    private EventCard currentEventCard; // 현재 화면에 표시 중인 이벤트 카드
    public EventCard CurrentEventCard => currentEventCard;

    private Dictionary<string, EventCard> eventCardMap = new Dictionary<string, EventCard>();

    public string currentCardArea;
    public int currentCardDay = 1;  //현재 날짜 관리 변수 *중요함*
    public int currentCardIndex = -1;  //임시로 1일차 -1번을 현재 카드로 지정했습니다. DrawEventCard를 사용시 1일차 0번을 가져옵니다.

    public void LoadAllEventCards() //시작시 한번 호출되어 이벤트 카드 전체를 불러옵니다.
    {
        EventCard[] cards = Resources.LoadAll<EventCard>("EventCards");
        // 카드를 외부 json형태로 받아오기 위함 (07/08일 추가)
        var textLoader = new EventCardTextLoader();

        foreach (var card in cards)
        {
            textLoader.ApplyTextToCard(card);  // JSON에서 텍스트 불러와 주입함 (07/08 추가)

            if (!eventCardMap.ContainsKey(card.EventID)) // EventID는 ScriptableObject의 이름
            {
                eventCardMap.Add(card.EventID, card);
            }
            else
            {
                Debug.LogWarning($"중복된 EventID가 있습니다: {card.EventID}");
            }
        }

        Debug.Log($"총 {eventCardMap.Count}개의 이벤트 카드 로드됨");
    }

    public EventCard GetEventCardById(string id) //이벤트 카드 조회
    {
        if (eventCardMap.TryGetValue(id, out var card))
            return card;

        Debug.LogError($"이벤트 ID {id}에 해당하는 카드 없음");
        return null;
    }

    public void ChangeDay(int num) //입력된 수만큼 날짜를 변경합니다. 급하게 만들어서 예외처리가 없습니다! 오류에 주의해주세요!
    {
        currentCardDay += num;
        currentCardIndex = -1;
    }
    public void SetDay(int num) //입력된 수로 날짜를 변경합니다.
    {
        currentCardDay = num;
        currentCardIndex = -1;
    }


    public void InitializeDeck(int day) //지정된 일차까지 덱 생성
    {
        while(eventCardDeckList.Count <= day)
        {
            eventCardDeckList.Add(new EventCardDeck());
        }
    }

    // EventCardDeck에서 다음 카드를 가져옴
    public bool DrawEventCard()
    {
        Debug.Log(currentCardDay);
        // 현재 날짜가 유효하지 않다면 종료
        if (!IsValidDay(currentCardDay)) return false;

        EventCardDeck currentDeck = eventCardDeckList[currentCardDay];

        // 현재 날짜의 카드 덱이 비어있으면 종료
        if (currentDeck.EventCardCount == 0)
        {
            Debug.LogWarning("현재 날짜의 카드 덱이 비어 있습니다.");
            return false;
        }

        // 현재 카드가 마지막 카드인 경우
        if (currentCardIndex >= currentDeck.EventCardCount - 1)
        {
            Debug.Log("당일 이벤트 카드 소진");
            return false;
        }

        // 정상 진행
        currentCardIndex++;
        EventCardInfo eventCardInfo = currentDeck.GetEventCard(currentCardIndex);
        currentEventCard = eventCardInfo.Card;
        currentCardArea = eventCardInfo.Area;
        return true;
    }

    // 특정날짜의 덱에 카드 삽입
    public void InsertEventCardToDeck(int day, EventCard eventCard, int index, string area = null)
    {
        if (IsValidDay(day))
        {
            eventCardDeckList[day].InsertEventCard(eventCard, index, area);
        }
    }

    // 특정 날짜(day)의 카드 덱에 이벤트 카드를 추가한 후 섞음
    public void AddEventCardWithShuffle(int day, EventCard eventCard, string area = null)
    {
        if (IsValidDay(day))
        {
            eventCardDeckList[day].AddEventCard(eventCard, area);
            eventCardDeckList[day].ShuffleDeck();
        }
    }
    public void AddEventCardWithoutShuffle(int day, EventCard eventCard, string area = null)
    {
        if (IsValidDay(day))
        {
            eventCardDeckList[day].AddEventCard(eventCard, area);
        }
    }

    // 특정 날짜(day)의 카드 덱에서 지정된 위치(index)의 이벤트 카드를 제거
    public void RemoveEventCard(int day, int index)
    {
        if (IsValidDay(day))
        {
            eventCardDeckList[day].RemoveEventCard(index);
        }
    }

    // 유효한 날짜(day)인지 확인하는 함수
    private bool IsValidDay(int day)
    {
        return day >= 0 && day < eventCardDeckList.Count;
    }

    // --- Save/Load Methods ---
    public List<EventCardDeck> GetDecksForSave()
    {
        return eventCardDeckList;
    }

    public void RestoreDecks(List<EventCardDeck> loadedDecks)
    {
        eventCardDeckList = loadedDecks;
    }
}

public class EventCardDeck
{
    private List<EventCardInfo> eventCardList = new List<EventCardInfo>();

    public int EventCardCount => eventCardList.Count;

    public EventCardInfo GetEventCard(int index)
    {
        if (index < 0 || index >= eventCardList.Count)
        {
            Debug.LogWarning("잘못된 인덱스 값입니다.");
            return null;
        }
        return eventCardList[index];
    }

    public void ShuffleDeck()
    {
        for (int i = eventCardList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (eventCardList[i], eventCardList[randomIndex]) = (eventCardList[randomIndex], eventCardList[i]);
        }
    }

    public void AddEventCard(EventCard eventCard, string area)
    {
        eventCardList.Add(new EventCardInfo(area, eventCard));
    }

    public void InsertEventCard(EventCard eventCard, int index, string area)
    {
        if (index < 0 || index > eventCardList.Count)
        {
            Debug.LogWarning("잘못된 인덱스 값입니다.");
            return;
        }
        eventCardList.Insert(index, new EventCardInfo(area, eventCard));
    }

    public EventCardInfo RemoveEventCard(int index = -1)
    {
        if (eventCardList.Count == 0)
        {
            Debug.LogWarning("덱이 비어 있습니다.");
            return null;
        }

        if (index < 0 || index >= eventCardList.Count)
        {
            index = eventCardList.Count - 1;
        }

        EventCardInfo removedCard = eventCardList[index];
        eventCardList.RemoveAt(index);
        return removedCard;
    }

    // for Save/Load
    public List<EventCardInfo> GetCardInfoList()
    {
        return eventCardList;
    }

    public void SetCardInfoList(List<EventCardInfo> list)
    {
        eventCardList = list;
    }
}
public class EventCardInfo
{
    public string Area { get; set; }
    public EventCard Card { get; set; }

    public EventCardInfo(string area, EventCard card)
    {
        Area = area;
        Card = card;
    }
}
