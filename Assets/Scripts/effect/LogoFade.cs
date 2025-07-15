using UnityEngine;
using UnityEngine.UI;

public class LogoFade : MonoBehaviour
{
    public Image logoImage;          // 로고 UI Image
    public GameObject startUIGroup;  // Touch to Start 등 나중에 보일 UI 그룹
    public float holdTime = 2f;
    public float fadeDuration = 1.5f;

    void Start()
    {
        startUIGroup.SetActive(false);  // 시작 UI는 처음에 꺼두기
        StartCoroutine(FadeOutLogo());
    }

    System.Collections.IEnumerator FadeOutLogo()
    {
        SetAlpha(1f);
        yield return new WaitForSeconds(holdTime);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(0f);
        logoImage.gameObject.SetActive(false);   // 로고 꺼버림
        startUIGroup.SetActive(true);            // Touch to Start UI 등장
    }

    void SetAlpha(float alpha)
    {
        Color c = logoImage.color;
        c.a = alpha;
        logoImage.color = c;
    }
}
