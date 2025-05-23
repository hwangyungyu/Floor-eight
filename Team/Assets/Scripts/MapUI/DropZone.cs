using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler  //드롭존, 각 지역을 담당합니다.
{
    public string zoneID;          //지역 ID, 각 지역은 이에 대해서 다른 값을 가져야합니다.
    public int maxCapacity = 5;
    private int currentCount = 0;

    public Text countText;

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

        dropped.transform.SetParent(parentTransform);

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
}