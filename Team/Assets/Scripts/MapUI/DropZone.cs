using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    public string zoneID;
    public int maxCapacity = 5;
    private int currentCount = 0;

    public Text countText;

    public RectTransform parentTransform;

    private void Start()
    {
        UpdateCountText();
        DropZoneManager.Instance.RegisterDropZone(zoneID, this);
    }


    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        if (dropped == null || dropped.GetComponent<CitizenDrag>() == null || currentCount >= maxCapacity)
            return;

        dropped.transform.SetParent(parentTransform);

        // 드롭 성공 시
        currentCount++;
        UpdateCountText();
        DropZoneManager.Instance.UpdateTotal();

        var drag = dropped.GetComponent<CitizenDrag>();
        drag.MarkAsPlaced();
        drag.AssignedDropZone = this; // 되돌릴 때 필요
    }

    public void DecreaseCount()
    {
        currentCount = Mathf.Max(0, currentCount - 1);
        UpdateCountText();
    }

    public int GetCurrentCount()
    {
        return currentCount;
    }
    public int GetMaxCapacity()
    {
        return maxCapacity;
    }
    void UpdateCountText()
    {
        countText.text = $"{currentCount} / {maxCapacity}";
    }
}