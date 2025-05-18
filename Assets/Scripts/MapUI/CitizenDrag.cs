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
    private DropZone tempLastDropZone;
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

    private void OnEnable()
    {
        if (DropZoneManager.Instance != null)
            DropZoneManager.Instance.OnTestReset += ResetToBundle;
    }

    private void OnDisable()
    {
        if (DropZoneManager.Instance != null)
            DropZoneManager.Instance.OnTestReset -= ResetToBundle;
    }

    public void AssignButtonEvent()
    {
        returnButton.onClick.AddListener(ReturnToBundle);
    }

    public void ReturnToBundle()
    {
        
        if (bundle != null)
            bundle.RegisterCitizen(this);

        if (assignedDropZone != null)
            assignedDropZone.UnregisterCitizen(this);

    }

    public void ResetToBundle()
    {
        return;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

        tempLastDropZone = assignedDropZone;

        // 추종자 확보 및 상대 위치 계산
        followers = GetFollowingCitizens();
        followerOffsets.Clear();
        followerRects.Clear();

        transform.SetParent(canvas.transform);

        
        for (int i = 0; i < followers.Count; i++)
        {
            var follower = followers[i];
            follower.tempLastDropZone = follower.assignedDropZone;
            // 겹쳐진 느낌을 주기 위한 오프셋 설정
            Vector2 offset = new Vector2(20+20f * i, 0);
            followerOffsets.Add(offset);

            follower.canvasGroup.blocksRaycasts = false;
            follower.transform.SetParent(canvas.transform);
            // 드래그 중인 시민의 바로 아래부터 순차적으로 배치
            follower.transform.SetSiblingIndex(transform.GetSiblingIndex() + -i + 1);

            followerRects.Add(follower.GetComponent<RectTransform>());
        }
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.delta / canvas.scaleFactor;
        rectTransform.anchoredPosition += delta;

        for (int i = 0; i < followers.Count; i++)
        {
            followerRects[i].anchoredPosition = rectTransform.anchoredPosition + followerOffsets[i];
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        if (assignedDropZone == tempLastDropZone)
        {
            ReturnToOriginalPosition();
        }
        foreach (var follower in followers)
        {
            if (follower.assignedDropZone == follower.tempLastDropZone)
            {
                follower.ReturnToOriginalPosition();
            }
            follower.canvasGroup.blocksRaycasts = true;
        }

        followers.Clear();
        followerOffsets.Clear();
    }

    private List<CitizenDrag> GetFollowingCitizens()
    {
        List<CitizenDrag> result = new List<CitizenDrag>();

        if (assignedDropZone == null) return result;

        List<CitizenDrag> zoneList = assignedDropZone.citizens;
        int selfIndex = zoneList.IndexOf(this);

        if (selfIndex == -1 || selfIndex == zoneList.Count - 1)
            return result;

        for (int i = selfIndex + 1; i < zoneList.Count; i++)
        {
            result.Add(zoneList[i]);
        }

        return result;
    }


    public void HandleGroupDrop(DropZone targetZone)
    {
        int availableCapacity = targetZone.GetRemainingCapacity();

        // 현재 시민 + 추종자 리스트를 하나로 결합
        List<CitizenDrag> group = new List<CitizenDrag> { this };
        group.AddRange(followers);

        // 수용 가능한 시민들
        List<CitizenDrag> accepted = group.Take(availableCapacity).ToList();
        List<CitizenDrag> rejected = group.Skip(availableCapacity).ToList();

        // 등록
        foreach (var citizen in accepted)
        {
            if (citizen.assignedDropZone != null)
                citizen.assignedDropZone.UnregisterCitizen(citizen);

            targetZone.RegisterCitizen(citizen);
        }

        // 복귀 처리
        foreach (var citizen in rejected)
        {
            citizen.ReturnToOriginalPosition();
        }
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(assignedDropZone.parentTransform);
    }
}
