using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject rateUsButton, musicButton, gameSoundButton, vibrationButton;
    [SerializeField]
    private GameObject panel;

    [SerializeField] Sprite enabledSprite, disabledSprite;




    public void OpenSettingPanel()
    {
        FindObjectOfType<UIManager>().HidePlayButton();

        panel.SetActive(true);
        if (Settings.Instance.musicEnabled)
        {
            musicButton.GetComponent<Image>().sprite = enabledSprite;
            musicButton.transform.Find("EnabledImage").gameObject.SetActive(true);
            musicButton.transform.Find("DisabledImage").gameObject.SetActive(false);
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = disabledSprite;
            musicButton.transform.Find("EnabledImage").gameObject.SetActive(false);
            musicButton.transform.Find("DisabledImage").gameObject.SetActive(true);
        }

        if (Settings.Instance.gameSoundEnabled)
        {
            gameSoundButton.GetComponent<Image>().sprite = enabledSprite;
            gameSoundButton.transform.Find("EnabledImage").gameObject.SetActive(true);
            gameSoundButton.transform.Find("DisabledImage").gameObject.SetActive(false);
        }
        else
        {
            gameSoundButton.GetComponent<Image>().sprite = disabledSprite;

            gameSoundButton.transform.Find("EnabledImage").gameObject.SetActive(false);
            gameSoundButton.transform.Find("DisabledImage").gameObject.SetActive(true);
        }


        if (Settings.Instance.vibrationEnabled)
        {
            vibrationButton.GetComponent<Image>().sprite = enabledSprite;
            vibrationButton.transform.Find("EnabledImage").gameObject.SetActive(true);
            vibrationButton.transform.Find("DisabledImage").gameObject.SetActive(false);
        }
        else
        {
            vibrationButton.GetComponent<Image>().sprite = disabledSprite;
            vibrationButton.transform.Find("EnabledImage").gameObject.SetActive(false);
            vibrationButton.transform.Find("DisabledImage").gameObject.SetActive(true);
        }
    }

    public void CloseSettingsPanel()
    {
        FindObjectOfType<UIManager>().ShowPlayButton();

        panel.SetActive(false);
    }


    public void VibrationButton()
    {
        if (Settings.Instance.vibrationEnabled)
        {
            vibrationButton.GetComponent<Image>().sprite = disabledSprite;

            PlayerPrefs.SetInt("Set_Vibration", 0);
            vibrationButton.transform.Find("EnabledImage").gameObject.SetActive(false);
            vibrationButton.transform.Find("DisabledImage").gameObject.SetActive(true);
            Settings.Instance.vibrationEnabled = false;
        }
        else
        {
            vibrationButton.GetComponent<Image>().sprite = enabledSprite;

            PlayerPrefs.SetInt("Set_Vibration", 1);
            vibrationButton.transform.Find("EnabledImage").gameObject.SetActive(true);
            vibrationButton.transform.Find("DisabledImage").gameObject.SetActive(false);
            Settings.Instance.vibrationEnabled = true;

        }
    }
    public void MusicButton()
    {
        if (Settings.Instance.musicEnabled)
        {
            musicButton.GetComponent<Image>().sprite = disabledSprite;

            PlayerPrefs.SetInt("Set_Music", 0);
            musicButton.transform.Find("EnabledImage").gameObject.SetActive(false);
            musicButton.transform.Find("DisabledImage").gameObject.SetActive(true);
            Settings.Instance.musicEnabled = false;
            GameManager.instance.SoundManager.MainSound.Stop();
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = enabledSprite;
            PlayerPrefs.SetInt("Set_Music", 1);
            musicButton.transform.Find("EnabledImage").gameObject.SetActive(true);
            musicButton.transform.Find("DisabledImage").gameObject.SetActive(false);
            Settings.Instance.musicEnabled = true;
            GameManager.instance.SoundManager.MainSound.Play();
        }
    }
    public void GameSoundButton()
    {
        if (Settings.Instance.gameSoundEnabled)
        {
            gameSoundButton.GetComponent<Image>().sprite = disabledSprite;

            PlayerPrefs.SetInt("Set_GameSound", 0);
            gameSoundButton.transform.Find("EnabledImage").gameObject.SetActive(false);
            gameSoundButton.transform.Find("DisabledImage").gameObject.SetActive(true);
            Settings.Instance.gameSoundEnabled = false;
        }
        else
        {
            gameSoundButton.GetComponent<Image>().sprite = enabledSprite;

            PlayerPrefs.SetInt("Set_GameSound", 1);
            gameSoundButton.transform.Find("EnabledImage").gameObject.SetActive(true);
            gameSoundButton.transform.Find("DisabledImage").gameObject.SetActive(false);
            Settings.Instance.gameSoundEnabled = true;

        }
    }

    public void RateUsButton()
    {

#if UNITY_ANDROID
        Application.OpenURL(InitializerManager.instance.Data.SettingGame.VoteButton);
#elif UNITY_IOS
        Application.OpenURL(InitializerManager.instance.Data.SettingGame.VoteButtonIOS);
#endif

    }
}
