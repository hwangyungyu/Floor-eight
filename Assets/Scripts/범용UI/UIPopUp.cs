using System.Collections;
using UnityEngine;

public class UIPopUp : MonoBehaviour // 지도 버튼을 눌렀을때 지도 화면이 슬라이드 되는 걸 구현하기 위한 클래스입니다.
{
    public RectTransform popupPanel;
    public float slideDuration = 0.5f;  //슬라이드 시간
    public bool isOn = false;   // 현재 지도 UI 유무

    // 해당 부분을 좌표로 이용하게 되면 다른 곳의 UI에서 망가지는 상황이 발생함으로 수정
    // UI를 활용하는 방안이 더 좋아보임
    public Vector2 hiddenPos = new Vector2(-500f, 0); // 화면 왼쪽 밖

    public Vector2 shownPos = new Vector2(490f, 0);       // 화면 안쪽

    private Coroutine currentRoutine;

    void Start()
    {
        popupPanel.anchoredPosition = hiddenPos;
    }

    public void UIButton() // 지도 버튼에는 이 함수가 연동되어 클릭시 코루틴을 실행시킵니다.
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        if (isOn)
            currentRoutine = StartCoroutine(SlideTo(hiddenPos));
        else
            currentRoutine = StartCoroutine(SlideTo(shownPos));

        isOn = !isOn;
    }

    private IEnumerator SlideTo(Vector2 targetPos) //targetPos로 popupPanel을 이동시키는 코루틴
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
