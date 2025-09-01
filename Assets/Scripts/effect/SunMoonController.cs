using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SunMoonController : MonoBehaviour
{
    public static SunMoonController instance;

    // ▶ UI 컴포넌트들 연결용 변수 (Inspector에서 할당 필요)
    public Image diskImage;           // 낮/밤 상태를 나타내는 원판 이미지
    public Image backgroundImage;     // 화면 전체의 배경 이미지 (색상 전환용)

    // ▶ 연출에 사용할 시간 조절 변수
    public float transitionDuration = 1.5f;  // 회전 및 배경색 전환에 걸리는 총 시간 (초)

    // ▶ 낮/밤 상태를 기억하는 변수 (처음엔 낮)
    private bool isDay = true;

    // ▶ 낮/밤 배경 색상 정의
    private Color dayColor = new Color(1f, 0.8f, 0.6f, 0.7f);         // 연한 주황색 (낮 배경)
    private Color nightColor = new Color(0f, 0f, 0f, 0.7f);     // 불투명한 검정색 (밤 배경)

    private void Awake()
    {
        // 인스턴스가 아직 없으면 이 오브젝트를 할당
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복된 경우 제거
        }
    }

        //  외부에서 버튼 클릭 시 호출할 함수
    public void ToggleDayNight()
    {
        // 낮/밤 전환 애니메이션 코루틴 실행
        StartCoroutine(AnimateTransition());
    }

    // ▶ 낮/밤 전환을 부드럽게 처리하는 코루틴 함수
    private IEnumerator AnimateTransition()
    {
        // 원판 이미지를 보이게 설정
        diskImage.gameObject.SetActive(true);

        // 원판의 회전 시작값과 목표값 설정 (Z축으로 180도 회전)
        Quaternion startRot = diskImage.transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0f, 0f, 180f);

        // 배경 색상의 시작값과 목표값 설정
        Color startColor = backgroundImage.color;
        Color targetColor = isDay ? nightColor : dayColor;

        float elapsed = 0f;

        // transitionDuration 동안 회전과 색상 전환을 동시에 진행
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration; // 진행 비율 (0.0 ~ 1.0)

            // ① 원판 회전 애니메이션 처리 (부드럽게 회전)
            diskImage.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            // ② 배경 색상 전환 애니메이션 처리 (부드럽게 색 바뀜)
            backgroundImage.color = Color.Lerp(startColor, targetColor, t);

            elapsed += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 마지막 프레임에서 정확한 값으로 고정
        diskImage.transform.rotation = endRot;
        backgroundImage.color = targetColor;

        // 낮/밤 상태를 반대로 변경
        isDay = !isDay;

        // 원판을 잠시 보였다가 다시 숨김 (0.5초 후 비활성화)
        yield return new WaitForSeconds(0.5f);
        diskImage.gameObject.SetActive(false);
    }
}
