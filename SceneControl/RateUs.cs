using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateUs : MonoBehaviour
{
    public bool isMainRateUsPanel = false;

    public Toggle[] toggles;
    public GameObject panel, watchAdsSkinPanel;

    public int selectedToggle = 0;

    public bool isSelected = false;




    private void Start()
    {
        if (!isMainRateUsPanel)
            GameManagerIngame.Instance.LevelManager.LevelUpEvent += LevelUpEvent;
    }

    public void LevelUpEvent()
    {

        if (!PlayerPrefs.HasKey("RateButtonUsed") && GameManagerIngame.Instance.LevelManager.Level % 4 == 0 && watchAdsSkinPanel.activeSelf == false)
        {
            panel.SetActive(true);
        }
    }

    public void RateButton()
    {
        if (selectedToggle != 0)
        {
            if (selectedToggle > 3)
            {
#if UNITY_ANDROID
                Application.OpenURL(InitializerManager.instance.Data.SettingGame.VoteButton);
#elif UNITY_IOS
                Application.OpenURL(InitializerManager.instance.Data.SettingGame.VoteButtonIOS);
#endif

                PlayerPrefs.SetInt("RateButtonUsed", 1);
            }
        }

        ClosePanel();
    }


    public void ClosePanel()
    {
        panel.SetActive(false);
    }

    public void Later()
    {
        panel.SetActive(false);
    }

    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    public void Toggle1OnValueChanged(bool value)
    {
        if (isSelected) return;

        isSelected = true;
        selectedToggle = 1;

        for (int i = 0; i < 5; i++)
        {
            toggles[i].isOn = false;
        }
        for (int i = 0; i < 1; i++)
        {
            toggles[i].isOn = true;
        }
        isSelected = false;

    }
    public void Toggle2OnValueChanged(bool value)
    {
        if (isSelected) return;
        isSelected = true;

        selectedToggle = 2;

        for (int i = 0; i < 5; i++)
        {
            toggles[i].isOn = false;
        }
        for (int i = 0; i < 2; i++)
        {
            toggles[i].isOn = true;
        }
        isSelected = false;

    }
    public void Toggle3OnValueChanged(bool value)
    {
        if (isSelected) return;
        selectedToggle = 3;
        isSelected = true;

        for (int i = 0; i < 5; i++)
        {
            toggles[i].isOn = false;
        }
        for (int i = 0; i < 3; i++)
        {
            toggles[i].isOn = true;
        }
        isSelected = false;

    }
    public void Toggle4OnValueChanged(bool value)
    {
        if (isSelected) return;
        isSelected = true;

        selectedToggle = 4;

        for (int i = 0; i < 5; i++)
        {
            toggles[i].isOn = false;
        }
        for (int i = 0; i < 4; i++)
        {
            toggles[i].isOn = true;
        }
        isSelected = false;

    }
    public void Toggle5OnValueChanged(bool value)
    {
        if (isSelected) return;
        isSelected = true;

        selectedToggle = 5;

        for (int i = 0; i < 5; i++)
        {
            toggles[i].isOn = false;
        }
        for (int i = 0; i < 5; i++)
        {
            toggles[i].isOn = true;
        }
        isSelected = false;

    }

}
