using System;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceExecuter
{
    private EventCardManager eventCardManager;

    public ChoiceExecuter(EventCardManager eventCardManager) //eventCardManager를 가져오게 하려고 만든 생성자입니다.
    {
        this.eventCardManager = eventCardManager;
    }

    public bool CanDecreaseResource(string resourceName, int amount) //자원 감소가 가능한지 판단하는 함수.
    {
        string name = resourceName.ToLower();
        switch (name)
        {
            case "food":
                return ResourceManager.Instance.Food >= amount;
            case "utilityitem":
                return ResourceManager.Instance.UtilityItem >= amount;
            case "medicine":
                return ResourceManager.Instance.Medicine >= amount;
            case "defense":
                return ResourceManager.Instance.Defense >= amount;
            case "mental":
                return ResourceManager.Instance.Mental >= amount;
            case "madness":
                return ResourceManager.Instance.Madness >= amount;
            case "population":
                return ResourceManager.Instance.Population >= amount;
            default:
                Debug.LogWarning($"알 수 없는 자원 이름: {resourceName}");
                return false;
        }
    }

    public void IncreaseResource(string resourceName, int amount) //자원을 증가 시키는 함수.
    {
        string name = resourceName.ToLower();
        switch (name)
        {
            case "food":
                ResourceManager.Instance.Food += amount;
                break;
            case "utilityitem":
                ResourceManager.Instance.UtilityItem += amount;
                break;
            case "medicine":
                ResourceManager.Instance.Medicine += amount;
                break;
            case "defense":
                ResourceManager.Instance.Defense += amount;
                break;
            case "mental":
                ResourceManager.Instance.Mental += amount;
                break;
            case "madness":
                ResourceManager.Instance.Madness += amount;
                break;
            case "population":
                ResourceManager.Instance.Population += amount;
                break;
            default:
                Debug.LogWarning($"알 수 없는 자원 이름: {resourceName}");
                break;
        }
    }

    public void DecreaseResource(string resourceName, int amount) //자원을 감소시키는 함수.
    {
        string name = resourceName.ToLower();
        switch (name)
        {
            case "food":
                ResourceManager.Instance.Food -= amount;
                break;
            case "utilityitem":
                ResourceManager.Instance.UtilityItem -= amount;
                break;
            case "medicine":
                ResourceManager.Instance.Medicine -= amount;
                break;
            case "defense":
                ResourceManager.Instance.Defense -= amount;
                break;
            case "mental":
                ResourceManager.Instance.Mental -= amount;
                break;
            case "madness":
                ResourceManager.Instance.Madness -= amount;
                break;
            case "population":
                ResourceManager.Instance.Population -= amount;
                break;
            default:
                Debug.LogWarning($"알 수 없는 자원 이름: {resourceName}");
                break;
        }
    }

    public void AddEventCardToDeck(string eventID, int delayDays, string area) //이벤트 카드를 덱에 추가하고 섞습니다.
    {
        EventCard card = GameManager.Instance.eventCardManager.GetEventCardById(eventID);
        if (card != null)
        {
            int targetDay = GameManager.Day + delayDays;  //우선 현재 날짜를 기준으로 삼기 위해 GameManager에서 가져왔습니다.
            eventCardManager.AddEventCardWithShuffle(targetDay, card, area);
            Debug.Log($"{eventID} → {targetDay}일차 덱에 추가됨");
        }
        else
        {
            Debug.LogError($"EventCard {eventID} 로드 실패");
        }
    }

    public void AddNextEventCard(string eventID, string area) //바로 다음 이벤트 카드로 해당 카드를 넣습니다.
    {
        EventCard card = GameManager.Instance.eventCardManager.GetEventCardById(eventID);
        if (card != null)
        {
            int day = GameManager.Day;
            int insertIndex = eventCardManager.currentCardIndex + 1;
            eventCardManager.InsertEventCardToDeck(day, card, insertIndex, area);
            Debug.Log($"{eventID} → {day}일차 {insertIndex}번 위치에 바로 다음 카드로 추가됨");
        }
        else
        {
            Debug.LogError($"EventCard {eventID} 로드 실패");
        }
    }

    public void ExecuteEffect(string effect) //선택지 효과 처리
    {
        string[] parts = effect.Split(' ');
        if (parts.Length == 0) return;

        string areaID = GameManager.Instance.eventCardManager.currentCardArea;
        Area area = null;

        if (!string.IsNullOrEmpty(areaID))
            AreaManager.Instance.areas.TryGetValue(areaID, out area);

        switch (parts[0])
        {
            case "ItemIncrease":
                if (parts.Length >= 3)
                {
                    string resourceName = parts[1].ToLower();
                    if (int.TryParse(parts[2], out int baseAmount))
                    {
                        int resourceIndex = ResourceManager.Instance.GetResourceIndex(resourceName);
                        int bonus = (area != null && resourceIndex >= 0) ? area.currentBonus[resourceIndex] : 0;
                        int totalAmount = Math.Max(0, baseAmount + bonus);
                        IncreaseResource(resourceName, totalAmount);
                    }
                }
                break;

            case "ItemDecrease":
                if (parts.Length >= 3)
                {
                    string resourceName = parts[1].ToLower();
                    if (int.TryParse(parts[2], out int baseAmount))
                    {
                        int resourceIndex = ResourceManager.Instance.GetResourceIndex(resourceName);
                        int penalty = (area != null && resourceIndex >= 0) ? area.currentPenalty[resourceIndex] : 0;
                        int totalAmount = Math.Max(0, baseAmount + penalty);
                        DecreaseResource(resourceName, totalAmount);
                    }
                }
                break;

            case "Bonus":
                if (parts.Length >= 3)
                {
                    string resourceName = parts[1].ToLower();
                    if (int.TryParse(parts[2], out int bonusValue))
                    {
                        Area targetArea = area;
                        if (parts.Length >= 4)
                        {
                            string targetAreaID = parts[3];
                            AreaManager.Instance.areas.TryGetValue(targetAreaID, out targetArea);
                        }
                        if (targetArea != null)
                        {
                            int resourceIndex = ResourceManager.Instance.GetResourceIndex(resourceName);
                            if (resourceIndex >= 0)
                            {
                                targetArea.currentBonus[resourceIndex] += bonusValue;
                            }
                        }
                    }
                }
                break;

            case "Penalty":
                if (parts.Length >= 3)
                {
                    string resourceName = parts[1].ToLower();
                    if (int.TryParse(parts[2], out int penaltyValue))
                    {
                        Area targetArea = area;
                        if (parts.Length >= 4)
                        {
                            string targetAreaID = parts[3];
                            AreaManager.Instance.areas.TryGetValue(targetAreaID, out targetArea);
                        }
                        if (targetArea != null)
                        {
                            int resourceIndex = ResourceManager.Instance.GetResourceIndex(resourceName);
                            if (resourceIndex >= 0)
                            {
                                targetArea.currentPenalty[resourceIndex] += penaltyValue;
                            }
                        }
                    }
                }
                break;

            case "SetFlag":
                if (parts.Length >= 3)
                {
                    string flagName = parts[1];
                    bool flagValue = parts[2].ToLower() == "true";
                    FlagManager.Instance.SetFlag(flagName, flagValue);
                }
                break;

            case "AddNextEventCard":
                if (parts.Length >= 2)
                    AddNextEventCard(parts[1], (areaID ?? null));
                break;

            case "AddEventCardToDeck":
                if (parts.Length >= 3 && int.TryParse(parts[2], out int count))
                    AddEventCardToDeck(parts[1], count, (areaID ?? null));
                break;
            case "AdditionalMessage":
                break;
            default:
                Debug.LogWarning("알 수 없는 효과: " + effect);
                break;
        }
    }
}