using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContestUI : MonoBehaviour
{
    public GameObject ContestPanel;
    public TMP_Text timeText;
    public GameObject MainUI, Main, Leaderboard, Rules, Rewards, Winners, background;
    public Button startButton, facebookButton;


    [Header("Winner Screen References")]
    public SkinnedMeshRenderer player1Mesh;
    public SkinnedMeshRenderer player2Mesh;
    public SkinnedMeshRenderer player3Mesh;
    public TMP_Text player1Name, player2Name, player3Name;
    public GameObject player1, player2, player3;

    [Header("Rewards References")]
    public TMP_Text firstCoinValue;
    public TMP_Text secondCoinValue;
    public TMP_Text thirdCoinValue;
    public Image firstSkin;
    public Image secondSkin;
    public Image thirdSkin;

    private void Awake()
    {
        SetRemainingTimeText();

        if (AlfucodeAPIContest.instance.contestStatus == MetaData.StatusContest.NewContest)
        {
            CheckFacebookConnection();

            GetRewards();
        }
        else if (AlfucodeAPIContest.instance.contestStatus == MetaData.StatusContest.CurrantConetest)
        {
            CheckFacebookConnection();
            GetRewards();
        }
        else
        {
            GetWinners();
        }

    }

    public void SuccessConnection()
    {
        startButton.gameObject.SetActive(true);
        facebookButton.gameObject.SetActive(false);
    }

    public void ResetScoreData()
    {
        AlfucodeAPIContest.instance.CreatePlayerStatisticDefinition(0);
        AlfucodeAPIContest.instance.GetContestStatus();
        startButton.gameObject.SetActive(true);
        facebookButton.gameObject.SetActive(false);
    }

    public void CheckFacebookConnection()
    {
        if (GameManager.Scurity.Checking(MetaData.ConstVariable.FilesSetting.IsFBLogin))
        {
            startButton.gameObject.SetActive(true);
            facebookButton.gameObject.SetActive(false);

        }
        else
        {
            startButton.gameObject.SetActive(false);
            facebookButton.gameObject.SetActive(true);
        }
    }


    public void SetRemainingTimeText()
    {
        if (AlfucodeAPIContest.instance.contestStatus == MetaData.StatusContest.NewContest || AlfucodeAPIContest.instance.contestStatus == MetaData.StatusContest.CurrantConetest)
        {
            DateTime start = DateTime.ParseExact(AlfucodeAPIContest.instance.contestData.EndTime, "M/dd/yyyy HH:mm:ss", null);
            DateTime serverTime = AlfucodeAPIServer.instance.Data.ServerDateTime;
            TimeSpan day = start - serverTime;

            if (day.Days > 0)
                timeText.text = string.Format("{0} days {1} hours", (int)day.TotalDays, day.Hours);
            else
                timeText.text = string.Format("{0} hours", day.Hours);

        }
    }

    public void GetRewards()
    {
        foreach (var item in AlfucodeAPIContest.instance.contestData.ListPrize)
        {
            if (item.Range == 0)
            {
                if (item.VirtualCurrancy == MetaData.VirtualCurrency.AlfuCoin)
                {
                    firstCoinValue.text = item.Amount.ToString();
                }
                else
                {
                    firstSkin.sprite = Resources.Load<Sprite>(MetaData.ConstVariable.Character.Find(item.VirtualItem.ToString()).iconFilePath);
                }

            }
            if (item.Range == 1)
            {
                if (item.VirtualCurrancy == MetaData.VirtualCurrency.AlfuCoin)
                {
                    secondCoinValue.text = item.Amount.ToString();
                }
                else
                {
                    secondSkin.sprite = Resources.Load<Sprite>(MetaData.ConstVariable.Character.Find(item.VirtualItem.ToString()).iconFilePath);
                }

            }
            if (item.Range == 2)
            {
                if (item.VirtualCurrancy == MetaData.VirtualCurrency.AlfuCoin)
                {
                    thirdCoinValue.text = item.Amount.ToString();
                }
                else
                {
                    thirdSkin.sprite = Resources.Load<Sprite>(MetaData.ConstVariable.Character.Find(item.VirtualItem.ToString()).iconFilePath);
                }

            }
        }
    }

    public void GetWinners()
    {
        Winners.SetActive(true);

        if (AlfucodeAPIContest.instance.currantContest.Leaderboard.Count > 0)
        {
            player1Name.text = AlfucodeAPIContest.instance.currantContest.Leaderboard[0].Profile.DisplayName;
            player1Mesh.material = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(GameManager.instance.skin1Player).materialPath);
        }

        if (AlfucodeAPIContest.instance.currantContest.Leaderboard.Count > 1)
        {
            player2Name.text = AlfucodeAPIContest.instance.currantContest.Leaderboard[1].Profile.DisplayName;
            player2Mesh.material = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(GameManager.instance.skin2Player).materialPath);
        }

        if (AlfucodeAPIContest.instance.currantContest.Leaderboard.Count > 2)
        {
            player3Name.text = AlfucodeAPIContest.instance.currantContest.Leaderboard[2].Profile.DisplayName;
            player3Mesh.material = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(GameManager.instance.skin3Player).materialPath);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.Contest);
    }

    public void OpenLeaderboard()
    {
        Leaderboard.SetActive(true);
        Rules.SetActive(false);
        Rewards.SetActive(false);
        Winners.SetActive(false);
    }

    public void OpenRules()
    {
        Leaderboard.SetActive(false);
        Rules.SetActive(true);
        Rewards.SetActive(false);
        Winners.SetActive(false);
    }

    public void OpenRewards()
    {
        Leaderboard.SetActive(false);
        Rules.SetActive(false);
        Rewards.SetActive(true);
        Winners.SetActive(false);
    }

    public void OpenWinners()
    {
        Leaderboard.SetActive(false);
        Rules.SetActive(false);
        Rewards.SetActive(false);
        Winners.SetActive(true);
    }

    public void GoToContestMain()
    {
        CheckFacebookConnection();

        if (AlfucodeAPIContest.instance.contestStatus == MetaData.StatusContest.NewContest || AlfucodeAPIContest.instance.contestStatus == MetaData.StatusContest.CurrantConetest)
        {
            Winners.SetActive(false);

        }
        else
        {
            GetWinners();
        }

        ContestPanel.SetActive(true);
        Leaderboard.SetActive(false);
        Rules.SetActive(false);
        Rewards.SetActive(false);
        Main.SetActive(true);
        background.SetActive(false);
        MainUI.SetActive(false);
    }

    public void GoToMain()
    {

        FindObjectOfType<UIManager>().ShowPlayButton();

        Main.SetActive(true);
        Leaderboard.SetActive(false);
        Rules.SetActive(false);
        Rewards.SetActive(false);
        ContestPanel.SetActive(false);
        background.SetActive(true);
        MainUI.SetActive(true);
    }
}
