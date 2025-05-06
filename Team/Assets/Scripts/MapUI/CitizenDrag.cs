using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CitizenDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CitizenBundle bundle;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public Button returnButton; // 배치 취소용 버튼

    public DropZone AssignedDropZone; //이 시민이 배치되어 있는 드롭존

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        returnButton = GetComponent<Button>();
    }

    private void OnEnable() //테스트용 이벤트 구독
    {
        if (DropZoneManager.Instance != null)
        {
            DropZoneManager.Instance.OnTestReset += ResetToBundle;
        }
    }

    private void OnDisable() //테스트용 이벤트 구독해제
    {
        if (DropZoneManager.Instance != null)
        {
            DropZoneManager.Instance.OnTestReset -= ResetToBundle;
        }
    }

    public void AssignButtonEvent() //버튼에 번들 귀환용 코드 추가하기, 시민 생성단계에서 호출됨
    {
        returnButton.onClick.AddListener(ReturnToBundle);
    }

    public void ReturnToBundle() // 시민을 클릭할 시 배치를 취소하고 번들로 귀환, 이 코드에서 번들의 수량을 복구시킴
    {
        if (bundle != null)
        {
            bundle.ReturnCitizen(); // 수량 복구
        }

        if (AssignedDropZone != null)
        {
            AssignedDropZone.DecreaseCount(); // 드롭존 수 감소
        }

        Destroy(gameObject); // 자신 제거
    }

    public void ResetToBundle() //테스트 리셋을 위한 코드. 테스트 코드 작동시 번들에서 직접 인구수로 수량을 초기화하기 때문에 수량 복구 코드를 제거함
    {
        if (AssignedDropZone != null)
        {
            AssignedDropZone.DecreaseCount(); // 드롭존 수 감소
        }

        Destroy(gameObject); // 자신 제거
    }

    public void OnBeginDrag(PointerEventData eventData) //드래그 시작시, 기본 설정
    {
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) //드래그하기
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) //드래그 종료
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
}