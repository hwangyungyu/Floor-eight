using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    // 초기화
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    public void SunMoonChange()
    {
        // 낮과 밤을 바꾸는 함수 실행
        SunMoonController.instance.ToggleDayNight();
    }
}
