using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.Interpolators;
using UnityEngine.UI;

public class HPBarDelay : MonoBehaviour
{
    [Header("Sliders")]
    public Slider topSlider;       // The main slider that user moves
    public Slider bottomSlider;    // The one that follows later

    [Header("Settings")]
    public float delay = 1.0f;     // Delay before bottom slider starts following
    public float lerpSpeed = 2.0f; // Speed of interpolation

    private Coroutine followCoroutine;

    void Start()
    {
        if (topSlider != null)
            topSlider.onValueChanged.AddListener(OnTopSliderChanged);
    }

    void OnTopSliderChanged(float newValue)
    {
        // If already following, stop the previous coroutine
        if (followCoroutine != null)
            StopCoroutine(followCoroutine);

        // Start a new delayed lerp coroutine
        followCoroutine = StartCoroutine(FollowWithDelay(newValue));
    }

    IEnumerator FollowWithDelay(float targetValue)
    {
        // Wait for the specified delay before starting the lerp
        yield return new WaitForSeconds(delay);

        float startValue = bottomSlider.value;
        float elapsed = 0f;

        // Smoothly interpolate bottom slider’s value
        while (Mathf.Abs(bottomSlider.value - targetValue) > 0.001f)
        {
            elapsed += Time.deltaTime * lerpSpeed;
            bottomSlider.value = Mathf.Lerp(startValue, targetValue, elapsed);
            yield return null;
        }

        bottomSlider.value = targetValue; // Snap to exact target at the end
    }
}
