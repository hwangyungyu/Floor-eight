using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// TutorialManager는 0일차에만 활성화되어 튜토리얼을 진행합니다.
// 게임의 다른 부분에 영향을 최소화하면서 튜토리얼 UI와 순서를 관리합니다.
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    // 튜토리얼 진행에 필요한 UI 요소들 (Unity 에디터에서 연결 필요)
    public GameObject tutorialPanel; // 튜토리얼 설명, 버튼 등을 담는 패널
    public GameObject tutorialBlockPanel; // 다음 버튼 외의 터치를 막는 패널
    public Text tutorialText;      // 튜토리얼 설명을 표시할 텍스트
    public Button nextButton;      // 다음 단계로 진행하기 위한 버튼

    private int tutorialStep = 0; // 현재 튜토리얼 진행 단계를 추적
    public int TutorialStep => tutorialStep; // 현재 단계를 외부에서 읽을 수 있도록 프로퍼티 추가

    // 튜토리얼이 활성화 상태인지 확인하는 프로퍼티
    public bool IsTutorialActive => tutorialStep > 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 게임 시작 시 튜토리얼 패널은 비활성화
        if (tutorialPanel != null)
        {            tutorialPanel.SetActive(false);
        }

        // GameManager가 초기화된 후 튜토리얼 시작 여부를 결정해야 하므로
        // 잠시 대기 후 확인
        StartCoroutine(CheckForTutorialStart());
    }

    private IEnumerator CheckForTutorialStart()
    {
        // GameManager가 완전히 초기화될 때까지 잠시 대기
        yield return new WaitForSeconds(0.1f);

        // 0일차일 경우에만 튜토리얼 시작
        if (GameManager.Day == 0)
        {
            StartTutorial();
        }
    }

    // 튜토리얼 시작
    public void StartTutorial()
    {
        Debug.Log("튜토리얼을 시작합니다.");
        tutorialStep = 1;
        ShowStep(tutorialStep);
    }

    // 다음 튜토리얼 단계 진행 (외부 호출용)
    public void GoToNextStep()
    {
        if (!IsTutorialActive) return;

        tutorialStep++;
        ShowStep(tutorialStep);
    }

    // 튜토리얼의 특정 행동이 완료되었을 때 호출될 범용 함수
    // Unity Event에서 파라미터를 직접 넘겨주어 사용합니다. (예: 지도 클릭 시 2, 선택지 클릭 시 3)
    public void OnTutorialAction(int expectedStep)
    {
        if (!IsTutorialActive || tutorialStep != expectedStep) return;

        Debug.Log($"튜토리얼: {expectedStep}단계의 행동이 감지되었습니다. 다음 단계로 진행합니다.");
        GoToNextStep();
    }

    // 특정 단계의 튜토리얼 내용을 표시
    private void ShowStep(int step)
    {
        // 패널 활성화
        tutorialPanel.SetActive(true);
        // 다음 버튼과 방지 패널 기본적으로 비활성화
        nextButton.gameObject.SetActive(false);
        tutorialBlockPanel.SetActive(false);
        // 리스너 초기화 후 다음 단계로 가는 기본 리스너 추가
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(GoToNextStep);


        switch (step)
        {
            case 1:
                // 1단계: 자원 확인
                ResourceManager.Instance.AllResource(3);
                tutorialText.text = "생존 0일차, 튜토리얼을 시작합니다.\n 기본 자원이 지급되었으니 화면 우측의 자원을 확인하세요.";
                // (추후 추가) 자원 UI 강조 로직
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(GoToNextStep);
                nextButton.gameObject.SetActive(true);
                tutorialBlockPanel.SetActive(true);
                break;

            case 2:
                // 2단계: 지도 버튼 확인
                tutorialText.text = "화면 좌측의 지도 버튼을 클릭하면 지도 UI가 펼쳐집니다.\n지도 버튼을 클릭해보세요.";
                // 지도 버튼 강조 로직
                nextButton.gameObject.SetActive(false); // 플레이어가 직접 지도 버튼을 클릭하도록 유도
                tutorialBlockPanel.SetActive(false);
                break;

            case 3:
                // 3단계: 지도 UI 기능 설명 및 배치 완료 유도
                tutorialText.text = "시민을 드래그하여 배치할 수 있습니다.\n시민의 수는 보유한 시민자원의 수와 동일합니다.\n배치가 끝나면 '완료' 버튼을 눌러주세요.";
                // '완료' 버튼 강조 로직. 해당 버튼이 눌리는 것을 감지해야 하므로 다음 버튼을 비활성화 합니다.
                nextButton.gameObject.SetActive(false);
                tutorialBlockPanel.SetActive(false);
                break;

            case 4:
                // 4단계: 카드 설명
                tutorialText.text = "이제 오늘의 사건 카드를 확인해야 합니다.\n카드는 배치한 시민당 하나씩 순차적으로 나타나고, 텍스트와 선택지가 나타납니다.";
                // 카드 강조 로직
                nextButton.gameObject.SetActive(true);
                tutorialBlockPanel.SetActive(true);
                break;

            case 5:
                // 5단계: 선택지 선택 유도
                tutorialText.text = "이제 아래의 선택지 중 하나를 골라야 합니다.\n선택에 따라 자원이 변하거나 다른 사건이 발생할 수 있습니다.";
                // 선택지 버튼들 강조 로직
                nextButton.gameObject.SetActive(false); // 플레이어가 직접 선택지를 누르도록 유도
                tutorialBlockPanel.SetActive(false);
                break;

            case 6:
                // 6단계: 모든 카드 선택 완료 대기
                tutorialText.text = "배치한 시민의 수만큼 사건이 발생합니다.\n모든 사건에 대한 선택을 마쳐주세요.";
                // 모든 카드가 처리될 때까지 대기
                nextButton.gameObject.SetActive(false);
                tutorialBlockPanel.SetActive(false);
                break;

            case 7:
                // 7단계: 튜토리얼 종료 확인
                tutorialText.text = "좋은 선택입니다! 이것으로 튜토리얼을 마칩니다.\n이제 본격적으로 당신의 판단에 따라 진행하게 됩니다.";
                // 자원 UI 다시 강조
                nextButton.GetComponentInChildren<Text>().text = "튜토리얼 종료";
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(EndTutorial);
                nextButton.gameObject.SetActive(true);
                tutorialBlockPanel.SetActive(true);
                break;

            default:
                Debug.LogWarning("정의되지 않은 튜토리얼 단계입니다.");
                EndTutorial();
                break;
        }
    }

    // 튜토리얼 종료
    public void EndTutorial()
    {
        Debug.Log("튜토리얼을 종료합니다.");
        tutorialPanel.SetActive(false);
        tutorialStep = 0;
    }
}