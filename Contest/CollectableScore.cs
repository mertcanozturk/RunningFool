using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CollectableScore : MonoBehaviour
{
    public Transform top;

    [System.Serializable]
    public struct ScoreStruct
    {
        public Sprite sprite;
        public int value;
    }

    public ScoreStruct[] scores;

    SpriteRenderer spriteRenderer;

    public bool autoPlay = false;
    public float duration = 4f;
    public Ease _moveEase = Ease.Linear;

    public int randomScore;


    private Vector3 originalPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
            {
                GameManagerIngame.Instance.Contest.CollectScore(scores[randomScore].value);
                Destroy(gameObject);
            }
        }
    }


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalPosition = transform.position;

        randomScore = Random.Range(0, scores.Length);

        duration = duration - (scores[randomScore].value * .05f);

        spriteRenderer.sprite = scores[randomScore].sprite;

        StartCoroutine(Loop());
    }


    IEnumerator Loop()
    {
        while (true)
        {
            transform.DOMove(top.position, duration).SetEase(_moveEase);
            yield return new WaitForSeconds(duration);
            transform.DOMove(originalPosition, duration).SetEase(_moveEase);
            yield return new WaitForSeconds(duration);
        }
    }
}
