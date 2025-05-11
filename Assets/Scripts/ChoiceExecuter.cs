using System.Collections.Generic;
using UnityEngine;

public class ChoiceExecuter
{
    private EventCardManager eventCardManager;

    public ChoiceExecuter(EventCardManager eventCardManager) //eventCardManager를 가져오게 하려고 만든 생성자입니다.
    {
        this.eventCardManager = eventCardManager;
    }

    public bool CanDecreaseResource(string resourceName, int amount) //자원 감소가 가능한지를 평가하는 함수입니다.
    {
        switch (resourceName)
        {
            case "Food":
                return ResourceManager.Instance.Food >= amount;
            case "UtilityItem":
                return ResourceManager.Instance.UtilityItem >= amount;
            case "Medicine":
                return ResourceManager.Instance.Medicine >= amount;
            case "Defense":
                return ResourceManager.Instance.Defense >= amount;
            case "Mental":
                return ResourceManager.Instance.Mental >= amount;
            case "Madness":
                return ResourceManager.Instance.Madness >= amount;
            case "Population":
                return ResourceManager.Instance.Population >= amount;
            default:
                Debug.LogWarning($"알 수 없는 자원 이름: {resourceName}");
                return false;
        }
    }

    public void IncreaseResource(string resourceName, int amount) //자원을 증가시키는 함수입니다
    {
        switch (resourceName)
        {
            case "Food":
                ResourceManager.Instance.Food += amount;
                break;
            case "UtilityItem":
                ResourceManager.Instance.UtilityItem += amount;
                break;
            case "Medicine":
                ResourceManager.Instance.Medicine += amount;
                break;
            case "Defense":
                ResourceManager.Instance.Defense += amount;
                break;
            case "Mental":
                ResourceManager.Instance.Mental += amount;
                break;
            case "Madness":
                ResourceManager.Instance.Madness += amount;
                break;
            case "Population":
                ResourceManager.Instance.Population += amount;
                break;
            default:
                Debug.LogWarning($"알 수 없는 자원 이름: {resourceName}");
                break;
        }
    }

    public void DecreaseResource(string resourceName, int amount) //자원을 감소시키는 함수입니다. 평가없이 감소만 시도합니다.
    {
        switch (resourceName)
        {
            case "Food":
                ResourceManager.Instance.Food -= amount;
                break;
            case "UtilityItem":
                ResourceManager.Instance.UtilityItem -= amount;
                break;
            case "Medicine":
                ResourceManager.Instance.Medicine -= amount;
                break;
            case "Defense":
                ResourceManager.Instance.Defense -= amount;
                break;
            case "Mental":
                ResourceManager.Instance.Mental -= amount;
                break;
            case "Madness":
                ResourceManager.Instance.Madness -= amount;
                break;
            case "Population":
                ResourceManager.Instance.Population -= amount;
                break;
            default:
                Debug.LogWarning($"알 수 없는 자원 이름: {resourceName}");
                break;
        }
    }

    public void AddEventCardToDeck(string eventID, int delayDays) //이벤트 카드를 덱에 추가하고 섞습니다.
    {
        EventCard card = Resources.Load<EventCard>($"EventCards/{eventID}");
        if (card != null)
        {
            int targetDay = GameManager.day + delayDays;  //우선 현재 날짜를 기준으로 삼기 위해 GameManager에서 가져왔습니다.
            eventCardManager.AddEventCardWithShuffle(targetDay, card);
            Debug.Log($"{eventID} → {targetDay}일차 덱에 추가됨");
        }
        else
        {
            Debug.LogError($"EventCard {eventID} 로드 실패");
        }
    }

    public void AddNextEventCard(string eventID) //바로 다음 이벤트 카드로 해당 카드를 넣습니다.
    {
        EventCard card = Resources.Load<EventCard>($"EventCards/{eventID}");
        if (card != null)
        {
            int day = eventCardManager.currentCardDay; //현재 이벤트 카드가 기준인 것 같아서 eventCardManager에서 날짜 정보를 가져옵니다.
            int insertIndex = eventCardManager.currentCardIndex + 1;
            eventCardManager.InsertEventCardToDeck(day, card, insertIndex);
            Debug.Log($"{eventID} → {day}일차 {insertIndex}번 위치에 바로 다음 카드로 추가됨");
        }
        else
        {
            Debug.LogError($"EventCard {eventID} 로드 실패");
        }
    }
}