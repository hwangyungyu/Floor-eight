using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class CitizenDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public DropZone bundle;                 // 귀환 시 수량 회복용
    public DropZone assignedDropZone; // 현재 등록된 드롭존
    private DropZone tempLastDropZone; //이전에 등록된 드롭존 저장(복귀용)
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Button returnButton;

    private List<CitizenDrag> followers = new List<CitizenDrag>();
    private List<Vector2> followerOffsets = new();

    private List<RectTransform> followerRects = new List<RectTransform>();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        returnButton = GetComponent<Button>();
    }

    public void AssignButtonEvent() //개체 생성과정에서 함께 호출중, 복귀 코드를 버튼에 배정하기 위함
    {
        returnButton.onClick.AddListener(ReturnToBundle);
    }

    public void ReturnToBundle() //번들로 복귀
    {
        if (assignedDropZone != null)
            assignedDropZone.UnregisterCitizen(this); //현재 구조상 무조건 원래 드롭존에서 해제를 먼저 해야합니다.

        if (bundle != null)
            bundle.RegisterCitizen(this);
    }
    public void OnBeginDrag(PointerEventData eventData) //드래그 시작시 호출
    {
        canvasGroup.blocksRaycasts = false;
        tempLastDropZone = assignedDropZone; //이전에 배정된 드롭존 임시 저장(비교용)

        // 추종자 확보 및 상대 위치 계산
        followers = GetFollowingCitizens();
        followerOffsets.Clear();
        followerRects.Clear();

        //화면상에서 보이는 순서를 조정하기 위해 부모개체를 전환
        transform.SetParent(canvas.transform); //혹시 이렇게 부모개체로 인해 발생할 수 있는 오류가 있을 경우 확인 필요

        
        for (int i = 0; i < followers.Count; i++)
        {
            var follower = followers[i];
            follower.tempLastDropZone = follower.assignedDropZone; //따라오는 개체들도 현재 드롭존 임시 저장
            // 겹쳐진 느낌을 주기 위한 오프셋 설정
            Vector2 offset = new Vector2(20+20f * i, 0);
            followerOffsets.Add(offset);

            follower.canvasGroup.blocksRaycasts = false;
            follower.transform.SetParent(canvas.transform); //화면상에서 보이는 순서를 조정하기 위해 부모개체를 전환
            // 드래그 중인 시민의 바로 아래부터 계층에 순차적으로 배치
            follower.transform.SetSiblingIndex(transform.GetSiblingIndex() + -i + 1);

            followerRects.Add(follower.GetComponent<RectTransform>());
        }
        transform.SetAsLastSibling(); //현재 객체를 화면에서 가장 앞에 보이도록 계층 조정
    }

    public void OnDrag(PointerEventData eventData) //드래그 중 호출
    {
        //캔버스 기준으로 위치 계산
        Vector2 delta = eventData.delta / canvas.scaleFactor;
        rectTransform.anchoredPosition += delta;

        for (int i = 0; i < followers.Count; i++) //나머지도 따라오게
        {
            followerRects[i].anchoredPosition = rectTransform.anchoredPosition + followerOffsets[i];
        }
    }

    public void OnEndDrag(PointerEventData eventData) //드롭이 종료되면 호출
    {
        canvasGroup.blocksRaycasts = true;
        if (assignedDropZone == tempLastDropZone) //드롭존이 변경되지 않았다면 드롭 실패로 간주
        {
            ReturnToOriginalPosition();
        }
        foreach (var follower in followers) //따라오던 시민들 처리
        {
            if (follower.assignedDropZone == follower.tempLastDropZone) //드롭존이 변경되지 않았다면 드롭 실패로 간주
            {
                follower.ReturnToOriginalPosition();
            }
            follower.canvasGroup.blocksRaycasts = true;
        }
        //개체에 남아있는 정보 해제
        followers.Clear(); 
        followerOffsets.Clear();
    }

    private List<CitizenDrag> GetFollowingCitizens() //현재 이 개체를 뒤에 따라오는 시민들을 반환(인덱스 기준)
    {
        List<CitizenDrag> result = new List<CitizenDrag>();

        if (assignedDropZone == null) return result;

        List<CitizenDrag> zoneList = assignedDropZone.citizens;
        int selfIndex = zoneList.IndexOf(this); //이 개체의 원래 배정된 드롭존에서의 인덱스 기준

        if (selfIndex == -1 || selfIndex == zoneList.Count - 1) //예외 처리(자신밖에 없을 경우)
            return result;

        for (int i = selfIndex + 1; i < zoneList.Count; i++) //자신보다 인덱스가 큰 시민들 하나씩 추가
        {
            result.Add(zoneList[i]);
        }

        return result;
    }


    public void HandleGroupDrop(DropZone targetZone) //드롭 시도시 이 코드를 호출
    {
        int availableCapacity = targetZone.GetRemainingCapacity(); //

        // 현재 시민 + 추종자 리스트를 하나로 결합
        List<CitizenDrag> group = new List<CitizenDrag> { this };
        group.AddRange(followers);

        // 수용 가능한 시민들
        List<CitizenDrag> accepted = group.Take(availableCapacity).ToList(); //수용가능량만큼 리스트로 반환
        List<CitizenDrag> rejected = group.Skip(availableCapacity).ToList(); //수용가능량을 제외하고 리스트로 반환

        // 등록
        foreach (var citizen in accepted) //수용 가능한 시민들 하나씩 드롭존에 가입
        {
            if (citizen.assignedDropZone != null)
                citizen.assignedDropZone.UnregisterCitizen(citizen); //원래 배정된 드롭존에서 해제

            targetZone.RegisterCitizen(citizen); //새로운 드롭존에 가입
        }

        // 복귀 처리
        foreach (var citizen in rejected) //남은 시민들 하나씩 복귀
        {
            citizen.ReturnToOriginalPosition();
        }
    }

    private void ReturnToOriginalPosition() //드래그 실패시 이 함수가 호출되어 부모개체를 되돌리고 자동으로 정렬됩니다.
    {
        transform.SetParent(assignedDropZone.parentTransform);
    }
}
