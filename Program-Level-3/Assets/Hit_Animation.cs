using UnityEngine;

public class Hit_Animation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
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
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f); // Reset to fully opaque
    }
    private System.Collections.IEnumerator AnimateCoroutine(float scaleMultiplier, float duration)
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * scaleMultiplier;

        Color startColor = spriteRenderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Fully transparent

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            spriteRenderer.color = Color.Lerp(startColor, targetColor, t);
            elapsed += Time.deltaTime;
            
            yield return null;
        }

        transform.localScale = targetScale;
        spriteRenderer.color = targetColor;
    }
}

