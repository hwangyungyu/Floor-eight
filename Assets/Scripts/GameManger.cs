using UnityEngine;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    public static GameManger Instance;

    public static int day;

    // 시간 보내고 초기화해주는 버튼
    public Button myButton;

    void Start()
    {
        day = 1;
    }

    private void ChoiceSelected(int card) { 
    
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
}
