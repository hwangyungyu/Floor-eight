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
        DropZoneManager.Instance.RegisterDropZone(zoneID, this); //시작할때 드롭존 매니저에 자신을 등록해놓습니다.
    }

    public void OnDrop(PointerEventData eventData) //여기서 현재 드래그 중인 시민의 드롭을 실행합니다.
    {
        var dropped = eventData.pointerDrag; //드롭된 개체 판단
        if (dropped == null) return;

        var citizen = dropped.GetComponent<CitizenDrag>();
        if (citizen == null) return;

        // 시민에게 드롭 처리 요청
        citizen.HandleGroupDrop(this);
    }

    public bool RegisterCitizen(CitizenDrag citizen) //시민개체를 받아와서 이 드롭존에 가입시킵니다.
    {
        if (citizens.Count >= maxCapacity) return false;
        if (citizens.Contains(citizen)) return false;

        citizens.Add(citizen);
        citizen.assignedDropZone = this;
        citizen.transform.SetParent(parentTransform);
        UpdateCountText();
        return true;
    }

    public void UnregisterCitizen(CitizenDrag citizen) //시민개체를 받아와서 이 드롭존에서 가입해제
    {
        if (citizens.Remove(citizen))
        {
            UpdateCountText();
        }
    }

    public int GetRemainingCapacity() //남은 가능 수용량을 반환함
    {
        return maxCapacity - citizens.Count;
    }

    private void UpdateCountText() //드롭존에 있는 수량 텍스트 업데이트
    {
        countText.text = $"{citizens.Count} / {maxCapacity}";
        DropZoneManager.Instance.UpdateTotal();
    }

    public CitizenDrag GetLastCitizen() //마지막 시민을 반환
    {
        if (citizens.Count == 0) return null;
        return citizens[citizens.Count - 1];
    }

    /// <summary>
    /// 이하는 임시 이벤트 카드 관련 코드입니다.
    /// </summary>
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

