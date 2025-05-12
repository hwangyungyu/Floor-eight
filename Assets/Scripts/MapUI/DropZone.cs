using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DropZone : MonoBehaviour, IDropHandler  //드롭존, 각 지역에서 드래그를 담당합니다.
{
    public string zoneID;          //지역 ID, 각 지역은 이에 대해서 다른 값을 가져야합니다.
    public int maxCapacity = 5;
    private int currentCount = 0;

    public Text countText;

    [SerializeField] private List<EventCard> eventCards; //테스트 용으로 임시적으로 이 클래스가 지역 이벤트 카드를 가지도록 했습니다.

    public RectTransform parentTransform;           //시민을 드래그 받았을때 이 Transform에 배치시킵니다.

    private void Start()
    {
        UpdateCountText();
        DropZoneManager.Instance.RegisterDropZone(zoneID, this); // 이 드롭존을 드롭존 매니저의 딕셔너리에 등록
    }


    public void OnDrop(PointerEventData eventData) //드롭시, 드롭된 오브젝트를 판단하고 배치구역에 배치시킴
    {
        GameObject dropped = eventData.pointerDrag;

        if (dropped == null || dropped.GetComponent<CitizenDrag>() == null || currentCount >= maxCapacity)
            return;

        CitizenDrag citizen = dropped.GetComponent<CitizenDrag>();

        if (citizen.AssignedDropZone != null) return;

        dropped.transform.SetParent(parentTransform);

        CanvasGroup cg = dropped.GetComponent<CanvasGroup>();
        cg.alpha = 1;

        // 드롭 성공 시
        currentCount++;
        UpdateCountText();

        var drag = dropped.GetComponent<CitizenDrag>();
        drag.AssignedDropZone = this; // 되돌릴 때 필요
    }

    public void DecreaseCount() // 이 드롭존에 배치된 시민 숫자를 감소
    {
        currentCount = Mathf.Max(0, currentCount - 1);
        UpdateCountText();
    }

    public int GetCurrentCount() // 시민 수량 내보내기
    {
        return currentCount;
    }

    void UpdateCountText() // 지역 배치 시민 수량을 업데이트, 동시에 전체 시민 배치 업데이트
    {
        countText.text = $"{currentCount} / {maxCapacity}";
        DropZoneManager.Instance.UpdateTotal();
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