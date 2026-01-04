using System;
using System.Collections;
using UnityEngine;

// Đơn giản: fade màn hình sang đen rồi sáng lại, dùng CanvasGroup
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [Header("Canvas Group đen full màn hình")]
    public CanvasGroup canvasGroup; // alpha 0 => trong suốt, 1 => đen

    void Awake()
    {
        Instance = this;
        if (canvasGroup == null)
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void FadeOutIn(float fadeDuration, Action onMiddle)
    {
        if (canvasGroup == null)
        {
            onMiddle?.Invoke();
            return;
        }

        StartCoroutine(FadeOutInRoutine(fadeDuration, onMiddle));
    }

    IEnumerator FadeOutInRoutine(float fadeDuration, Action onMiddle)
    {
        // Fade to black
        canvasGroup.blocksRaycasts = true;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        // Thực hiện hành động ở giữa (teleport, đổi lighting...)
        onMiddle?.Invoke();

        // Fade back to clear
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
