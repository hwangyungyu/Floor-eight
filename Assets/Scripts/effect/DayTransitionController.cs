using UnityEngine;
using System.Collections;

// Legacy Text를 쓸 경우 주석 해제
using UnityEngine.UI;

public class DayTransitionController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform currentRt;   // 현재 숫자
    public RectTransform nextRt;      // 다음 숫자
    public CanvasGroup currentCg;
    public CanvasGroup nextCg;

    [Header("Text (둘 중 하나만 연결해도 OK)")]

    public Text currentTxt;
    public Text nextTxt;

    [Header("Main Slide Settings")]
    [Tooltip("중앙 기준 위/아래 이동 거리(px)")]
    public float distance = 180f;
    [Tooltip("메인 전환 시간(초)")]
    public float duration = 0.6f;
    [Tooltip("메인 슬라이드 이징 곡선(0→1)")]
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Snapping")]
    [Tooltip("메인 전환이 끝나면 정확히 중앙/아래로 스냅할지 여부")]
    public bool snapToExactCenter = true;

    [Header("Position Overshoot (아래로 더 갔다가 중앙으로 복귀)")]
    public bool doPositionOvershoot = true;
    [Tooltip("중앙 도착 후 아래로 더 내려갈 거리(px)")]
    public float overshootDistance = 24f;
    [Tooltip("오버슈트(↓) 후 복귀(↑)까지 총 시간(초)")]
    public float overshootDuration = 0.18f;
    [Tooltip("0→1 구간에서 0→1.2→1 같이 살짝 넘어갔다 돌아오는 커브")]
    public AnimationCurve overshootCurve = new AnimationCurve(
        new Keyframe(0.00f, 0.00f),
        new Keyframe(0.55f, 1.20f),
        new Keyframe(1.00f, 1.00f)
    );

    [Header("Scale Pop (툭 튀는 팝)")]
    public bool doPop = false;           // 필요시 켜기
    public float popDuration = 0.18f;
    public float popScale = 1.08f;
    public AnimationCurve popCurve = new AnimationCurve(
        new Keyframe(0.00f, 1.00f),
        new Keyframe(0.60f, 1.08f),
        new Keyframe(1.00f, 1.00f)
    );

    private Vector2 centerPos;

    private void Awake()
    {
        // Anchor를 중앙(0.5,0.5)로 맞춘 경우 기준점은 (0,0)
        centerPos = Vector2.zero;
    }

    public void SetLabels(int fromDay, int toDay)
    {
        if (currentTxt) currentTxt.text = $"{fromDay}일차";
        if (nextTxt)    nextTxt.text    = $"{toDay}일차";
    }

    /// <summary>
    /// 1) 현재(중앙) ↓ 페이드아웃, 2) 다음(위) ↓ 중앙으로 페이드인,
    /// 3) (옵션) 중앙 도착 후 아래로 살짝 더 ↓ 갔다가 ↑ 중앙 복귀,
    /// 4) (옵션) 스케일 팝
    /// </summary>
    public IEnumerator PlayAndWait(int fromDay, int toDay)
    {
        SetLabels(fromDay, toDay);

        // 시작 상태 세팅
        currentRt.anchoredPosition = centerPos;
        currentCg.alpha = 1f;

        nextRt.anchoredPosition = centerPos + new Vector2(0f, distance);
        nextCg.alpha = 0f;

        float t = 0f;
        while (t < duration)
        {
            // 테스트/연출용: 일시정지 영향 받지 않도록 unscaled
            t += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(t / duration);
            float k = ease.Evaluate(u);

            // 현재: 중앙 → 아래
            currentRt.anchoredPosition = Vector2.Lerp(centerPos, centerPos - new Vector2(0f, distance), k);
            currentCg.alpha = 1f - k;

            // 다음: 위 → 중앙
            nextRt.anchoredPosition = Vector2.Lerp(centerPos + new Vector2(0f, distance), centerPos, k);
            nextCg.alpha = k;

            yield return null;
        }

        if (snapToExactCenter)
        {
            currentRt.anchoredPosition = centerPos - new Vector2(0f, distance);
            currentCg.alpha = 0f;

            nextRt.anchoredPosition = centerPos;
            nextCg.alpha = 1f;
        }

        // 위치 오버슈트(아래로 살짝 더 내려갔다가 중앙으로 복귀)
        if (doPositionOvershoot)
            yield return StartCoroutine(PositionOvershoot(nextRt));
        /*
        // 스케일 팝(옵션)
        if (doPop)
            yield return StartCoroutine(Pop(nextRt));
        */
    }

    private IEnumerator PositionOvershoot(RectTransform rt)
    {
        Vector2 basePos = rt.anchoredPosition; // 보통 (0,0)
        Vector2 downPos = basePos - new Vector2(0f, overshootDistance);

        float t = 0f;
        while (t < overshootDuration)
        {
            t += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(t / overshootDuration);

            float k = overshootCurve.Evaluate(u);       // 0→1.2→1 형태
            Vector2 pos = Vector2.LerpUnclamped(basePos, downPos, k);
            rt.anchoredPosition = pos;

            yield return null;
        }

        rt.anchoredPosition = basePos; // 정확히 중앙 스냅
    }

    private IEnumerator Pop(RectTransform rt)
    {
        rt.localScale = Vector3.one;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(t / popDuration);

            float s = Mathf.LerpUnclamped(1f, popScale, popCurve.Evaluate(u)); // 1→popScale→1
            rt.localScale = new Vector3(s, s, 1f);

            yield return null;
        }

        rt.localScale = Vector3.one;
    }
}
