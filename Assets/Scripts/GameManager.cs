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

    public Dictionary<string, bool> flags = new Dictionary<string, bool>(); //플래그
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
    public void ChoiceSelected(int choiceNum) //선택지 실행
    {
        List<string> effects = null;

        switch (choiceNum)
        {
            case 1:
                effects = eventCardManager.CurrentEventCard.ChoiceEffect1;
                break;
            case 2:
                effects = eventCardManager.CurrentEventCard.ChoiceEffect2;
                break;
            case 3:
                effects = eventCardManager.CurrentEventCard.ChoiceEffect3;
                break;
            default:
                Debug.LogError("잘못된 선택 번호입니다.");
                return;
        }

        foreach (string effect in effects)
        {
            executer.ExecuteEffect(effect);
        }
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

        // --- 자동 저장 ---
        SaveLoadManager.Instance.SaveGame();
    }
    public void ShowNextCard() //다음 카드 보여주기
    {
        bool success = eventCardManager.DrawEventCard();  // 다음 카드 뽑기
        EventCard card = eventCardManager.CurrentEventCard;


        cardUI.ReadyUI();

        if (success == false)
        {
            Debug.Log("더 이상 표시할 카드가 없습니다.");
            NextDay(); //여기서 다음날로 넘어가는 중!
            return;
        }

        cardUI.SetCard(card);  // 카드 데이터를 UI에 전달
    }

    private void StartEventCardSequence()
    {
        ShowNextCard();
    }
}
