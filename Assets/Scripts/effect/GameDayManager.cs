using UnityEngine;
using System.Collections;

public class GameDayManager : MonoBehaviour
{
    public DayTransitionController transition; // 인스펙터에 드래그
    public int day = 1;                        // 시작 일차

    private bool busy;

    // 버튼 OnClick에 연결할 함수
    public void Next()
    {
        if (busy || transition == null) return;
        StartCoroutine(Co_Run());
    }

    IEnumerator Co_Run()
    {
        busy = true;

        int from = day;
        int to = day + 1;

        // 원하면 테스트 중 입력 멈추고 싶을 때:
        float prev = Time.timeScale;
        Time.timeScale = 0f;
        yield return StartCoroutine(transition.PlayAndWait(from, to));
        Time.timeScale = prev;

        day = to; // 카운터 증가만(게임 로직 X)
        busy = false;
    }
}
