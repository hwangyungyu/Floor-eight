using System.Collections;
using UnityEngine;

public class MapMover : MonoBehaviour
{
    public RectTransform popupPanel;
    public float slideDuration = 0.5f;
    public bool isOn = false;

    private Vector2 hiddenPos = new Vector2(-990f, 0); // 화면 왼쪽 밖
    private Vector2 shownPos = new Vector2(0, 0);       // 화면 안쪽

    private Coroutine currentRoutine;

    void Start()
    {
        popupPanel.anchoredPosition = hiddenPos;
    }

    public void MapButton()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (isOn)
            currentRoutine = StartCoroutine(SlideTo(hiddenPos));
        else
            currentRoutine = StartCoroutine(SlideTo(shownPos));

        isOn = !isOn;
    }

    private IEnumerator SlideTo(Vector2 targetPos)
    {
        Vector2 startPos = popupPanel.anchoredPosition;
        float time = 0;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;
            popupPanel.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        popupPanel.anchoredPosition = targetPos;
    }
}
