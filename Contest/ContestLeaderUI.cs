using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContestLeaderUI : MonoBehaviour
{
    [SerializeField] ContestLeaderItem[] playerUI;
    [SerializeField] ContestLeaderItem currentPlayerUI;

    public void WhenOpenLeagueUI()
    {
        AlfucodeAPIContest.instance.GetContestStatus();
        Debug.Log("Updated");
        var players = AlfucodeAPIContest.instance.currantContest.Leaderboard;

        for (int i = 3; i < 50; i++)
        {
            playerUI[i].PlayerRank.text = (i + 1).ToString();
            playerUI[i].PlayerScore.text = "0";
            playerUI[i].PlayerName.text = "No Player";
        }

        if (players.Count > 0)
        {
            for (int i = 0; i < Mathf.Min(players.Count, 50); i++)
            {
                playerUI[i].PlayerName.text = players[i].DisplayName;
                playerUI[i].PlayerScore.text = players[i].StatValue.ToString();
            }
        }
    }

    public void SetCurrentPlayer()
    {
        var player = AlfucodeAPIContest.instance.playerAround.Leaderboard[0];

        if (player.StatValue > 0)
            currentPlayerUI.PlayerRank.text = (player.Position+1).ToString();
        else
            currentPlayerUI.PlayerRank.text = "-";

        currentPlayerUI.PlayerScore.text = player.StatValue.ToString();
        currentPlayerUI.PlayerName.text = player.DisplayName;

    }

    private void OnEnable()
    {
        if (AlfucodeAPIContest.instance.contestStatus == MetaData.StatusContest.NewContest || AlfucodeAPIContest.instance.contestStatus == MetaData.StatusContest.CurrantConetest)
        {
            WhenOpenLeagueUI();
            SetCurrentPlayer();
        }
    }

}
