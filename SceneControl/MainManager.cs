using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;
    [SerializeField] private TextMeshProUGUI alfuCoinText;
    [SerializeField] private TextMeshProUGUI alfuCoinText2;
    [SerializeField] private TextMeshProUGUI alfuCoinText3;
    [SerializeField] private GameObject confirmationMessagePanel;
    [SerializeField] private GameObject skinNotification;
    [SerializeField] private GameObject watchAdsToUnlockSkinPanel, skinUnlockedPanel;
    [SerializeField] private GameObject playButton, onlineButton, contestButton;
    [SerializeField] private TextMeshProUGUI watchadsCount;
    [SerializeField] private SkinnedMeshRenderer[] playerMeshes;
    [SerializeField] public TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject mainFacebookButton, AchievementsButton, settingsConnectButton, settingsDisconnectButton;

    int skinId;

    string hash;
    string gustid;
    public InputField UsernameField;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        AdsManager.instance.RequestBanner();
        UserNamePlayer();
        InitializerManager.instance.gamelogo.gameObject.SetActive(false);
        InitializerManager.instance.Container.gameObject.SetActive(false);
        CheckBuyableSkin();
        if (!GameManager.instance.SoundManager.MainSound.isPlaying && Settings.Instance.musicEnabled)
        {
            GameManager.instance.SoundManager.MainSound.Play();
            GameManager.instance.SoundManager.MainSound.loop = true;
        }

        PlayerPrefs.SetInt("MainOpenCount", PlayerPrefs.GetInt("MainOpenCount") + 1);

        if (PlayerPrefs.GetInt("MainOpenCount") > 1)
        {
            if (!skinUnlockedPanel.activeSelf)
                StartCoroutine(RandomWatchAdsUnlockSkin());
        }

        if (SyncSceneScript.SyncSceneObject)
            Destroy(SyncSceneScript.SyncSceneObject.gameObject);

        CheckFacebookConnection();
    }



    public void FacebookConnect()
    {
        GameManager.instance.SoundManager.ButtonSound.Play();
        FaceBookScript.Instance.FacebookLogin();
    }
    public void FacebookDisconnect()
    {
        GameManager.instance.SoundManager.ButtonSound.Play();
        GameManager.Scurity.DeleteData(MetaData.ConstVariable.FilesSetting.IsFBLogin);
        GameManager.Scurity.DeleteData(MetaData.ConstVariable.PlayFabPlayer.FacebookNameWithId);
        GameManager.Scurity.DeleteData(MetaData.ConstVariable.PlayFabPlayer.HashFaceBookId);
        GameManager.Scurity.DeleteData(MetaData.ConstVariable.PlayFabPlayer.FacebookId);
        GameManager.Scurity.DeleteData(MetaData.ConstVariable.PlayFabPlayer.FacebookName);
        FaceBookScript.Instance.FacebookLogout();       
        settingsConnectButton.SetActive(true);
        settingsDisconnectButton.SetActive(false);
        Application.Quit(); 
    }

    public void CheckFacebookConnection()
    {
        if (GameManager.Scurity.Checking(MetaData.ConstVariable.FilesSetting.IsFBLogin))
        {
            AchievementsButton.transform.position = mainFacebookButton.transform.position;
            settingsConnectButton.GetComponent<Button>().interactable = false;
           // settingsDisconnectButton.SetActive(true);
            mainFacebookButton.SetActive(false);
        }
        else
        {
            settingsConnectButton.SetActive(true);
            settingsDisconnectButton.SetActive(false);
        }
    }


    IEnumerator RandomWatchAdsUnlockSkin()
    {
        yield return new WaitForSeconds(1f);

        if (!skinUnlockedPanel.activeSelf)
        {
            int rate = UnityEngine.Random.Range(0, 100);

            if (rate < 15)
            {
                GameManagerIngame.Instance.watchAdsWithSkin = 3;

                playButton.SetActive(false);
                onlineButton.SetActive(false);
                contestButton.SetActive(false);
                watchAdsToUnlockSkinPanel.SetActive(true);
                bool usableSkin = false;
                while (!usableSkin)
                {
                    skinId = UnityEngine.Random.Range(2, MetaData.ConstVariable.Character.characters.Length);
                    if (PlayerPrefs.GetInt(MetaData.ConstVariable.Character.characters[skinId].key) != 1)
                    {
                        usableSkin = true;
                    }
                }

                watchAdsToUnlockSkinPanel.transform.Find("SkinImage").GetComponent<Image>().sprite =
                    Resources.Load<Sprite>(MetaData.ConstVariable.Character.characters[skinId].iconFilePath);

            }
        }

    }

    public void WatchAdsForSkin()
    {
        AdsManager.instance.rewardAdsTypes = AdsManager.RewardAdsTypes.WatchAdsWithSkin;
        AdsManager.instance.ShowRewardedAd();
        PlayerLog.Instance.IncreaseWatchAdsForSkinAdsCounter();
    }

    public void UserNamePlayer()
    {
        hash = GameManager.Scurity.GetHash(AlfucodeAPIServer.instance.InfoPlayerPayload.AccountInfo.PlayFabId);
        hash = hash.Substring(Math.Max(0, hash.Length - 5));
        gustid = string.Format("Guest_{0}", hash);

        if (!PlayerPrefs.HasKey(MetaData.ConstVariable.PlayFabPlayer.PlayerName))
        {
            if (string.IsNullOrEmpty(AlfucodeAPIServer.instance.InfoPlayerPayload.PlayerProfile.DisplayName))
            {
                UsernameField.text = gustid;
                playerNameText.text = gustid;
                //ShowMessageCreateOrLoadAccount();
                if (string.IsNullOrEmpty(AlfucodeAPIServer.instance.InfoPlayerPayload.PlayerProfile.DisplayName))
                    SaveButton_Onclick();
            }
            else
            {
                UsernameField.text = string.Format(AlfucodeAPIServer.instance.InfoPlayerPayload.PlayerProfile.DisplayName);
                playerNameText.text = string.Format(AlfucodeAPIServer.instance.InfoPlayerPayload.PlayerProfile.DisplayName); ;

                //if (AlfucodeAPIServer.instance.InfoPlayerPayload.PlayerProfile.DisplayName != gustid)
                // EnableDisableInput();
            }
        }
        else
        {
            UsernameField.text = GameManager.Player.NamePlayer;
            playerNameText.text = GameManager.Player.NamePlayer;
            if (!string.IsNullOrEmpty(GameManager.Player.NamePlayer) && GameManager.Player.NamePlayer != gustid)
            {
                //EnableDisableInput();
            }
            else
            {
                UsernameField.text = string.Format("Guest_{0}", hash);
                SaveButton_Onclick();
            }
        }
    }

    void SaveButton_Onclick()
    {
        if (UsernameField.text.Length > 2 && GameManager.Player.NamePlayer != UsernameField.text)
        {
            AlfucodeAPIServer.instance.SetUserNamePlayer(UsernameField.text);
        }
    }


    private void Update()
    {
        alfuCoinText.text = ShopManager.Instance.AlfuCoin.ToString();
        alfuCoinText2.text = ShopManager.Instance.AlfuCoin.ToString();
        alfuCoinText3.text = ShopManager.Instance.AlfuCoin.ToString();
        watchadsCount.text = "x" + GameManagerIngame.Instance.watchAdsWithSkin.ToString();

        if (GameManagerIngame.Instance.watchAdsWithSkin == 0)
        {
            ShopManager.Instance.BuyCharacter(MetaData.ConstVariable.Character.characters[skinId].key);
            ShopManager.Instance.AddAlfuCoin(MetaData.ConstVariable.Character.characters[skinId].price);
            PlayerPrefs.SetString("SelectedCharacter", MetaData.ConstVariable.Character.characters[skinId].key);
            foreach (var item in playerMeshes)
            {
                item.material = Resources.Load<Material>(MetaData.ConstVariable.Character.characters[skinId].materialPath);

            }
            playButton.SetActive(true);
            onlineButton.SetActive(true);
            contestButton.SetActive(true);
            watchAdsToUnlockSkinPanel.SetActive(false);
            GameManagerIngame.Instance.watchAdsWithSkin = 3;
            ServerData.Instance.SaveSkinInfo(MetaData.ConstVariable.Character.characters[skinId].key);
        }
    }

    public void CheckBuyableSkin()
    {
        foreach (var item in MetaData.ConstVariable.Character.characters)
        {
            if (item.price < ShopManager.Instance.AlfuCoin && PlayerPrefs.GetInt(item.key) == 0)
            {
                skinNotification.SetActive(true);
                break;
            }
        }
    }

    public void Play()
    {
        if (!PlayerPrefs.HasKey("CurrentLevel"))
        {
            PlayerPrefs.SetInt("CurrentLevel", 1);
            PlayerPrefs.SetString("OpenedWorld", MetaData.ConstVariable.Planet.planets[0].key);
            PlayerPrefs.SetString("OpenedJsonFile", MetaData.ConstVariable.Planet.planets[0].jsonFileName);
        }

        PlayerPrefs.SetInt("LevelSelected", 1);
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.GameScene);
    }

    public void ChangeMainMeshSkins(string key = "", int id = -1)
    {
        Material mat;
        if (id == -1 && key != "")
            mat = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(key).materialPath);
        else if (id != -1)
            mat = Resources.Load<Material>(MetaData.ConstVariable.Character.characters[id].materialPath);
        else
            mat = Resources.Load<Material>(MetaData.ConstVariable.Character.characters[0].materialPath);

        foreach (var item in playerMeshes)
        {
            item.material = mat;
        }
    }

    public void CloseWatchAdsToUnlockSkinPanel()
    {
        playButton.SetActive(true);
        onlineButton.SetActive(true);
        contestButton.SetActive(true);
        watchAdsToUnlockSkinPanel.SetActive(false);
        GameManagerIngame.Instance.watchAdsWithSkin = 3;
        PlayerLog.Instance.IncreaseWatchAdsForSkinCloseCounter();
    }


    public void OpenWorlds()
    {
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.PlanetsScene);
    }

    public void OpenShop()
    {
        if (watchAdsToUnlockSkinPanel.activeSelf == true)
            PlayerLog.Instance.IncreaseWatchAdsForSkinShopButtonCounter();
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.ShopScene);
    }

    public void Discord()
    {
        Application.OpenURL("https://discord.gg/gmn6W2A");
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        QuitGame();
    }
    public void Achievements()
    {
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.AchievementsScene);
    }

    public void QuitGameButton()
    {
        FindObjectOfType<UIManager>().HidePlayButton();
        confirmationMessagePanel.SetActive(true);
    }

    public void CloseConfirmationPanel()
    {
        FindObjectOfType<UIManager>().ShowPlayButton();

        confirmationMessagePanel.SetActive(false);
    }

    public void NoAdsButton()
    {
        AlfucodePurchaser.instance.BuyNoAds();
    }

    public void MultiPlayerButton()
    {
        GameManagerIngame.Instance.GameMode = GameManagerIngame.Mode.Multiplayer2;
        SceneManager.LoadScene("Connect");

    }

    public void QuitGame()
    {
        //PlayerPrefs.DeleteAll();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void VegaStar()
    {
#if UNITY_IOS
       Application.OpenURL("https://apps.apple.com/tr/app/vega-star/id1482251870");

#elif UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.vegastar.alfucode");
#else
         Application.OpenURL("https://play.google.com/store/apps/details?id=com.vegastar.alfucode");
#endif
    }
    public void NeonTarget()
    {
#if UNITY_IOS
       Application.OpenURL("https://apps.apple.com/us/app/neon-target-iq-game/id1487417180?ls=1");

#elif UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.alfucode.neontarget");
#else
         Application.OpenURL("https://play.google.com/store/apps/details?id=com.alfucode.neontarget");
#endif
    }
    public void FallingFool()
    {
#if UNITY_IOS
       Application.OpenURL("https://apps.apple.com/us/app/falling-fool/id1503687349");

#elif UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.alfucode.fallingfool");
#else
         Application.OpenURL("https://play.google.com/store/apps/details?id=com.alfucode.fallingfool");
#endif
    }

    public void FightingRunner()
    {
#if UNITY_IOS
       Application.OpenURL("https://apps.apple.com/tr/app/fighting-runner-multiplayer/id1512768514");

#elif UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.fightingrunner.alfucode");
#else
         Application.OpenURL("https://play.google.com/store/apps/details?id=com.fightingrunner.alfucode");
#endif
    }


}
