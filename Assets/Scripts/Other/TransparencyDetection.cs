using System.Collections;
using UnityEngine;

public class TransparencyDetection : MonoBehaviour
{
    private const float FullNonTransparent = 1.0f;
    
    [Range(0f, 1f)]
    [SerializeField] private float transparencyAmount = 0.8f;
    [SerializeField] private float fadeTime = 0.5f;

    
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D (Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            if (collider is CapsuleCollider2D)
            {
                StartCoroutine(FadeRoutine(_spriteRenderer, fadeTime, _spriteRenderer.color.a, transparencyAmount));
            }
            
        }
    }

    private void OnTriggerExit2D (Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            if (collider is CapsuleCollider2D)
            {
                StartCoroutine(FadeRoutine(_spriteRenderer, fadeTime, _spriteRenderer.color.a, FullNonTransparent));
            }
          
        }
        
    }


    private IEnumerator FadeRoutine(SpriteRenderer spriteRenderer, float fadeTimer, float startTransparencyAmount, float targetTransparencyAmount )
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            
            float newAlpha = Mathf.Lerp(startTransparencyAmount, targetTransparencyAmount,  elapsedTime/fadeTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
            
            yield return null;
        }
    }
    
}
