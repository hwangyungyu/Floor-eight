using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class DropZone : MonoBehaviour, IDropHandler
{
    public bool isBundle = false;
    public int MaxCapacity //이 드롭존의 시민 최대 수용량입니다. 연결된 지역에서 값을 반환합니다.
    {
        get
        {
            if (isBundle || linkedArea == null)
                return ResourceManager.Instance.Population; //지역이 번들일시, 현재 시민 자원의 갯수를 반환합니다.
            return linkedArea.maxCitizenCapacity;
        }
    }
    public Area linkedArea; //연결된 지역입니다. 이 드롭존은 이제 지역의 시민의 배치에 대해서만 담당합니다.
    public List<CitizenDrag> citizens = new List<CitizenDrag>();
    public RectTransform parentTransform;

    private void Start()
    {
        DropZoneManager.Instance.RegisterDropZone(this); //시작할때 드롭존 매니저에 자신을 등록해놓습니다.
    }

    public void ReturnAllCitizen() //모든 시민을 번들로 되돌립니다.
    {
        List<CitizenDrag> toRemove = new List<CitizenDrag> (citizens);
        foreach(CitizenDrag citizen in toRemove)
        {
            citizen.ReturnToBundle();
        }
        toRemove.Clear();
    }

    public void OnDrop(PointerEventData eventData) //여기서 현재 드래그 중인 시민의 드롭을 실행합니다.
    {
        if (linkedArea.isEnabled == false) return;

        var dropped = eventData.pointerDrag; //드롭된 개체 판단
        if (dropped == null) return;

        var citizen = dropped.GetComponent<CitizenDrag>();
        if (citizen == null) return;

        // 시민에게 드롭 처리 요청
        citizen.HandleGroupDrop(this);
    }

    public bool RegisterCitizen(CitizenDrag citizen) //시민개체를 받아와서 이 드롭존에 가입시킵니다.
    {
        if (citizens.Count >= MaxCapacity) return false;
        if (citizens.Contains(citizen)) return false;

        citizens.Add(citizen);
        citizen.assignedDropZone = this;
        citizen.transform.SetParent(parentTransform);

        
        linkedArea?.OnCitizenAssigned(citizen); // 지역에 시민 수 변경 통보
        return true;
    }

    public void UnregisterCitizen(CitizenDrag citizen) //시민개체를 받아와서 이 드롭존에서 가입해제
    {
        if (citizens.Remove(citizen))
        {   
            linkedArea?.OnCitizenUnassigned(citizen); // 지역에 시민 해제 통보
        }
    }

    public int GetRemainingCapacity() //남은 가능 수용량을 반환함
    {
        return MaxCapacity - citizens.Count;
    }

    public CitizenDrag GetLastCitizen() //마지막 시민을 반환
    {
        if (citizens.Count == 0) return null;
        return citizens[citizens.Count - 1];
    }
}

