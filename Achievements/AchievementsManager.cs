using Assets.Scripts.Alfucode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MetaData.ConstVariable.Achievements;
public class AchievementsManager : Singleton<AchievementsManager>
{

    [SerializeField] List<AchievementInfo> completedAchievements, notifications;

    #region AllTimes

    [SerializeField] private int jumpTimes;
    [SerializeField] private int dieTimes;
    [SerializeField] private int playTimes;
    [SerializeField] private int reviveTimes;
    [SerializeField] private int loginTimes;
    [SerializeField] private float gamePlayTime;
    #endregion

    #region Records
    [SerializeField] private float minimumLevelPassTime;
    #endregion

    #region InLevelCounters
    [SerializeField] private int dieTimesInCurrentLevel;
    [SerializeField] private int jumpTimesInCurrentLevel;
    #endregion

    public int JumpTimes { get { return jumpTimes; } }
    public int DieTimes { get { return dieTimes; } }
    public int PlayTimes { get { return playTimes; } }
    public int ReviveTimes { get { return reviveTimes; } }
    public int LoginTimes { get { return loginTimes; } }
    public float GamePlayTime { get { return gamePlayTime; } }
    public int JumpTimesInCurrentLevel { get { return jumpTimesInCurrentLevel; } }
    public int DieTimesInCurrentLevel { get { return dieTimesInCurrentLevel; } }


    private void Awake()
    {
        InitializeParameters();
        InitializeAchievements();
        StartCoroutine(TimeCounter());
        IncreaseLoginCounter();
        InitializeDailyMissions();
    }


    private void InitializeParameters()
    {
        completedAchievements = new List<AchievementInfo>();
        notifications = new List<AchievementInfo>();

        foreach (var achievement in achievements)
        {
            if (PlayerPrefs.GetInt(achievement.key) == 1)
            {
                completedAchievements.Add(achievement);
            }
        }

        if (PlayerPrefs.HasKey("Ach_jumpTimes"))
        {
            jumpTimes = PlayerPrefs.GetInt("Ach_jumpTimes");
            dieTimes = PlayerPrefs.GetInt("Ach_dieTimes");
            playTimes = PlayerPrefs.GetInt("Ach_playTimes");
            reviveTimes = PlayerPrefs.GetInt("Ach_reviveTimes");
            loginTimes = PlayerPrefs.GetInt("DailyMissionDay");
            gamePlayTime = PlayerPrefs.GetInt("GamePlayTime");

            minimumLevelPassTime = float.Parse(PlayerPrefs.GetString("Rec_minimumLevelPassTime"));
        }
        else
        {
            PlayerPrefs.SetInt("Ach_jumpTimes", 0);
            PlayerPrefs.SetInt("Ach_dieTimes", 0);
            PlayerPrefs.SetInt("Ach_playTimes", 0);
            PlayerPrefs.SetInt("Ach_reviveTimes", 0);
            PlayerPrefs.SetInt("Ach_loginTimes", 0);
            PlayerPrefs.SetFloat("GamePlayTime", 0);
            PlayerPrefs.SetString("Rec_minimumLevelPassTime", "0");

            jumpTimes = dieTimes = playTimes = reviveTimes = dieTimesInCurrentLevel = jumpTimesInCurrentLevel = 0;
            minimumLevelPassTime = 0f;
        }
    }

    private void InitializeDailyMissions()
    {
        if (!PlayerPrefs.HasKey("DailyMissionDay"))
        {
            PlayerPrefs.SetInt("DailyMissionDay", 1);
            PlayerPrefs.SetInt("DailyMissionDateDay", DateTime.Now.Day);
            PlayerPrefs.SetInt("DailyMissionTimes", 0);
            PlayerPrefs.SetInt("DailyMissionCollected", 0);
            PlayerPrefs.SetInt("DailyReviveCount", 0);
        }
        else
        {
            if (PlayerPrefs.GetInt("DailyMissionDateDay") != DateTime.Now.Day)
            {
                PlayerPrefs.SetInt("DailyMissionDay", PlayerPrefs.GetInt("DailyMissionDay") + 1);
                PlayerPrefs.SetInt("DailyMissionDateDay", DateTime.Now.Day);
                PlayerPrefs.SetInt("DailyMissionTimes", 0);
                PlayerPrefs.SetInt("DailyMissionCollected", 0);
                PlayerPrefs.SetInt("DailyReviveCount", 0);
            }
        }
    }

