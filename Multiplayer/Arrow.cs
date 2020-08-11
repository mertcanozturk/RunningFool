using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    SpriteRenderer sR;
    // Start is called before the first frame update
    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            sR.DOFade(0f, 1f).From().SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(1);
            sR.DOFade(255f, 1f).From().SetEase(Ease.OutQuad);
        }

    }

}
