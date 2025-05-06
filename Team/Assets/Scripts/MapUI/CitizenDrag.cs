using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CitizenDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CitizenBundle bundle;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public Button returnButton; // UI Button을 시민에 붙여둔다

    public DropZone AssignedDropZone;

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
        {
            DropZoneManager.Instance.OnTestReset += ResetToBundle;
        }
    }

    private void OnDisable()
    {
        if (DropZoneManager.Instance != null)
        {
            DropZoneManager.Instance.OnTestReset -= ResetToBundle;
        }
    }

    public void AssignButtonEvent()
    {
        returnButton.onClick.AddListener(ReturnToBundle);
    }

    public void ReturnToBundle()
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

    public void ResetToBundle()
    {
        if (AssignedDropZone != null)
        {
            AssignedDropZone.DecreaseCount(); // 드롭존 수 감소
        }

        Destroy(gameObject); // 자신 제거
    }

    public void MarkAsPlaced()
    {
        canvasGroup.blocksRaycasts = false;
        returnButton.gameObject.SetActive(true); // 되돌리기 버튼 활성화
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        if (transform.parent == canvas.transform)
        {
            // 드롭 실패
            bundle.ReturnCitizen();
            Destroy(gameObject);
        }
    }
}