using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public static CardUI Instance;
    [Header("UI 요소")]
    public GameObject cardUI;
    public Text eventText;
    public Transform choiceButtonContainer;
    public GameObject choiceButtonPrefab;
    public CanvasGroup canvasGroup; // UI 투명도 및 상호작용 제어
    public RectTransform cardRoot;  // 카드 전체를 포함하는 루트 Transform (애니메이션용)
    [SerializeField] private List<GameObject> choiceButtons;

    [Header("버튼1 요소")]
    [SerializeField] private List<GameObject> buttonUI1;
    [Header("버튼2 요소")]
    [SerializeField] private List<GameObject> buttonUI2;
    [Header("버튼3 요소")]
    [SerializeField] private List<GameObject> buttonUI3;
    [Header("자원 아이콘 스프라이트")]
    [SerializeField] private Sprite foodSprite;
    [SerializeField] private Sprite medicineSprite;
    [SerializeField] private Sprite mentalSprite;
    [SerializeField] private Sprite madnessSprite;
    [SerializeField] private Sprite utilitySprite;
    [SerializeField] private Sprite defenseSprite;
    [SerializeField] private Sprite populationSprite;


    // 자원명 -> 스프라이트 매핑
    private Dictionary<string, Sprite> resourceIcons;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        resourceIcons = new Dictionary<string, Sprite>
        {
            { "food", foodSprite },
            { "medicine", medicineSprite },
            { "mental", mentalSprite },
            { "madness", madnessSprite },
            { "utilityitem", utilitySprite },
            { "defense", defenseSprite },
            { "population", populationSprite }
        };
    }

    public void ReadyUI() //카드 준비, UI 투명화
    {
        cardUI.transform.localScale = Vector3.one;
        canvasGroup.alpha = 0f;

        if (cardRoot != null)
        {
            cardRoot.anchoredPosition = new Vector2(0, 50); // 약간 위에 고정
        }
    }

    public void SetCard(EventCard card) //카드 표시
    {
        string area = GameManager.Instance.eventCardManager.currentCardArea;
        eventText.text = $"현재 카드 지역: {area ?? "없음"}\n";
        eventText.text += card.EventText;

        CreateChoiceButton(card.ChoiceText1, 1);
        CreateChoiceButton(card.ChoiceText2, 2);
        CreateChoiceButton(card.ChoiceText3, 3);

        // 애니메이션 실행
        StartCoroutine(FadeInCard());
    }

    private IEnumerator FadeInCard() //UI 페이드인
    {
        float duration = 0.4f;
        float elapsed = 0f;

        Vector2 startPos = new Vector2(0, 50);
        Vector2 endPos = Vector2.zero;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            canvasGroup.alpha = t;

            if (cardRoot != null)
            {
                cardRoot.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            }

            yield return null;
        }

        canvasGroup.alpha = 1f;
        if (cardRoot != null)
        {
            cardRoot.anchoredPosition = endPos;
        }
    }

    private void CreateChoiceButton(string choiceText, int choiceNumber) //선택지 버튼을 설정합니다.
    {
        GameObject buttonObj = choiceButtons[choiceNumber - 1];
        Button button = buttonObj.GetComponent<Button>();
        Text buttonText = buttonObj.GetComponentsInChildren<Text>()[0];
        Text additionalMessage = buttonObj.GetComponentsInChildren<Text>()[1];
        additionalMessage.text = "";

        EventCard currentCard = GameManager.Instance.eventCardManager.CurrentEventCard;
        List<string> effects = null;
        List<string> fail_effects = null;
        float probability = 1f;

        switch (choiceNumber)
        {
            case 1:
                effects = currentCard.ChoiceEffect1;
                fail_effects = currentCard.ChoiceEffect1_Fail;
                probability = currentCard.ChoiceProbability1;
                buttonObj.SetActive(currentCard.ChoiceEnabled1);
                break;
            case 2:
                effects = currentCard.ChoiceEffect2;
                fail_effects = currentCard.ChoiceEffect2_Fail;
                probability = currentCard.ChoiceProbability2;
                buttonObj.SetActive(currentCard.ChoiceEnabled2);
                break;
            case 3:
                effects = currentCard.ChoiceEffect3;
                fail_effects = currentCard.ChoiceEffect3_Fail;
                probability = currentCard.ChoiceProbability3;
                buttonObj.SetActive(currentCard.ChoiceEnabled3);
                break;
        }

        if (!buttonObj.activeSelf) return;

        bool isInteractable = IsChoiceAffordable(effects) && IsChoiceAffordable(fail_effects);
        button.interactable = isInteractable;

        var successChanges = GetResourceChanges(effects);
        var failChanges = GetResourceChanges(fail_effects);
        ParseAdditionalMessage(effects, additionalMessage);

        if (buttonText != null) buttonText.text = choiceText;

        List<GameObject> uiList = GetUiListForChoice(choiceNumber);
        if (uiList == null) return;

        foreach (var ui in uiList) ui.SetActive(false);

        var allChanges = new List<(string name, int value, bool success, float pro)>();
        if (probability > 0)
        {
            foreach (var c in successChanges)
            {
                allChanges.Add((c.name, c.value, true, probability));
            }
        }
        if (probability < 1)
        {
            foreach (var c in failChanges)
            {
                allChanges.Add((c.name, c.value, false, 1 - probability));
            }
        }

        int totalChanges = allChanges.Count;

        if (totalChanges == 1)
        {
            PopulateSingleResourceUI(uiList[0], (allChanges[0].name, allChanges[0].value), allChanges[0].success, allChanges[0].pro);
        }
        else if (totalChanges == 2)
        {
            PopulateSingleResourceUI(uiList[0], (allChanges[0].name, allChanges[0].value), allChanges[0].success, allChanges[0].pro);
            PopulateSingleResourceUI(uiList[1], (allChanges[1].name, allChanges[1].value), allChanges[1].success, allChanges[1].pro);
        }
        else if (totalChanges >= 3)
        {
            PopulateSingleResourceUI(uiList[0], (allChanges[0].name, allChanges[0].value), allChanges[0].success, allChanges[0].pro);
            PopulateSingleResourceUI(uiList[2], (allChanges[1].name, allChanges[1].value), allChanges[1].success, allChanges[1].pro);
            PopulateSingleResourceUI(uiList[3], (allChanges[2].name, allChanges[2].value), allChanges[2].success, allChanges[2].pro);
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            GameManager.Instance.ChoiceSelected(choiceNumber);
            GameManager.Instance.ShowNextCard();
        });
    }

    private bool IsChoiceAffordable(List<string> effects)
    {
        if (effects == null) return true;

        string areaID = GameManager.Instance.eventCardManager.currentCardArea;
        Area areaData = null;
        if (!string.IsNullOrEmpty(areaID))
        {
            AreaManager.Instance.areas.TryGetValue(areaID, out areaData);
        }

        foreach (string effect in effects)
        {
            string[] parts = effect.Split(' ');
            if (parts.Length < 3) continue;

            string effectType = parts[0];
            string resourceName = parts[1];
            if (!int.TryParse(parts[2], out int requiredAmount)) continue;

            if (effectType == "ItemDecrease")
            {
                int index = ResourceManager.Instance.GetResourceIndex(resourceName.ToLower());
                if (areaData?.currentPenalty != null && index >= 0 && index < areaData.currentPenalty.Count)
                {
                    requiredAmount += areaData.currentPenalty[index];
                }
                requiredAmount = Mathf.Max(0, requiredAmount);

                if (ResourceManager.Instance.GetResourceByName(resourceName) < requiredAmount)
                {
                    return false;
                }
            }
            else if (effectType == "DecreaseSpecial")
            {
                if (ResourceManager.Instance.GetResourceByName(resourceName) < requiredAmount)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private List<GameObject> GetUiListForChoice(int choiceNumber)
    {
        switch (choiceNumber)
        {
            case 1: return buttonUI1;
            case 2: return buttonUI2;
            case 3: return buttonUI3;
            default: return null;
        }
    }

    private List<(string name, int value)> GetResourceChanges(List<string> effects)
    {
        var resourceChanges = new List<(string name, int value)>();
        if (effects == null) return resourceChanges;

        string area = GameManager.Instance.eventCardManager.currentCardArea;
        Area areaData = null;
        if (!string.IsNullOrEmpty(area))
            AreaManager.Instance.areas.TryGetValue(area, out areaData);

        foreach (string effect in effects)
        {
            string[] parts = effect.Split(' ');
            string effectType = parts[0];

            if (parts.Length >= 3 && (effectType == "ItemDecrease" || effectType == "ItemIncrease"))
            {
                string resourceName = parts[1].ToLower();
                if (!int.TryParse(parts[2], out int baseAmount)) continue;

                int amount = baseAmount;
                int index = ResourceManager.Instance.GetResourceIndex(resourceName);

                if (effectType == "ItemDecrease")
                {
                    if (areaData?.currentPenalty != null && index >= 0 && index < areaData.currentPenalty.Count)
                        amount += areaData.currentPenalty[index];
                    if (resourceName != "madness")
                        resourceChanges.Add((resourceName, -Mathf.Max(0, amount)));
                }
                else // ItemIncrease
                {
                    if (areaData?.currentBonus != null && index >= 0 && index < areaData.currentBonus.Count)
                        amount += areaData.currentBonus[index];
                    if (resourceName != "madness")
                        resourceChanges.Add((resourceName, Mathf.Max(0, amount)));
                }
            }
        }
        return resourceChanges;
    }

    private void ParseAdditionalMessage(List<string> effects, Text additionalMessage)
    {
        if (effects == null || additionalMessage == null) return;

        foreach (string effect in effects)
        {
            string[] parts = effect.Split(' ');
            if (parts.Length >= 2 && parts[0] == "AdditionalMessage")
            {
                additionalMessage.text = parts[1].Replace("_", " ");
                if (parts.Length >= 3 && ColorUtility.TryParseHtmlString(parts[2], out Color newColor))
                    additionalMessage.color = newColor;
                else
                    additionalMessage.color = Color.black;
                return;
            }
        }
    }

    private void PopulateSingleResourceUI(GameObject uiObj, (string name, int value) change, bool success, float pro)
    {
        uiObj.SetActive(true);

        Text uiText = uiObj.GetComponentInChildren<Text>();
        int value = change.value;
        uiText.text = $"{(value >= 0 ? "+" : "")}{value}";
        uiText.color = value >= 0 ? Color.green : Color.red;

        Image uiImage = null;
        Image[] images = uiObj.GetComponentsInChildren<Image>(true);
        foreach (Image img in images)
        {
            img.color = Color.white;
            if (img.gameObject != uiObj)
            {
                uiImage = img;
            }
            else
            {
                if (pro != 1)
                {
                    if (success)
                        img.color = Color.yellow;
                    else
                        img.color = Color.blue;
                }
            }
        }

        if (uiImage != null && resourceIcons.TryGetValue(change.name.ToLower(), out Sprite icon))
        {
            uiImage.sprite = icon;
            uiImage.enabled = true;
        }
        else if (uiImage != null)
        {
            uiImage.enabled = false;
        }
    }
}
