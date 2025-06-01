using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour //임시적으로 작성한걸 합쳤습니다.
{
    public static GameManager Instance;

    public static int Day => Instance.eventCardManager.currentCardDay;

    private static EventCard CurrentEventCard => Instance.eventCardManager.CurrentEventCard;
    //eventCardManager쪽에 있는 값하고 어떤식으로 관리해야할지 모르겠어서 우선 그쪽의 값을 참조하도록 만들었습니다.

    // 이 두개는 테스트를 위해서 임시적으로 연결했습니다.
    public ChoiceExecuter executer;
    public EventCardManager eventCardManager;
    public CardUI cardUI;

    // 시간 보내고 초기화해주는 버튼
    public Button myButton;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("GameManager가 여러개?");
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        eventCardManager = new EventCardManager();
        eventCardManager.InitializeDeck(20); //테스트용 덱 생성
        eventCardManager.LoadAllEventCards(); //이벤트 카드 전체 로드
        executer = new ChoiceExecuter(eventCardManager);
        AreaManager.Instance.OnPopulationPlacementComplete += StartEventCardSequence; //테스트용

        
        eventCardManager.SetDay(1);
    }

    private void AreaConfirmed(int area) { 
        
    }

    private void AreaCitizenChanged(int a, int b) { 
    
    }

    private void CreateNewEventDeck() { 
    
    }

    public void NextCard()
    {

    }

    public void NextDay() {
        // 귀찮아서 바로 반영하도록 test해놓음
        DropZoneManager.Instance.TestReset();

        eventCardManager.ChangeDay(1); //eventCardManager의 것을 수정하게 바꿨습니다.
    }
    public void ShowNextCard() //테스트 용이라 코드 구조가 끔찍하지만 졸려서 수정을 못하겠습니다.
    {
        bool success = eventCardManager.DrawEventCard();  // 다음 카드 뽑기
        EventCard card = eventCardManager.CurrentEventCard;


        cardUI.ReadyUI();

        if (success == false)
        {
            Debug.Log("더 이상 표시할 카드가 없습니다.");
            NextDay();
            return;
        }

        cardUI.SetCard(card);  // 카드 데이터를 UI에 전달
    }

    private void StartEventCardSequence()
    {
        ShowNextCard();
    }
}
