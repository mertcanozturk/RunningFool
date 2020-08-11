using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewards : MonoBehaviour
{
    [SerializeField] Text timeText;
    [SerializeField] Button button;

    private int seconds = int.MinValue;

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (!PlayerPrefs.HasKey("RecentlyReceivedDailyReward"))
        {
            PlayerPrefs.SetString("RecentlyReceivedDailyReward", DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy HH:mm:ss"));
            PlayerPrefs.SetInt("RewardCounter", 0);
            Debug.Log("You can take daily reward");
            button.interactable = true;
        }
        else
        {
            CheckTime();

            if (seconds > 0)
            {
                StartCoroutine(TimeCounter());
            }
            else
            {
                Debug.Log("You can take daily reward");
                button.interactable = true;

            }
        }
    }

    IEnumerator TimeCounter()
    {
        while (seconds > 0)
        {
            yield return new WaitForSeconds(1);
            seconds--;
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            string timeText = time.ToString(@"hh\:mm\:ss");
            this.timeText.text = timeText;
            Debug.Log(timeText);
        }

        Debug.Log("You can take your daily reward");
        button.interactable = true;
    }

    public DateTime GetFastestNISTDate()
    {
        var result = DateTime.MinValue;
        // Initialize the list of NIST time servers
        // http://tf.nist.gov/tf-cgi/servers.cgi
        string[] servers = new string[] {
"nist1-ny.ustiming.org",
"nist1-nj.ustiming.org",
"nist1-pa.ustiming.org",
"time-a.nist.gov",
"time-b.nist.gov",
"nist1.aol-va.symmetricom.com",
"nist1.columbiacountyga.gov",
"nist1-chi.ustiming.org",
"nist.expertsmi.com",
"nist.netservicesgroup.com"
};

        // Try 5 servers in random order to spread the load
        System.Random rnd = new System.Random();
        foreach (string server in servers.OrderBy(s => rnd.NextDouble()).Take(5))
        {
            try
            {
                // Connect to the server (at port 13) and get the response
                string serverResponse = string.Empty;
                using (var reader = new StreamReader(new System.Net.Sockets.TcpClient(server, 13).GetStream()))
                {
                    serverResponse = reader.ReadToEnd();
                }

                // If a response was received
                if (!string.IsNullOrEmpty(serverResponse))
                {
                    // Split the response string ("55596 11-02-14 13:54:11 00 0 0 478.1 UTC(NIST) *")
                    string[] tokens = serverResponse.Split(' ');

                    // Check the number of tokens
                    if (tokens.Length >= 6)
                    {
                        // Check the health status
                        string health = tokens[5];
                        if (health == "0")
                        {
                            // Get date and time parts from the server response
                            string[] dateParts = tokens[1].Split('-');
                            string[] timeParts = tokens[2].Split(':');

                            // Create a DateTime instance
                            DateTime utcDateTime = new DateTime(
                                Convert.ToInt32(dateParts[0]) + 2000,
                                Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]),
                                Convert.ToInt32(timeParts[0]), Convert.ToInt32(timeParts[1]),
                                Convert.ToInt32(timeParts[2]));

                            // Convert received (UTC) DateTime value to the local timezone
                            result = utcDateTime.ToLocalTime();

                            return result;
                            // Response successfully received; exit the loop

                        }
                    }
                }
            }
            catch
            {
                // Ignore exception and try the next server
            }
        }
        //return result;
        return DateTime.Now;
    }

    void CheckTime()
    {
        DateTime now = DateTime.Now;
        DateTime receivedTime = Convert.ToDateTime(PlayerPrefs.GetString("RecentlyReceivedDailyReward"));
        TimeSpan fark = now.Subtract(receivedTime);
        seconds = 86400 - (int)fark.TotalSeconds;
    }

    public void TakeRewards()
    {
        Debug.Log("You took your daily reward.");

        int dayCount = PlayerPrefs.GetInt("RewardCounter") + 1;
        PlayerPrefs.SetInt("RewardCounter", +dayCount);

        var reward = MetaData.ConstVariable.DailyRewards.Find("day" + dayCount);

        switch (reward.reward.type)
        {
            case MetaData.ConstVariable.DailyRewards.RewardType.Score:

                break;
            case MetaData.ConstVariable.DailyRewards.RewardType.Gold:
                ShopManager.Instance.AddCoin((int)reward.reward.value);
                break;
            case MetaData.ConstVariable.DailyRewards.RewardType.AlfuCoin:

                break;
            case MetaData.ConstVariable.DailyRewards.RewardType.Character:
                ShopManager.Instance.AddCoin(MetaData.ConstVariable.Character.Find((string)reward.reward.value).price);
                ShopManager.Instance.BuyCharacter(MetaData.ConstVariable.Character.Find((string)reward.reward.value).key);
                break;
            default:
                break;
        }

        PlayerPrefs.SetString("RecentlyReceivedDailyReward", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
        CheckTime();
        StartCoroutine(TimeCounter());
    }
}
