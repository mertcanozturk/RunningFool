using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Achievement : MonoBehaviour
{
    //[SerializeField] GameObject sampleAchievement;
    public AchievementItem sampleAchievement;
    [SerializeField] GameObject content;
    [SerializeField] GameObject panel;
    public AchievementDailyMissionItem dailyMissions;
    [SerializeField] GameObject[] dailyMissionCircles;
    [SerializeField] GameObject notifyImage;
    [SerializeField] Transform coinTransform;
    [SerializeField] Transform mainCanvas;
    [SerializeField] GameObject collectableCoins;
    [SerializeField] GameObject[] coins;
    [SerializeField] GameObject AddCoinParticle;

    bool isThereUnlockedSkin = false;
    bool isThereCompleted = false;
    string unlockedSkinKey = "";

    [SerializeField] GameObject newUnlockedSkinPanel;


    List<RectTransform> achievementsTransforms = new List<RectTransform>();
    bool achievementCreated = false;

    void Start()
    {
        string completed = "";
        string notCompleted = "";
        foreach (var achievement in MetaData.ConstVariable.Achievements.achievements)
        {
            if (PlayerPrefs.GetInt(achievement.key) == 1)
            {
                completed += achievement.name + Environment.NewLine;
            }
            else
            {
                notCompleted += achievement.name + Environment.NewLine;
            }
        }
        CreateAchievementsUI();
        CheckDailyMission();
        CreateFirstAchievementUI();
        CheckUnlockedSkin();

        if (isThereCompleted)
            notifyImage.SetActive(true);
        else
            notifyImage.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CollectAnimation(50);
        }
    }

    void CreateFirstAchievementUI()
    {
        sampleAchievement.transform.Find("AlfuCoinAmount").GetComponent<TextMeshProUGUI>().text = "50";
        sampleAchievement.transform.Find("AchievementText").GetComponent<TextMeshProUGUI>().text = "Play first time";

        if (PlayerPrefs.GetInt(MetaData.ConstVariable.Planet.planets[0].key + "_Level_" + 1 + "_Passed") == 1)
        {
            if (PlayerPrefs.HasKey("PlayFirstTimeCollected"))
            {
                sampleAchievement.transform.Find("CollectButton").gameObject.SetActive(false);
                sampleAchievement.transform.Find("Slider").GetComponent<Slider>().value = 1;
            }
            else
            {
                sampleAchievement.transform.Find("Slider").GetComponent<Slider>().value = 1;
                sampleAchievement.transform.Find("CollectButton").GetComponent<Button>().enabled = true;
                sampleAchievement.transform.Find("CollectButton").GetComponent<Image>().enabled = true;
                sampleAchievement.transform.Find("CollectButton").GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                sampleAchievement.transform.Find("CollectButton").GetComponent<Button>().onClick.AddListener(() => { CollectFirstTime(); });

            }
        }
        else
        {
            sampleAchievement.transform.Find("CollectButton").GetComponent<Button>().enabled = false;
            sampleAchievement.transform.Find("CollectButton").GetComponent<Image>().enabled = false;
            sampleAchievement.transform.Find("CollectButton").GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            sampleAchievement.transform.Find("Slider").GetComponent<Slider>().value = 0;

        }

    }

    void CreateAchievementsUI()
    {

        int index = 0;
        foreach (var achievement in MetaData.ConstVariable.Achievements.achievements)
        {
            if (!achievement.completed && PlayerPrefs.GetInt(achievement.key) == 0)
            {

                float value = 0;
                switch (achievement.type)
                {
                    case MetaData.ConstVariable.Achievements.Type.Play:
                        value = (float)AchievementsManager.Instance.PlayTimes / (float)achievement.times;
                        break;
                    case MetaData.ConstVariable.Achievements.Type.Jump:
                        value = (float)AchievementsManager.Instance.JumpTimes / (float)achievement.times;
                        break;
                    case MetaData.ConstVariable.Achievements.Type.Die:
                        value = (float)AchievementsManager.Instance.DieTimes / (float)achievement.times;
                        break;
                    case MetaData.ConstVariable.Achievements.Type.Login:
                        value = (float)AchievementsManager.Instance.LoginTimes / (float)achievement.times;
                        break;
                    case MetaData.ConstVariable.Achievements.Type.Reach:
                        value = (float)GameManagerIngame.Instance.Score / (float)achievement.times;
                        break;
                    case MetaData.ConstVariable.Achievements.Type.Revive:
                        value = (float)AchievementsManager.Instance.ReviveTimes / (float)achievement.times;
                        break;
                    case MetaData.ConstVariable.Achievements.Type.JumpInLevel:
                        value = (float)AchievementsManager.Instance.JumpTimesInCurrentLevel / (float)achievement.times;
                        break;
                    case MetaData.ConstVariable.Achievements.Type.PlayTime:
                        value = (float)AchievementsManager.Instance.GamePlayTime / (float)achievement.times;
                        break;
                    case MetaData.ConstVariable.Achievements.Type.DieInLevel:
                        value = (float)AchievementsManager.Instance.DieTimesInCurrentLevel / (float)achievement.times;
                        break;
                    default:
                        break;
                }

                if (value >= 1 && !achievement.completed)
                {
                    var obj = Instantiate(sampleAchievement, content.transform);
                    obj.gameObject.SetActive(true);
                    obj.CollectButton.enabled = false;
                    obj.CollectButton.image.enabled = false;
                    obj.ChieldCollectButton.enabled = false;

                    RectTransform objTransform = obj.GetComponent<RectTransform>();
                    achievementsTransforms.Add(objTransform);
                    objTransform.localPosition = new Vector3(0, -350 + (-250 * (index + 1)), 0);

                    if (achievement.reward.type == MetaData.ConstVariable.Achievements.RewardType.AlfuCoin)
                    {
                        obj.transform.Find("AlfuCoinAmount").GetComponent<TextMeshProUGUI>().text = achievement.reward.value.ToString();
                    }
                    else if (achievement.reward.type == MetaData.ConstVariable.Achievements.RewardType.Character)
                    {
                        obj.AlfuCoinAmount.text = "";
                        obj.SkinImage.enabled = true;
                        obj.SkinImage.sprite = Resources.Load<Sprite>(MetaData.ConstVariable.Character.Find(achievement.reward.value.ToString()).iconFilePath);
                        obj.AlfuCoinImage.enabled = false;
                    }

                    obj.AchievementText.text = achievement.name;
                    obj.CollectButton.onClick.RemoveAllListeners();
                    obj.CollectButton.onClick.AddListener(() => { CollectAchievement(achievement.key, obj.transform.Find("CollectButton").GetComponent<Button>()); });

                    if (achievement.completed)
                    {
                        obj.CollectButton.GetComponent<Button>().enabled = false;
                        obj.CollectButton.GetComponent<Image>().enabled = false;
                        obj.CollectButton.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
                    }

                    obj.transform.Find("Slider").GetComponent<Slider>().value = value;

                    obj.transform.Find("CollectButton").GetComponent<Button>().interactable = true;

                    if (achievement.reward.type == MetaData.ConstVariable.Achievements.RewardType.Character)
                    {
                        PlayerPrefs.SetInt(achievement.key, 1);
                        achievement.completed = true;
                        obj.CollectButton.gameObject.SetActive(false);
                        obj.SkinImage.enabled = true;
                        obj.SkinImage.sprite = Resources.Load<Sprite>(MetaData.ConstVariable.Character.Find(achievement.reward.value.ToString()).iconFilePath);
                        obj.AlfuCoinImage.enabled = false;
                        isThereUnlockedSkin = true;
                        PlayerPrefs.SetInt(achievement.reward.value.ToString(), 1);
                        unlockedSkinKey = achievement.reward.value.ToString();
                    }
                    else
                    {
                        isThereCompleted = true;
                        obj.transform.Find("CollectButton").GetComponent<Button>().enabled = true;
                        obj.transform.Find("CollectButton").GetComponent<Image>().enabled = true;
                        obj.transform.Find("CollectButton").GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                    }
                    index++;

                }
            }

        }
        foreach (var achievement in MetaData.ConstVariable.Achievements.achievements)
        {
            float value = 0;

            switch (achievement.type)
            {
                case MetaData.ConstVariable.Achievements.Type.Play:
                    value = (float)AchievementsManager.Instance.PlayTimes / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.Jump:
                    value = (float)AchievementsManager.Instance.JumpTimes / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.Die:
                    value = (float)AchievementsManager.Instance.DieTimes / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.Login:
                    value = (float)AchievementsManager.Instance.LoginTimes / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.Reach:
                    value = (float)GameManagerIngame.Instance.Score / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.Revive:
                    value = (float)AchievementsManager.Instance.ReviveTimes / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.JumpInLevel:
                    value = (float)AchievementsManager.Instance.JumpTimesInCurrentLevel / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.PlayTime:
                    value = (float)AchievementsManager.Instance.GamePlayTime / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.DieInLevel:
                    value = (float)AchievementsManager.Instance.DieTimesInCurrentLevel / (float)achievement.times;
                    break;
                default:
                    break;
            }

            if (value < 1)
            {
                var obj = Instantiate(sampleAchievement, content.transform);
                obj.gameObject.SetActive(true);
                obj.CollectButton.enabled = false;
                obj.CollectButton.image.enabled = false;
                obj.ChieldCollectButton.enabled = false;

                RectTransform objTransform = obj.GetComponent<RectTransform>();
                achievementsTransforms.Add(objTransform);
                objTransform.localPosition = new Vector3(0, -350 + (-250 * (index + 1)), 0);

                if (achievement.reward.type == MetaData.ConstVariable.Achievements.RewardType.AlfuCoin)
                {
                    obj.AlfuCoinAmount.text = achievement.reward.value.ToString();
                }
                else if (achievement.reward.type == MetaData.ConstVariable.Achievements.RewardType.Character)
                {
                    obj.AlfuCoinAmount.text = "";
                    obj.SkinImage.enabled = true;
                    obj.AlfuCoinImage.enabled = false;

                    var v = MetaData.ConstVariable.Character.characters.FirstOrDefault(x => x.key == achievement.reward.value.ToString());
                    if (v != null)
                    {
                        Sprite sp = Resources.Load<Sprite>(v.iconFilePath);
                        if (sp != null)
                            obj.SkinImage.sprite = sp;
                    }
                    obj.AlfuCoinAmount.enabled = false;
                }

                obj.AchievementText.text = achievement.name;
                obj.CollectButton.onClick.AddListener(() => { CollectAchievement(achievement.key, obj.CollectButton); });
                obj.Slider.value = value;

                if (achievement.completed)
                {
                    obj.CollectButton.enabled = false;
                    obj.CollectButton.enabled = false;
                    obj.ChieldCollectButton.enabled = false;
                }

                if (value >= 1 && !achievement.completed)
                {
                    obj.CollectButton.interactable = true;

                    if (achievement.reward.type == MetaData.ConstVariable.Achievements.RewardType.Character)
                    {
                        PlayerPrefs.SetInt(achievement.key, 1);
                        achievement.completed = true;
                        obj.CollectButton.gameObject.SetActive(false);
                        obj.SkinImage.enabled = true;
                        obj.SkinImage.sprite = Resources.Load<Sprite>(MetaData.ConstVariable.Character.Find(achievement.reward.value.ToString()).iconFilePath);
                        obj.AlfuCoinImage.enabled = false;
                        isThereUnlockedSkin = true;
                        PlayerPrefs.SetInt(achievement.reward.value.ToString(), 1);
                        unlockedSkinKey = achievement.reward.value.ToString();
                    }
                    else
                    {
                        isThereCompleted = true;
                        obj.CollectButton.enabled = true;
                        obj.CollectButton.image.enabled = true;
                        obj.ChieldCollectButton.enabled = true;
                    }
                }

                index++;
            }
        }
        foreach (var achievement in MetaData.ConstVariable.Achievements.achievements)
        {
            float value = 0;
            switch (achievement.type)
            {
                case MetaData.ConstVariable.Achievements.Type.Play:
                    value = (float)AchievementsManager.Instance.PlayTimes / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.Jump:
                    value = (float)AchievementsManager.Instance.JumpTimes / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.Die:
                    value = (float)AchievementsManager.Instance.DieTimes / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.Login:
                    value = (float)AchievementsManager.Instance.LoginTimes / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.Reach:
                    value = (float)GameManagerIngame.Instance.Score / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.Revive:
                    value = (float)AchievementsManager.Instance.ReviveTimes / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.JumpInLevel:
                    value = (float)AchievementsManager.Instance.JumpTimesInCurrentLevel / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.PlayTime:
                    value = (float)AchievementsManager.Instance.GamePlayTime / (float)achievement.times;
                    break;
                case MetaData.ConstVariable.Achievements.Type.DieInLevel:
                    value = (float)AchievementsManager.Instance.DieTimesInCurrentLevel / (float)achievement.times;
                    break;
                default:
                    break;
            }

            if (value >= 1 && achievement.completed)
            {
                var obj = Instantiate(sampleAchievement, content.transform);
                obj.gameObject.SetActive(true);
                obj.CollectButton.enabled = false;
                obj.CollectButton.image.enabled = false;
                obj.ChieldCollectButton.enabled = false;

                RectTransform objTransform = obj.GetComponent<RectTransform>();
                achievementsTransforms.Add(objTransform);
                objTransform.localPosition = new Vector3(0, -350 + (-250 * (index + 1)), 0);

                if (achievement.reward.type == MetaData.ConstVariable.Achievements.RewardType.AlfuCoin)
                {
                    obj.AlfuCoinAmount.text = achievement.reward.value.ToString();
                }
                else if (achievement.reward.type == MetaData.ConstVariable.Achievements.RewardType.Character)
                {
                    obj.AlfuCoinAmount.text = "";
                    obj.SkinImage.enabled = true;
                    obj.SkinImage.sprite = Resources.Load<Sprite>(MetaData.ConstVariable.Character.Find(achievement.reward.value.ToString()).iconFilePath);
                    obj.AlfuCoinImage.enabled = false;
                }

                obj.AchievementText.text = achievement.name;
                obj.CollectButton.onClick.AddListener(() => { CollectAchievement(achievement.key, obj.transform.Find("CollectButton").GetComponent<Button>()); });
                obj.Slider.value = value;

                if (achievement.completed)
                {
                    obj.CollectButton.gameObject.SetActive(false);
                    obj.CollectButton.image.enabled = true;
                    obj.ChieldCollectButton.enabled = true;

                }

                if (value >= 1 && !achievement.completed)
                {
                    obj.transform.Find("CollectButton").GetComponent<Button>().interactable = true;

                    if (achievement.reward.type == MetaData.ConstVariable.Achievements.RewardType.Character)
                    {
                        PlayerPrefs.SetInt(achievement.key, 1);
                        achievement.completed = true;
                        obj.CollectButton.gameObject.SetActive(false);
                        obj.SkinImage.GetComponent<Image>().enabled = true;
                        obj.SkinImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(MetaData.ConstVariable.Character.Find(achievement.reward.value.ToString()).iconFilePath);
                        obj.AlfuCoinImage.enabled = false;
                        isThereUnlockedSkin = true;
                        PlayerPrefs.SetInt(achievement.reward.value.ToString(), 1);
                        unlockedSkinKey = achievement.reward.value.ToString();
                    }
                    else
                    {
                        isThereCompleted = true;
                        obj.CollectButton.enabled = true;
                        obj.CollectButton.image.enabled = true;
                        obj.ChieldCollectButton.enabled = true;
                    }
                }

                index++;
            }
        }

        content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, index * 250 + 550);
    }

    void CollectFirstTime()
    {
        ShopManager.Instance.AddAlfuCoin(50);
        PlayerPrefs.SetInt("PlayFirstTimeCollected", 1);
        sampleAchievement.CollectButton.gameObject.SetActive(false);
    }

    void CheckDailyMission()
    {
        dailyMissions.CollectButton.enabled = false;
        dailyMissions.CollectButton.image.enabled = false;
        dailyMissions.CollectButtonText.enabled = false;


        int day = PlayerPrefs.GetInt("DailyMissionDay");
        dailyMissions.DailyMissionText.text = MetaData.ConstVariable.DailyMission.missions[day - 1].name;
        dailyMissions.AlfuCoinAmount.text = MetaData.ConstVariable.DailyMission.missions[day - 1].reward.value.ToString();

        for (int i = 0; i < Mathf.Clamp(PlayerPrefs.GetInt("DailyMissionTimes"), 0, 5); i++)
        {
            dailyMissionCircles[i].SetActive(true);
            if (i == 4 && PlayerPrefs.GetInt("DailyMissionCollected") == 0)
            {
                dailyMissions.CollectButton.enabled = true;
                dailyMissions.CollectButton.image.enabled = true;
                dailyMissions.CollectButtonText.enabled = true;
                dailyMissions.CollectButton.interactable = true;
                isThereCompleted = true;
            }
        }

        if (PlayerPrefs.GetInt("DailyMissionCollected") == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                dailyMissionCircles[i].SetActive(true);
                dailyMissions.Skip.gameObject.SetActive(false);
            }

        }

    }

    public void CollectDailyMission()
    {
        dailyMissions.CollectButton.gameObject.SetActive(false);
        dailyMissions.Skip.gameObject.SetActive(false);
        PlayerPrefs.SetInt("DailyMissionTimes", 0);
        PlayerPrefs.SetInt("DailyMissionCollected", 1);
        PlayerPrefs.SetInt("DailyMissionDateDay", DateTime.Now.Day);
        coins[0].transform.position = dailyMissions.CollectButton.transform.position;
        CollectAnimation((int)MetaData.ConstVariable.DailyMission.missions[PlayerPrefs.GetInt("DailyMissionDay") - 1].reward.value);
        ServerData.Instance.SaveDailyMissionInfo(PlayerPrefs.GetInt("DailyMissionDay"));
    }

    public void SkipDailyMission()
    {
        AdsManager.instance.rewardAdsTypes = AdsManager.RewardAdsTypes.SkipDailyMission;
        AdsManager.instance.ShowRewardedAd();
        CheckDailyMission();
    }

    private void CollectAchievement(string key, Button btn)
    {
        PlayerPrefs.SetInt(key, 1);
        MetaData.ConstVariable.Achievements.Find(key).completed = true;
        btn.gameObject.SetActive(false);
        coins[0].transform.position = btn.transform.position;
        CollectAnimation((int)MetaData.ConstVariable.Achievements.Find(key).reward.value);
        ServerData.Instance.SaveAchievement(key);
    }

    private void CollectAnimation(int value)
    {
        StartCoroutine(ExecuteCollectAnimation(-Camera.main.ScreenToWorldPoint(coinTransform.position), value));
    }

    public GameObject target;
    IEnumerator ExecuteCollectAnimation(Vector3 pos, int value)
    {
        //target.transform.position = pos;
        ParticleSystem particle = collectableCoins.GetComponent<ParticleSystem>();

        particle.Play();

        yield return new WaitForSeconds(1.6f);

        ShopManager.Instance.AddAlfuCoin(value);
    }

    private void CheckUnlockedSkin()
    {
        if (isThereUnlockedSkin)
        {
            FindObjectOfType<UIManager>().HidePlayButton();

            newUnlockedSkinPanel.SetActive(true);
            newUnlockedSkinPanel.transform.Find("SkinImage").GetComponent<Image>().sprite = Resources.Load<Sprite>(MetaData.ConstVariable.Character.Find(unlockedSkinKey).iconFilePath);
        }
    }

    public void EquipSkin()
    {
        PlayerPrefs.SetString("SelectedCharacter", unlockedSkinKey);
        ServerData.Instance.SaveSkinInfo(unlockedSkinKey);
        CloseUnlockedSkinPanel();
        EconomyScript.Instance.SetSkinToDataPlayer(unlockedSkinKey);
        FindObjectOfType<UIManager>().ShowPlayButton();

    }

    public void CloseUnlockedSkinPanel()
    {
        FindObjectOfType<UIManager>().ShowPlayButton();

        newUnlockedSkinPanel.SetActive(false);
    }

    public void Open()
    {
        FindObjectOfType<UIManager>().HidePlayButton();
        //coinTransform.SetParent(panel.transform);
        isThereCompleted = false;
        notifyImage.SetActive(false);
        panel.SetActive(true);

        PlayerLog.Instance.IncreaseAchievementsButtonCounter();

        if (!achievementCreated)
        {
            foreach (var item in achievementsTransforms)
            {
                item.localPosition = new Vector3(sampleAchievement.GetComponent<RectTransform>().localPosition.x, item.localPosition.y, 0);
            }
            achievementCreated = true;
        }
    }

    public void Close()
    {
        panel.SetActive(false);
        //coinTransform.SetParent(mainCanvas.transform);
        FindObjectOfType<UIManager>().ShowPlayButton();
    }

    public void Back()
    {
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.MainScene);
    }
}
