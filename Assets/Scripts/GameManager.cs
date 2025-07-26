using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections; // 코루틴 사용을 위해 추가

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int Day => Instance.eventCardManager.currentCardDay;
    private static EventCard CurrentEventCard => Instance.eventCardManager.CurrentEventCard;

    public ChoiceExecuter executer;
    public EventCardManager eventCardManager;

    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    private Action onSceneLoadedAction;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("GameManager가 여러개?");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UIUpdate() //UI 요소를 변수에 맞게 갱신합니다.
    {
        DropZoneManager.Instance.AdjustBundleZonesToMatchPopulation();
        AreaManager.Instance.UpdateTotal();
    }

    // 새 게임 시작 버튼에 연결할 함수
    public void StartNewGame()
    {
        onSceneLoadedAction = () => {
            InitializeNewGame();
        };
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene("TestScene");
    }

    // 이어하기 버튼에 연결할 함수
    public void LoadGameAndChangeScene()
    {
        onSceneLoadedAction = () => {
            InitializeNewGame();
            SaveLoadManager.Instance.LoadGame();
        };
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene("TestScene");
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestScene")
        {
            // 씬 로드 후 바로 실행하는 대신 코루틴을 시작
            StartCoroutine(WaitAndInitialize());
            SceneManager.sceneLoaded -= OnGameSceneLoaded;
        }
    }

    // 모든 Start() 함수가 실행된 후 초기화를 진행하기 위한 코루틴
    private IEnumerator WaitAndInitialize()
    {
        // 한 프레임 대기하여 모든 오브젝트의 Start()가 실행되도록 보장
        yield return null; 

        // 예약된 액션(초기화 또는 로드) 실행
        onSceneLoadedAction?.Invoke();
        onSceneLoadedAction = null;
    }

    // 새 게임을 위한 초기화 함수 (이어하기 시에도 먼저 호출됨)
    public void InitializeNewGame()
    {
        Debug.Log("게임 시스템 및 데이터 초기화를 시작합니다.");
        eventCardManager = new EventCardManager();
        eventCardManager.InitializeDeck(20);
        eventCardManager.LoadAllEventCards();
        executer = new ChoiceExecuter(eventCardManager);
        
        if (AreaManager.Instance != null)
        {
            AreaManager.Instance.OnPopulationPlacementComplete += StartEventCardSequence; //시민 배치 완료 수신용 이벤트
        }

        eventCardManager.SetDay(1);
        ResourceManager.Instance.InitializeResources();
        GameManager.Instance.UIUpdate();
    }

    public void ChoiceSelected(int choiceNum)
    {
        List<string> effects = null;
        switch (choiceNum)
        {
            case 1: effects = CurrentEventCard.ChoiceEffect1; break;
            case 2: effects = CurrentEventCard.ChoiceEffect2; break;
            case 3: effects = CurrentEventCard.ChoiceEffect3; break;
            default: Debug.LogError("잘못된 선택 번호입니다."); return;
        }
        foreach (string effect in effects)
        {
            executer.ExecuteEffect(effect);
        }
    }

    public void NextDay()
    {
        GameManager.Instance.UIUpdate();
        eventCardManager.ChangeDay(1);
        AreaManager.Instance.EndDaySchedule();
        
        SaveLoadManager.Instance.SaveGame();
    }
    
    public void ShowNextCard()
    {
        bool success = eventCardManager.DrawEventCard();
        EventCard card = CurrentEventCard;
        CardUI.Instance.cardUI.transform.localScale = Vector3.zero;
        if (!success)
        {
            Debug.Log("더 이상 표시할 카드가 없습니다.");
            NextDay();
            return;
        }

        if (CardUI.Instance != null)
        {
            CardUI.Instance.ReadyUI();
        }

        

        if (CardUI.Instance != null)
        {
            CardUI.Instance.SetCard(card);
        }
    }

    private void StartEventCardSequence() //시민 배치 완료 수신시 작동
    {
        ShowNextCard(); //다음 카드 송출, 정상작동시 첫번째 카드를 보여줘야 함
    }
}