using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class DropZone : MonoBehaviour, IDropHandler
{
    public bool isBundle = false;
    public string zoneID;
    public int maxCapacity = 5;

    public List<CitizenDrag> citizens = new List<CitizenDrag>();

    public Text countText;
    public RectTransform parentTransform;

    [SerializeField] private List<EventCard> eventCards; //테스트 용으로 임시적으로 이 클래스가 지역 이벤트 카드를 가지도록 했습니다.

    private void Start()
    {
        UpdateCountText();
        DropZoneManager.Instance.RegisterDropZone(zoneID, this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dropped = eventData.pointerDrag;
        if (dropped == null) return;

        var citizen = dropped.GetComponent<CitizenDrag>();
        if (citizen == null) return;

        // 시민에게 드롭 처리 요청
        citizen.HandleGroupDrop(this);
    }

    public bool RegisterCitizen(CitizenDrag citizen)
    {
        if (citizens.Count >= maxCapacity) return false;
        if (citizens.Contains(citizen)) return false;

        citizens.Add(citizen);
        citizen.assignedDropZone = this;
        citizen.transform.SetParent(parentTransform);
        UpdateCountText();
        return true;
    }

    public void UnregisterCitizen(CitizenDrag citizen)
    {
        if (citizens.Remove(citizen))
        {
            UpdateCountText();
        }
    }

    public int GetRemainingCapacity()
    {
        return maxCapacity - citizens.Count;
    }

    private void UpdateCountText()
    {
        countText.text = $"{citizens.Count} / {maxCapacity}";
        DropZoneManager.Instance.UpdateTotal();
    }

    public CitizenDrag GetLastCitizen()
    {
        if (citizens.Count == 0) return null;
        return citizens[citizens.Count - 1];
    }

    public List<EventCard> GetRandomEventCards(int amount) //테스트용 이벤트 카드 리스트 반환 함수
    {
        if (eventCards == null || eventCards.Count == 0)
        {
            Debug.Log("이벤트 카드 없음!");
            return null;
        }

        if (eventCards.Count <= amount)
            return new List<EventCard>(eventCards);

        List<EventCard> shuffled = new List<EventCard>(eventCards);
        // 리스트를 무작위로 섞음
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
        }

        return shuffled.GetRange(0, amount);
    }
}

