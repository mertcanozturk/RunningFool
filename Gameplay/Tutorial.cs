using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public TextMeshProUGUI touchtext, holdittext,holdit2Text, completeImage;

    public void ShowTouchTutorial()
    {
        touchtext.gameObject.SetActive(true);
    }
    public void HideTouchTutorial()
    {
        touchtext.gameObject.SetActive(false);
    }
    public void ShowHoldItTutorial()
    {
        holdittext.gameObject.SetActive(true);
    }
    public void HideoldItTutorial()
    {
        holdittext.gameObject.SetActive(false);
    }
    public void ShowHoldIt2Tutorial()
    {
        holdit2Text.gameObject.SetActive(true);
    }
    public void HideoldIt2Tutorial()
    {
        holdit2Text.gameObject.SetActive(false);
    }

    public void ShowCompleteTutorial()
    {
        completeImage.gameObject.SetActive(true);
    }
    public void HideCompleteTutorial()
    {
        completeImage.gameObject.SetActive(false);

        foreach (var item in GameObject.FindGameObjectsWithTag("Tutorial"))
        {
            Destroy(item);
        }
    }
}
