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
        eventText.text = card.EventText;

        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }

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

    private void CreateChoiceButton(string choiceText, int choiceNumber) //선택지 버튼 생성
    {
        if (string.IsNullOrEmpty(choiceText))
            return;

        GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceButtonContainer);
        Button button = buttonObj.GetComponent<Button>();
        Text buttonText = buttonObj.GetComponentInChildren<Text>();

        // 비활성화 조건 검사
        List<string> effects = null;
        EventCard currentCard = GameManager.Instance.eventCardManager.CurrentEventCard;

        switch (choiceNumber)
        {
            case 1:
                effects = currentCard.ChoiceEffect1;
                break;
            case 2:
                effects = currentCard.ChoiceEffect2;
                break;
            case 3:
                effects = currentCard.ChoiceEffect3;
                break;
        }

        bool isInteractable = true;

        if (effects != null)
        {
            foreach (string effect in effects)
            {
                string[] parts = effect.Split(' ');

                if (parts.Length >= 3 && parts[0] == "ItemDecrease")
                {
                    string resourceName = parts[1];
                    if (!int.TryParse(parts[2], out int amount))
                        continue;

                    if (!GameManager.Instance.executer.CanDecreaseResource(resourceName, amount))
                    {
                        isInteractable = false;
                        break;
                    }
                }
            }
        }

        button.interactable = isInteractable;

        if (buttonText != null)
        {
            buttonText.text = choiceText;
        }

        button.onClick.AddListener(() =>
        {
            GameManager.Instance.ChoiceSelected(choiceNumber);  // GameManager에서 선택 처리
            GameManager.Instance.ShowNextCard();  // 다음 카드 진행
        });
    }
}
