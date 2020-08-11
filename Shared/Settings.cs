using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : Singleton<Settings>
{
    public bool musicEnabled, gameSoundEnabled, vibrationEnabled;

    private void Awake()
    {
        Initialize();
    }
    void Initialize()
    {
        if (PlayerPrefs.HasKey("Set_Music"))
        {
            musicEnabled = PlayerPrefs.GetInt("Set_Music") == 1 ? true : false;
            gameSoundEnabled = PlayerPrefs.GetInt("Set_GameSound") == 1 ? true : false;
            vibrationEnabled = PlayerPrefs.GetInt("Set_Vibration") == 1 ? true : false;
        }
        else
        {
            musicEnabled = gameSoundEnabled = vibrationEnabled = true;
            PlayerPrefs.SetInt("Set_Music", 1);
            PlayerPrefs.SetInt("Set_GameSound", 1);
            PlayerPrefs.SetInt("Set_Vibration", 1);
        }
    }
}
