using System;
using System.Collections;
using UnityEngine;

// Đơn giản: fade màn hình sang đen rồi sáng lại, dùng CanvasGroup
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [Header("Canvas Group đen full màn hình")]
    public CanvasGroup canvasGroup; // alpha 0 => trong suốt, 1 => đen

    [Header("Startup")]
    [SerializeField] private bool startBlack = false;

    void Awake()
    {
        Instance = this;
        if (canvasGroup == null)
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = startBlack ? 1f : 0f;
            canvasGroup.blocksRaycasts = startBlack;
        }
    }

    public void SetBlackImmediate()
    {
        if (canvasGroup == null) return;
        StopAllCoroutines();
        SetAlphaImmediate(1f, true);
    }

    public void SetClearImmediate()
    {
        if (canvasGroup == null) return;
        StopAllCoroutines();
        SetAlphaImmediate(0f, false);
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

    public void FadeOut(float fadeDuration)
    {
        if (canvasGroup == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeToRoutine(1f, fadeDuration, true));
    }

    public void FadeIn(float fadeDuration)
    {
        if (canvasGroup == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeToRoutine(0f, fadeDuration, false));
    }

    public void FadeOutInHold(float fadeDuration, float holdSeconds, Action onMiddle = null)
    {
        if (canvasGroup == null)
        {
            onMiddle?.Invoke();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FadeOutInHoldRoutine(fadeDuration, holdSeconds, onMiddle));
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

    IEnumerator FadeOutInHoldRoutine(float fadeDuration, float holdSeconds, Action onMiddle)
    {
        yield return FadeToRoutine(1f, fadeDuration, true);

        if (holdSeconds > 0f)
            yield return new WaitForSeconds(holdSeconds);

        onMiddle?.Invoke();

        yield return FadeToRoutine(0f, fadeDuration, false);
    }

    IEnumerator FadeToRoutine(float targetAlpha, float fadeDuration, bool blockRaycasts)
    {
        if (canvasGroup == null) yield break;

        canvasGroup.blocksRaycasts = blockRaycasts;
        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        if (fadeDuration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            if (!blockRaycasts)
                canvasGroup.blocksRaycasts = false;
            yield break;
        }

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, lerp);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        if (!blockRaycasts)
            canvasGroup.blocksRaycasts = false;
    }

    private void SetAlphaImmediate(float targetAlpha, bool blockRaycasts)
    {
        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = blockRaycasts;
    }
}
