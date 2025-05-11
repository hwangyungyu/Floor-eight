using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour //
{
    public static GameManager Instance;

    public static int day;

    private static EventCard CurrentEventCard => Instance.eventCardManager.CurrentEventCard;
    //eventCardManager쪽에 있는 값하고 어떤식으로 관리해야할지 모르겠어서 우선 그쪽의 값을 참조하도록 만들었습니다.

    // 이 두개는 테스트를 위해서 임시적으로 연결했습니다.
    private ChoiceExecuter executer;
    private EventCardManager eventCardManager;

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
        day = 1;
        eventCardManager = new EventCardManager();
        executer = new ChoiceExecuter(eventCardManager);
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
        myButton.onClick.Invoke();

        day++;
    }

    private void ChoiceSelected(int choiceNum)
    {
        List<string> effects = null;

        switch (choiceNum)
        {
            case 1:
                effects = CurrentEventCard.ChoiceEffect1;
                break;
            case 2:
                effects = CurrentEventCard.ChoiceEffect2;
                break;
            case 3:
                effects = CurrentEventCard.ChoiceEffect3;
                break;
            default:
                Debug.LogError("잘못된 선택 번호입니다.");
                return;
        }

        foreach (string effect in effects)
        {
            ExecuteEffect(effect);
        }
    }

    private void ExecuteEffect(string effect)
    {
        string[] parts = effect.Split(' ');
        if (parts.Length == 0)
            return;

        switch (parts[0])
        {
            case "ItemIncrease":
                if (parts.Length >= 3)
                    executer.IncreaseResource(parts[1], int.Parse(parts[2]));
                break;

            case "ItemDecrease":
                if (parts.Length >= 3)
                    executer.DecreaseResource(parts[1], int.Parse(parts[2]));
                break;

            case "AddNextEventCard":
                if (parts.Length >= 2)
                    executer.AddNextEventCard(parts[1]);
                break;

            case "AddEventCardToDeck":
                if (parts.Length >= 3)
                    executer.AddEventCardToDeck(parts[1], int.Parse(parts[2]));
                break;

            default:
                Debug.LogWarning("알 수 없는 효과: " + effect);
                break;
        }
    }
}
