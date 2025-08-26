using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hit_Animation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Light2D ballLight;
    private Vector3 originalScale;
    private float originalLightIntensity;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ballLight = GetComponentInChildren<Light2D>();
        originalScale = transform.localScale;

        if (ballLight != null)
        {
            originalLightIntensity = ballLight.intensity;
        }
    }

    public void Animate(float scaleMultiplier, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateCoroutine(scaleMultiplier, duration));
    }

    public void ResetAnimation()
    {
        StopAllCoroutines();
        transform.localScale = originalScale;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        if (ballLight != null)
        {
            ballLight.intensity = originalLightIntensity;
        }
    }

    private System.Collections.IEnumerator AnimateCoroutine(float scaleMultiplier, float duration)
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * scaleMultiplier;

        Color startColor = spriteRenderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

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
