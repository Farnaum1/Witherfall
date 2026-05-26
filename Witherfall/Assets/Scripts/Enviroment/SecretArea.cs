using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretArea : MonoBehaviour
{
    public float fadeDuration = 1f;
    private Color hiddenColor;
    private SpriteRenderer spriteRenderer;
    private Coroutine currentCoroutine;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hiddenColor = spriteRenderer.color;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(FadeSprite(false));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            currentCoroutine = StartCoroutine(FadeSprite(true));
        } 
    }

    private IEnumerator FadeSprite(bool fadeOut)
    {
        Color starColor = spriteRenderer.color;
        Color targetColor = fadeOut ? new Color(hiddenColor.r, 
            hiddenColor.g, hiddenColor.b, 0f) : hiddenColor;
        float timeFading = 0f;

        while(timeFading < fadeDuration)
        {
            spriteRenderer.color = Color.Lerp(starColor, targetColor, timeFading / fadeDuration);
            timeFading += Time.deltaTime;
            yield return null;

        }

        spriteRenderer.color = targetColor;

    }
}
