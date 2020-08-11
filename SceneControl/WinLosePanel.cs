using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLosePanel : MonoBehaviour
{
    public GameObject nextButton, homeButton;
    public Transform homeButtonDefaultPosition;

    void Start()
    {
        if (GameManagerIngame.Instance.isChallengeGame)
        {
            nextButton.SetActive(false);
            homeButton.transform.position = homeButtonDefaultPosition.position;
        }
    }


}
