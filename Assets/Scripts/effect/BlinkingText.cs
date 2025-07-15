using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    public Text uiText;              // UnityEngine.UI.Text (기본 텍스트)
    public float blinkSpeed = 1f;    // 깜빡이는 속도 (1 = 1초 주기)

    private void Start()
    {
        StartCoroutine(Blink());
    }

    System.Collections.IEnumerator Blink()
    {
        while (true)
        {
            // 점점 사라짐
            for (float a = 1f; a >= 0f; a -= Time.deltaTime * blinkSpeed)
            {
                SetAlpha(a);
                yield return null;
            }

            // 점점 다시 나타남
            for (float a = 0f; a <= 1f; a += Time.deltaTime * blinkSpeed)
            {
                SetAlpha(a);
                yield return null;
            }
        }
    }

    void SetAlpha(float alpha)
    {
        if (uiText != null)
        {
            Color c = uiText.color;
            c.a = alpha;
            uiText.color = c;
        }
    }
}

