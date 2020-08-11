using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLocation : MonoBehaviour
{
    [SerializeField] private float lifeTime;

    private PolygonCollider2D collider2d;
    private SpriteRenderer spriteRenderer;
    Color baseColor;
    void Start()
    {
        collider2d = transform.GetComponentInChildren<PolygonCollider2D>();
        spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        StartCoroutine(CreateAgainMe());

    }

    IEnumerator CreateAgainMe()
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

        yield return new WaitForSeconds(lifeTime);
        
        GoToRandomLocation();
        
        StartCoroutine(CreateAgainMe());
    }

    void GoToRandomLocation()
    {
        Vector3 rotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Random.Range(transform.eulerAngles.y + 40, 150));
        transform.Rotate(rotation);
    }

}
