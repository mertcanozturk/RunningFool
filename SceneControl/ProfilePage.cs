using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProfilePage : MonoBehaviour
{
    [SerializeField] GameObject profilePagePanel;
    [SerializeField]
    TextMeshProUGUI jumpValueText, deathValueText, reviveValueText, loginValueText,
        playValueText, timeSpendValueText, scoreValueText, worldReachedValueText, levelReachedText, globalRankText, playerName;

    void Start()
    {
        AlfucodeAPILeaderBored.instance.GetTopPlayerLeague();
    }

    void InitializeTexts()
    {
        jumpValueText.text = AchievementsManager.Instance.JumpTimes.ToString();
        deathValueText.text = AchievementsManager.Instance.DieTimes.ToString();
        reviveValueText.text = AchievementsManager.Instance.ReviveTimes.ToString();

        int day = PlayerPrefs.GetInt("DailyMissionDay");

        if (day == 1)

            loginValueText.text = PlayerPrefs.GetInt("DailyMissionDay") + " day";
        else
            loginValueText.text = PlayerPrefs.GetInt("DailyMissionDay") + " days";


        playValueText.text = AchievementsManager.Instance.PlayTimes.ToString() + " times";
        scoreValueText.text = GameManagerIngame.Instance.Score.ToString();
        timeSpendValueText.text = (GameManagerIngame.Instance.GamePlayTime / 60).ToString() + " mins";
        playerName.text = GameManager.Player.NamePlayer;

        string worldreached = "-", levelreached = "-";

        foreach (var world in MetaData.ConstVariable.Planet.planets)
        {
            for (int i = 1; i <= 20; i++)
            {
                if (PlayerPrefs.HasKey(world.key + "_Level_" + i + "_Passed"))
                {
                    worldreached = world.name;
                    levelreached = i.ToString();
                }
            }
        }


        worldReachedValueText.text = worldreached;
        levelReachedText.text = levelreached;


        foreach (var item in AlfucodeAPILeaderBored.instance.Around.Leaderboard)
        {
            if (item.DisplayName == GameManager.Player.NamePlayer)
            {
                globalRankText.text = (item.Position + 1).ToString();
            }
        }

    }


    public void OpenProfilePage()
    {
        InitializeTexts();

        FindObjectOfType<UIManager>().HidePlayButton();

        profilePagePanel.SetActive(true);

        PlayerLog.Instance.IncreaseProfileButtonCounter();
    }

    public void CloseProfilePage()
    {
        FindObjectOfType<UIManager>().ShowPlayButton();

        profilePagePanel.SetActive(false);
    }
}
