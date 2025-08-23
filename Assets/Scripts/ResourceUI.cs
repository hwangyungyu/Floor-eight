using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    public static ResourceUI Instance;

    [Header("Basic Resources")]
    [SerializeField] private Text amountFood;
    [SerializeField] private Text amountUtil;
    [SerializeField] private Text amountMedi;
    [SerializeField] private Text amountDef;
    [SerializeField] private Text amountMen;
    [SerializeField] private Text amountMad;
    [SerializeField] private Text amountPopulation;

    [Header("Special Resources")]
    [SerializeField] private GameObject specialResourceItemPrefab; // UI 프리팹 (Text 2개 포함: 이름, 수량)
    [SerializeField] private Transform specialResourceContainer; // 프리팹이 생성될 부모 컨테이너

    [Header("Animation Settings")]
    private Color increaseColor = Color.green;
    private Color decreaseColor = Color.red; 
    private Color defaultColor = Color.black;

    [SerializeField] private float animationDuration = 0.5f; // 숫자 변화 시간
    [SerializeField] private float colorResetDelay = 1.0f;   // 색상 원복 시간

    private ResourceManager resourceManager;

    private int prevFood, prevUtil, prevMedi, prevDef, prevMen, prevMad, prevPop;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        if (resourceManager != null) //자원 변화 이벤트 구독 및 초기 설정
        {
            resourceManager.OnResourceChanged += RefreshAmount;

            prevFood = 0;
            prevUtil = 0;
            prevMedi = 0;
            prevDef = 0;
            prevMen = 0;
            prevMad = 0;
            prevPop = 0;

            RefreshAmount(); // 초기 출력
        }
        else
        {
            Debug.LogWarning("ResourceManager.Instance가 비어있습니다!");
        }
    }


    public void RefreshAmount() //자원 UI 업데이트
    {
        AnimateText(amountFood, prevFood, resourceManager.Food, out prevFood);
        AnimateText(amountUtil, prevUtil, resourceManager.UtilityItem, out prevUtil);
        AnimateText(amountMedi, prevMedi, resourceManager.Medicine, out prevMedi);
        AnimateText(amountDef, prevDef, resourceManager.Defense, out prevDef);
        AnimateText(amountMen, prevMen, resourceManager.Mental, out prevMen);
        AnimateText(amountMad, prevMad, resourceManager.Madness, out prevMad);
        AnimateText(amountPopulation, prevPop, resourceManager.Population, out prevPop);
        
        RefreshSpecialResourceUI();
    }

    private void RefreshSpecialResourceUI()
    {
        if (specialResourceItemPrefab == null || specialResourceContainer == null)
        {
            return; // Do nothing if prefab or container is not set
        }

        // Clear existing special resource items
        foreach (Transform child in specialResourceContainer)
        {
            Destroy(child.gameObject);
        }

        if (resourceManager.specialResources == null) return;
        
        // Create new items for each special resource
        foreach (var resource in resourceManager.specialResources)
        {
            GameObject itemGO = Instantiate(specialResourceItemPrefab, specialResourceContainer);
            Text[] texts = itemGO.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = resource.name;    // First Text for name
                texts[1].text = resource.amount.ToString(); // Second Text for amount
            }
            else
            {
                Debug.LogWarning("Special Resource Prefab에는 이름과 수량을 표시할 Text 컴포넌트가 2개 이상 필요합니다.");
            }
        }
    }

    private void AnimateText(Text text, int previous, int current, out int updated) //텍스트 연출 시작
    {
        if (text == null || previous == current)
        {
            updated = previous;
            return;
        }

        Color targetColor = (current > previous) ? increaseColor :
                            (current < previous) ? decreaseColor : defaultColor;

        StopCoroutine($"AnimateValue_{text.name}"); // 중복 방지
        StopCoroutine($"ResetColor_{text.name}");
        
        StartCoroutine(AnimateValue(text, previous, current, animationDuration));
        StartCoroutine(ResetColorAfterDelay(text, targetColor, colorResetDelay));

        updated = current;
    }

    private IEnumerator AnimateValue(Text text, int from, int to, float duration) //연출 코루틴
    {
        float time = 0f;
        text.color = (to > from) ? increaseColor :
                     (to < from) ? decreaseColor : defaultColor;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            int value = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            text.text = value.ToString();
            yield return null;
        }

        text.text = to.ToString();
    }

    private IEnumerator ResetColorAfterDelay(Text text, Color changedColor, float delay) //색상 코루틴
    {
        text.color = changedColor;
        yield return new WaitForSeconds(delay);
        text.color = defaultColor;
    }
}