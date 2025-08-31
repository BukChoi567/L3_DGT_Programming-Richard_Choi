using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hit_Animation : MonoBehaviour
{
    // Reference to SpriteRenderer
    private SpriteRenderer spriteRenderer;
    // Reference to Light2D for glow effect
    private Light2D ballLight;
    // Original scale
    private Vector3 originalScale;
    // Original light intensity
    private float originalLightIntensity;

    void Start()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        ballLight = GetComponentInChildren<Light2D>();
        originalScale = transform.localScale;
        if (ballLight != null)
        {
            originalLightIntensity = ballLight.intensity;
        }
    }

    // Animate the object by scale change and fade duration
    public void Animate(float scaleMultiplier, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateCoroutine(scaleMultiplier, duration));
    }

    public void ResetAnimation()
    // Reset to original state
    {
        StopAllCoroutines();
        transform.localScale = originalScale;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        if (ballLight != null)
        {
            ballLight.intensity = originalLightIntensity;
        }
    }

    // Coroutine to handle animation over time
    private System.Collections.IEnumerator AnimateCoroutine(float scaleMultiplier, float duration)
    {
        // change scale and color over duration
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * scaleMultiplier;

        Color startColor = spriteRenderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // change light intensity if light exists
        float startIntensity = ballLight != null ? ballLight.intensity : 0f;
        float targetIntensity = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            spriteRenderer.color = Color.Lerp(startColor, targetColor, t);

            if (ballLight != null)
            {
                ballLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        spriteRenderer.color = targetColor;

        if (ballLight != null)
        {
            ballLight.intensity = targetIntensity;
        }
    }
}
