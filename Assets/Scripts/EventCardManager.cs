using System.Collections.Generic;
using UnityEngine;

public class EventCardManager : MonoBehaviour
{
    private static List<EventCardDeck> eventCardDeckList = new List<EventCardDeck>(); //이 리스트에서 0번 인덱스는 사용하지 않습니다. 1일차는 1번 인덱스로 생각해주세요.

    private EventCard currentEventCard; // 현재 화면에 표시 중인 이벤트 카드
    public EventCard CurrentEventCard => currentEventCard;

    public int currentCardDay = 1;  //DrawEventCard()에서 현재 카드가 어느 곳에서 나왔는지를 저장하는 변수들
    public int currentCardIndex = -1;  //임시로 1일차 -1번을 현재 카드로 지정했습니다. DrawEventCard를 사용시 1일차 0번을 가져옵니다.

    // EventCardDeck에서 다음 카드를 가져옴
    public void DrawEventCard()
    {
        // 현재 날짜가 유효하지 않다면 종료
        if (!IsValidDay(currentCardDay)) return;

        EventCardDeck currentDeck = eventCardDeckList[currentCardDay];

        // 현재 날짜의 카드 덱이 비어있으면 경고 출력 후 종료
        if (currentDeck.EventCardCount == 0)
        {
            Debug.LogWarning("현재 날짜의 카드 덱이 비어 있습니다.");
            return;
        }

        // 현재 카드가 마지막 카드라면 다음날 첫번째 카드 불러오기(이 코드가 다음날로 넘어가게 해도 되는지?) 이거 수정해야 할거 같습니다.
        if (currentCardIndex >= currentDeck.EventCardCount - 1)
        {
            currentCardDay++;
            currentCardIndex = -1;

            // 다음 날이 유효한지 확인
            if (!IsValidDay(currentCardDay))
            {
                Debug.LogWarning("더 이상 가져올 이벤트 카드가 없습니다.");
                return;
            }

            // 다음 날의 덱을 다시 설정
            if (currentCardDay < eventCardDeckList.Count)
            {
                currentDeck = eventCardDeckList[currentCardDay];
            }
            else
            {
                Debug.LogWarning("모든 이벤트 카드를 소진했습니다.");
                return;
            }
        }

        // Index변수 업데이트 후 카드 덮어쓰기
        currentCardIndex++;
        currentEventCard = currentDeck.GetEventCard(currentCardIndex);
    }

    // 특정날짜의 덱에 카드 삽입
    public void InsertEventCardToDeck(int day, EventCard eventCard, int index)
    {
        if (IsValidDay(day))
        {
            eventCardDeckList[day].InsertEventCard(eventCard, index);
        }
    }

    // 특정 날짜(day)의 카드 덱에 이벤트 카드를 추가한 후 섞음
    public void AddEventCardWithShuffle(int day, EventCard eventCard)
    {
        if (IsValidDay(day))
        {
            eventCardDeckList[day].AddEventCard(eventCard);
            eventCardDeckList[day].ShuffleDeck();
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
}

public class EventCardDeck
{
    private List<EventCard> eventCardList = new List<EventCard>();

    public int EventCardCount => eventCardList.Count;  //카드 수량을 파악하기 위해 추가

    public EventCard GetEventCard(int index)
    {
        if (index < 0 || index > eventCardList.Count)
        {
            Debug.LogWarning("잘못된 인덱스 값입니다.");
            return null;
        }
        return eventCardList[index];
    }

    //덱 섞기
    public void ShuffleDeck()
    {
        for (int i = eventCardList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (eventCardList[i], eventCardList[randomIndex]) = (eventCardList[randomIndex], eventCardList[i]);
        }
    }

    //가장 마지막에 eventCard를 추가
    public void AddEventCard(EventCard eventCard)
    {
        eventCardList.Add(eventCard);
    }

    //인덱스에 eventCard 삽입
    public void InsertEventCard(EventCard eventCard, int index)
    {
        if (index < 0 || index > eventCardList.Count)
        {
            Debug.LogWarning("잘못된 인덱스 값입니다.");
            return;
        }
        eventCardList.Insert(index, eventCard);
    }

    // 지정된 인덱스의 이벤트 카드를 제거하고 반환
    public EventCard RemoveEventCard(int index = -1)
    {
        if (eventCardList.Count == 0)
        {
            Debug.LogWarning("덱이 비어 있습니다.");
            return null;
        }

        // 기본적으로 마지막 카드를 제거
        if (index < 0 || index >= eventCardList.Count)
        {
            index = eventCardList.Count - 1;
        }

        EventCard removedCard = eventCardList[index];
        eventCardList.RemoveAt(index);
        return removedCard;
    }
}