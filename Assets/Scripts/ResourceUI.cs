using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private Text amountFood;
    [SerializeField] private Text amountUtil;
    [SerializeField] private Text amountMedi;
    [SerializeField] private Text amountDef;
    [SerializeField] private Text amountMen;
    [SerializeField] private Text amountMad;
    [SerializeField] private Text amountPopulation;

    private Color increaseColor = Color.green; //1
    private Color decreaseColor = Color.red; 
    private Color defaultColor = Color.black;

    [SerializeField] private float animationDuration = 0.5f; // 숫자 변화 시간
    [SerializeField] private float colorResetDelay = 1.0f;   // 색상 원복 시간

    private ResourceManager resourceManager;

    private int prevFood, prevUtil, prevMedi, prevDef, prevMen, prevMad, prevPop;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        if (resourceManager != null) //자원 변화 이벤트 구독 및 초기 설정
        {
            resourceManager.OnResourceChanged += RefreshAmount;

            prevFood = resourceManager.Food;
            prevUtil = resourceManager.UtilityItem;
            prevMedi = resourceManager.Medicine;
            prevDef = resourceManager.Defense;
            prevMen = resourceManager.Mental;
            prevMad = resourceManager.Madness;
            prevPop = resourceManager.Population;

            RefreshAmount(); // 초기 출력
        }
        else
        {
            Debug.LogWarning("ResourceManager.Instance가 비어있습니다!");
        }
    }

    private void OnDisable() //자원 변화 이벤트 구독 해제
    {
        if (resourceManager != null)
            resourceManager.OnResourceChanged -= RefreshAmount;
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