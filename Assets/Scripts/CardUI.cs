using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("UI 요소")]
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
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

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
        List<string> effects = null;
        EventCard currentCard = GameManager.Instance.eventCardManager.CurrentEventCard;
        string area = GameManager.Instance.eventCardManager.currentCardArea;
        Area areaData = null;

        if (!string.IsNullOrEmpty(area))
            AreaManager.Instance.areas.TryGetValue(area, out areaData);

        switch (choiceNumber)
        {
            case 1:
                effects = currentCard.ChoiceEffect1;
                buttonObj.SetActive(currentCard.ChoiceEnabled1);
                break;
            case 2:
                effects = currentCard.ChoiceEffect2;
                buttonObj.SetActive(currentCard.ChoiceEnabled2);
                break;
            case 3:
                effects = currentCard.ChoiceEffect3;
                buttonObj.SetActive(currentCard.ChoiceEnabled3);
                break;
        }

        if (!buttonObj.activeSelf) return;

        bool isInteractable = true;
        List<(string name, int value)> resourceChanges = new List<(string, int)>();

        if (effects != null)
        {
            foreach (string effect in effects)
            {
                string[] parts = effect.Split(' ');

                if (parts.Length >= 3 && (parts[0] == "ItemDecrease" || parts[0] == "ItemIncrease"))
                {
                    string resourceName = parts[1].ToLower();
                    if (!int.TryParse(parts[2], out int baseAmount))
                        continue;

                    int amount = baseAmount;
                    int index = ResourceManager.Instance.GetResourceIndex(resourceName);

                    if (parts[0] == "ItemDecrease")
                    {
                        if (areaData?.currentPenalty != null && index >= 0 && index < areaData.currentPenalty.Count)
                            amount += areaData.currentPenalty[index];
                        amount = Mathf.Max(0, amount);

                        if (!GameManager.Instance.executer.CanDecreaseResource(resourceName, amount))
                            isInteractable = false;
                        resourceChanges.Add((resourceName, -amount));
                    }
                    else if (parts[0] == "ItemIncrease")
                    {
                        if (areaData?.currentBonus != null && index >= 0 && index < areaData.currentBonus.Count)
                            amount += areaData.currentBonus[index];
                        amount = Mathf.Max(0, amount);
                        resourceChanges.Add((resourceName, amount));
                    }
                }
                else if (parts.Length >= 2 && parts[0] == "AdditionalMessage")
                {
                    
                    if (additionalMessage != null)
                    {
                        additionalMessage.text = parts[1].Replace("_", " ");

                        if (parts.Length >= 3 && ColorUtility.TryParseHtmlString(parts[2], out Color newColor))
                        {
                            additionalMessage.color = newColor;
                        }
                        else
                        {
                            additionalMessage.color = Color.black;
                        }
                    }
                }
            }
        }

        button.interactable = isInteractable;
        if (buttonText != null) buttonText.text = choiceText;

        List<GameObject> uiList = choiceNumber switch
        {
            1 => buttonUI1,
            2 => buttonUI2,
            3 => buttonUI3,
            _ => null
        };

        for (int i = 0; i < uiList.Count; i++)
            uiList[i].SetActive(false); // 초기화

        for (int i = 0; i < resourceChanges.Count && i < 4; i++)
        {
            int uiIndex = (resourceChanges.Count == 3 && i == 1) ? 2 : i;
            if (resourceChanges.Count == 3 && i == 2) uiIndex = 3;

            GameObject uiObj = uiList[uiIndex];
            uiObj.SetActive(true);

            // 텍스트 처리
            Text uiText = uiObj.GetComponentInChildren<Text>();
            int value = resourceChanges[i].value;
            uiText.text = $"{(value > 0 ? "+" : "-")}{Mathf.Abs(value)}";

            // 색상 처리
            uiText.color = value >= 0 ? Color.green : Color.red;

            // 이미지 처리
            Image uiImage = null;
            Image[] images = uiObj.GetComponentsInChildren<Image>(true); // 비활성화도 포함
            foreach (Image img in images)
            {
                if (img.gameObject != uiObj) // 자기 자신 제외
                {
                    uiImage = img;
                    break;
                }
            }
            if (uiImage != null && resourceIcons.TryGetValue(resourceChanges[i].name.ToLower(), out Sprite icon))
            {
                uiImage.sprite = icon;
                uiImage.enabled = true;
            }
            else if (uiImage != null)
            {
                uiImage.enabled = false; // 아이콘 없으면 비활성화
            }
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            GameManager.Instance.ChoiceSelected(choiceNumber);
            GameManager.Instance.ShowNextCard();
        });
    }
}