    IEnumerator TimeCounter()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            foreach (var achievement in Find(MetaData.ConstVariable.Achievements.Type.PlayTime))
            {
                if (GameManagerIngame.Instance.GamePlayTime >= achievement.times && achievement.completed != true)
                {
                    achievement.completed = true;
                    completedAchievements.Add(achievement);

                    Debug.Log(achievement.key + " is completed.");
                }
            }

        }
    }

    private void InitializeAchievements()
    {
        if (PlayerPrefs.HasKey(MetaData.ConstVariable.Achievements.achievements[0].key))
        {
            foreach (var achievement in MetaData.ConstVariable.Achievements.achievements)
            {
                achievement.completed = PlayerPrefs.GetInt(achievement.key) == 1 ? true : false;
                if (PlayerPrefs.HasKey("date_" + achievement.key))
                {
                    achievement.dateTime = Convert.ToDateTime(PlayerPrefs.GetString("date_" + achievement.key));
                }
            }
        }
        else
        {
            foreach (var achievement in MetaData.ConstVariable.Achievements.achievements)
            {
                PlayerPrefs.SetInt(achievement.key, 0);
            }
        }
    }

    public void IncreaseJumpCounter()
    {
        if (MetaData.ConstVariable.DailyMission.missions[PlayerPrefs.GetInt("DailyMissionDay")].type == MetaData.ConstVariable.DailyMission.Type.Jump)
        {
            PlayerPrefs.SetInt("DailyMissionTimes", PlayerPrefs.GetInt("DailyMissionTimes") + 1);
        }

        jumpTimes++;
        jumpTimesInCurrentLevel++;


        PlayerPrefs.SetInt("Ach_jumpTimes", PlayerPrefs.GetInt("Ach_jumpTimes") + 1);

    }

    public void IncreaseDieCounter()
    {
        if (MetaData.ConstVariable.DailyMission.missions[PlayerPrefs.GetInt("DailyMissionDay")].type == MetaData.ConstVariable.DailyMission.Type.Die)
        {
            PlayerPrefs.SetInt("DailyMissionTimes", PlayerPrefs.GetInt("DailyMissionTimes") + 1);
        }

        dieTimes++;
        dieTimesInCurrentLevel++;

        PlayerPrefs.SetInt("Ach_dieTimes", PlayerPrefs.GetInt("Ach_dieTimes") + 1);

    }

    public void ResetLevelCounters()
    {
        dieTimesInCurrentLevel = 0;
        jumpTimesInCurrentLevel = 0;
    }

    public void LevelUp(int level, float insec, int score)
    {
        if (insec >= minimumLevelPassTime)
        {
            minimumLevelPassTime = insec;

            Debug.Log("New Record! Minimum level pass Time: " + insec.ToString());

            PlayerPrefs.SetString("Rec_minimumLevelPassTime", insec.ToString());
        }
    }

    public void IncreasePlayTimes()
    {
        if (MetaData.ConstVariable.DailyMission.missions[PlayerPrefs.GetInt("DailyMissionDay")].type == MetaData.ConstVariable.DailyMission.Type.Play)
        {
            PlayerPrefs.SetInt("DailyMissionTimes", PlayerPrefs.GetInt("DailyMissionTimes") + 1);
        }
        playTimes++;

        PlayerPrefs.SetInt("Ach_playTimes", PlayerPrefs.GetInt("Ach_playTimes") + 1);
        SavePlayerInfo();
    }

    private void SavePlayerInfo()
    {
        PlayerGameInfo info = new PlayerGameInfo(jumpTimes, dieTimes, playTimes, reviveTimes, PlayerPrefs.GetInt("DailyMissionDay"),
             GameManagerIngame.Instance.GamePlayTime, minimumLevelPassTime, dieTimes, jumpTimesInCurrentLevel);
        ServerData.Instance.SavePlayerInfo(info);
    }

    public void IncreaseReviveTimes()
    {
        if (MetaData.ConstVariable.DailyMission.missions[PlayerPrefs.GetInt("DailyMissionDay")].type == MetaData.ConstVariable.DailyMission.Type.Revive)
        {
            PlayerPrefs.SetInt("DailyMissionTimes", PlayerPrefs.GetInt("DailyMissionTimes") + 1);
        }
        reviveTimes++;

        PlayerPrefs.SetInt("Ach_reviveTimes", PlayerPrefs.GetInt("Ach_reviveTimes") + 1);

    }

    public void IncreaseLoginCounter()
    {
        PlayerPrefs.SetInt("Ach_loginTimes", PlayerPrefs.GetInt("Ach_loginTimes") + 1);
    }

    public void GiveMeReward(AchievementInfo achievement)
    {
        switch (achievement.reward.type)
        {
            case RewardType.Score:
                GameManagerIngame.Instance.Score += (int)achievement.reward.value;
                break;
            case RewardType.Gold:
                ShopManager.Instance.AddCoin((int)achievement.reward.value);
                break;
            case RewardType.AlfuCoin:
                //add alfucoin

                break;
            case RewardType.Character:
                ShopManager.Instance.AddCoin(MetaData.ConstVariable.Character.Find((string)achievement.reward.value).price);
                ShopManager.Instance.BuyCharacter(MetaData.ConstVariable.Character.Find((string)achievement.reward.value).key);
                break;
            default:
                break;
        }
    }

    public void BringBackPlayerData(PlayerGameInfo info)
    {
        PlayerPrefs.SetInt("Ach_jumpTimes", info.jumpTimes);
        PlayerPrefs.SetInt("Ach_dieTimes", info.dieTimes);
        PlayerPrefs.SetInt("Ach_playTimes", info.playTimes);
        PlayerPrefs.SetInt("Ach_reviveTimes", info.reviveTimes);
        PlayerPrefs.SetInt("DailyMissionDay", info.loginTimes);


        jumpTimes = info.jumpTimes;
        dieTimes = info.dieTimes;
        playTimes = info.playTimes;
        reviveTimes = info.reviveTimes;
        loginTimes = info.loginTimes;


        GameManagerIngame.Instance.GamePlayTime = info.gamePlayTime;
    }
}
