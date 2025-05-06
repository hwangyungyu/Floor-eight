using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CitizenBundle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject citizenPrefab;
    public Transform canvasTransform;
    public Text quantityText;
    public int citizenCount;

    private GameObject draggingCitizen;     // 현재 드래그 중인 시민 오브젝트

    void Start()
    {
        citizenCount = ResourceManager.Instance.Population;
        UpdateQuantityText();


        if (DropZoneManager.Instance != null) //임의적으로 우선 이벤트 구독 절차를 Start로 옮겼습니다.
        {
            DropZoneManager.Instance.OnTestReset += TestReset;
        }
    }

    private void OnDisable()
    {
        if (DropZoneManager.Instance != null)
        {
            DropZoneManager.Instance.OnTestReset -= TestReset;
        }
    }
    public void TestReset()
    {
        Debug.Log("TestReset() called");

        citizenCount = ResourceManager.Instance.Population;
        Debug.Log($"Updated citizenCount: {citizenCount}");

        UpdateQuantityText();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (citizenCount <= 0)
        {
            eventData.pointerDrag = null;
            return;
        }

        draggingCitizen = Instantiate(citizenPrefab, canvasTransform);
        draggingCitizen.transform.position = eventData.position;



        eventData.pointerDrag = draggingCitizen;

        var cg = draggingCitizen.GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.alpha = 0.7f;

        draggingCitizen.GetComponent<CitizenDrag>().bundle = this;

        draggingCitizen.GetComponent<CitizenDrag>().AssignButtonEvent();

        citizenCount--;
        UpdateQuantityText();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingCitizen != null)
        {
            draggingCitizen.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingCitizen != null)
        {
            // 드롭 영역이 아니라면 시민 삭제하고 수량 복구
            if (!eventData.pointerCurrentRaycast.isValid)
            {
                Destroy(draggingCitizen);
                ReturnCitizen();
            }
            else
            {
                draggingCitizen.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }

            draggingCitizen = null;
        }
    }

    public void ReturnCitizen()
    {
        citizenCount++;
        UpdateQuantityText();
    }

    void UpdateQuantityText()
    {
        quantityText.text = $"X {citizenCount}";
    }
}