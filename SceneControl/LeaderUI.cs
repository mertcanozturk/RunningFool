using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderUI : MonoBehaviour
{
    [SerializeField] GameObject[] playerUI;
    [SerializeField] GameObject currentPlayerUI;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject mainCanvas;

    void Start()
    {
        AlfucodeAPILeaderBored.instance.GetTopPlayerLeague();
    }

    public void WhenOpenLeagueUI()
    {
        var players = AlfucodeAPILeaderBored.instance.Player.Leaderboard;

        for (int i = 0; i < 50; i++)
        {
            playerUI[i].transform.Find("PlayerRank").GetComponent<TextMeshProUGUI>().text = (i + 1).ToString(); 
            playerUI[i].transform.Find("Score").GetComponent<TextMeshProUGUI>().text = "0";
        }

        if (players.Count > 0)
        {
            for (int i = 0; i < Mathf.Min(players.Count, 50); i++)
            {
                playerUI[i].transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = players[i].DisplayName;
                playerUI[i].transform.Find("Score").GetComponent<TextMeshProUGUI>().text = players[i].StatValue.ToString();
            }
        }

        foreach (var item in AlfucodeAPILeaderBored.instance.Around.Leaderboard)
        {
            if (item.DisplayName == GameManager.Player.NamePlayer)
            {
                currentPlayerUI.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = item.DisplayName;
                currentPlayerUI.transform.Find("PlayerScore").GetComponent<TextMeshProUGUI>().text = item.StatValue.ToString();
                currentPlayerUI.transform.Find("Rank").GetComponent<TextMeshProUGUI>().text = (item.Position + 1).ToString();
            }
        }
    }

    public void Open()
    {
        FindObjectOfType<UIManager>().HidePlayButton();
        mainCanvas.SetActive(false);
        panel.SetActive(true);
        WhenOpenLeagueUI();
        PlayerLog.Instance.IncreaseLeaderboardButtonCounter();
    }

    public void Close()
    {
        mainCanvas.SetActive(true);
        FindObjectOfType<UIManager>().ShowPlayButton();
        panel.SetActive(false);
    }

}
