using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeForMultiplayer : MonoBehaviour
{
    private Collider2D collider2d;
    private SpriteRenderer spriteRenderer;
    Color baseColor;
    void Start()
    {
        collider2d = transform.GetComponentInChildren<Collider2D>();
        spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        StartCoroutine(FadeEffect());
    }

    IEnumerator FadeEffect()
    {
        collider2d.enabled = false;

        spriteRenderer.color = new Color(255, 240, 0);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = baseColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = new Color(255, 240, 0);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = baseColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = new Color(255, 240, 0);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = baseColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = new Color(255, 240, 0);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = baseColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = new Color(255, 240, 0);
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = baseColor;
        collider2d.enabled = true;
    }
}
