using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CitizenBundle : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public GameObject citizenPrefab;
    public Transform canvasTransform;
    public Text quantityText;
    public int citizenCount;

    private GameObject draggingCitizen;     // 현재 드래그 중인 시민 오브젝트, 드래그 관련 변수입니다

    void Start()
    {
        citizenCount = ResourceManager.Instance.Population;
        UpdateQuantityText();


        if (DropZoneManager.Instance != null) // 테스트 용 이벤트를 구독합니다.
        {
            DropZoneManager.Instance.OnTestReset += TestReset;
        }
    }

    public void TestReset()  //이벤트를 구독해서 작동하는 테스트용 리셋 함수 입니다. 다음 일차로 넘어가면서 시민이 배치되어 있는 걸 초기화 하는걸 임의적으로 구현한 코드입니다.
    {

        citizenCount = ResourceManager.Instance.Population;

        UpdateQuantityText();
    }

    public void OnBeginDrag(PointerEventData eventData) // 드래그 시작시 작동합니다. 번들에서 시민을 드래그하는걸 만들고 싶었습니다. 실행시 시민을 복제하고 설정해줍니다.
    {
        if (citizenCount <= 0)
        {
            eventData.pointerDrag = null;
            return;
        }

        

        draggingCitizen = Instantiate(citizenPrefab, canvasTransform);
        Debug.Log("시민 생성됨: " + draggingCitizen.name);
        draggingCitizen.transform.position = eventData.position;



        eventData.pointerDrag = draggingCitizen;

        var cg = draggingCitizen.GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        //draggingCitizen.GetComponent<CitizenDrag>().bundle = this;

        draggingCitizen.GetComponent<CitizenDrag>().AssignButtonEvent();

        citizenCount--;
        UpdateQuantityText();
    }
    public void ReturnCitizen() // 번들에 시민 숫자를 1 증가시킵니다. 
    {
        citizenCount++;
        UpdateQuantityText();
    }

    void UpdateQuantityText() // 지도UI에서 하단부에 위치한 현재 번들이 가지고 있는 시민의 수량을 업데이트합니다.
    {
        quantityText.text = $"X {citizenCount}";
    }

    public void OnDrag(PointerEventData eventData) 
    {
        //아무 작동도 하지 않지만 이 코드가 없으면 IDragHandler가 정상 작동하지 않고
        //IDragHandler가 없으면 OnBeginDrag가 정상호출 되지 않습니다.
    }
}