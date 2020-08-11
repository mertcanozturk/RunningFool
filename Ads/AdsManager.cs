using System;
using UnityEngine;
using GoogleMobileAds.Api;

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class AdsManager : MonoBehaviour // Singleton<AdsManager>
{
    public enum RewardAdsTypes
    {
        SkipLevel,
        SkipDailyMission,
        EarnCoin10,
        EarnCoin20,
        Revive,
        NoReward,
        WatchAdsWithSkin
    }
    private BannerView bannerView;

    public RewardAdsTypes rewardAdsTypes;
    private InterstitialAd interstitial;
    public RewardedAd rewardedAd;
    private float deltaTime = 0.0f;
    private static string outputMessage = string.Empty;
    public static AdsManager instance;


    public bool isTest = true;
    public static string OutputMessage
    {
        set { outputMessage = value; }
    }

    public void Start()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-1727878865389203~9561236341";
#elif UNITY_IPHONE
        string appId = "ca-app-pub-1727878865389203~9206298533";
#else
        string appId = "unexpected_platform";
#endif



        MobileAds.Initialize(appId);

        CreateAndLoadRewardedAd();

        // RequestInterstitial();

    }

    private void Awake()
    {
        MakeSingleten();



        MobileAds.Initialize(initStatus => { });
    }

    void MakeSingleten()
    {

        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }


    }

    // Returns an ad request with custom ad targeting.
    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            //.AddTestDevice(AdRequest.TestDeviceSimulator)
            //.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            //.AddKeyword("game")
            //.SetGender(Gender.Male)
            //.SetBirthday(new DateTime(1985, 1, 1))
            //.TagForChildDirectedTreatment(false)
            //.AddExtra("color_bg", "9B30FF")
            .Build();
    }

    public void DestroyBanner()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
            //  bannerView.Destroy();
        }

    }

    public void RequestInterstitial()
    {
        // These ad units are configured to always serve test ads.




        string adUnitId = "unused";




#if UNITY_ANDROID



        if (isTest)
        {

            adUnitId = "ca-app-pub-3940256099942544/8691691433";


        }
        else
        {
            adUnitId = "ca-app-pub-1727878865389203/1710951899";
        }

#elif UNITY_IPHONE
        if (isTest)
        {

             adUnitId = "ca-app-pub-3940256099942544/8691691433";
          

        }
        else
        {
             adUnitId = "ca-app-pub-1727878865389203/9014726847";
        }

        
#endif

        // Clean up interstitial ad before creating a new one.
        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        // Create an interstitial.
        this.interstitial = new InterstitialAd(adUnitId);

        // Register for ad events.
        this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
        this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
        this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
        this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
        this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

        // Load an interstitial ad.
        this.interstitial.LoadAd(this.CreateAdRequest());
    }

    public void CreateAndLoadRewardedAd()
    {
        Debug.Log("Requested Rewarded");

        string adUnitId = "";
#if UNITY_ANDROID


        if (isTest)
        {

            adUnitId = "ca-app-pub-3940256099942544/5224354917";


        }
        else
        {
            adUnitId = "ca-app-pub-1727878865389203/6771706881";
        }

#elif UNITY_IPHONE

        if (isTest)
        {

            adUnitId = "ca-app-pub-3940256099942544/5224354917";


        }
        else
        {
            adUnitId = "ca-app-pub-1727878865389203/5422132045";
        }

#endif
        // Create new rewarded ad instance.



        if (rewardedAd != null)
        {

            if (!rewardedAd.IsLoaded())
            {



                // Create new rewarded ad instance.


                this.rewardedAd = new RewardedAd(adUnitId);

                // Called when an ad request has successfully loaded.
                this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
                // Called when an ad request failed to load.
                this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
                // Called when an ad is shown.
                this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
                // Called when an ad request failed to show.
                this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
                // Called when the user should be rewarded for interacting with the ad.
                this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
                // Called when the ad is closed.
                this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

                // Create an empty ad request.
                AdRequest request = this.CreateAdRequest();
                // Load the rewarded ad with the request.
                this.rewardedAd.LoadAd(request);


            }

        }
        else
        {

            // Create new rewarded ad instance.


            this.rewardedAd = new RewardedAd(adUnitId);

            // Called when an ad request has successfully loaded.
            this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            // Called when an ad request failed to load.
            this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            // Called when an ad is shown.
            this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
            // Called when an ad request failed to show.
            this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            // Called when the user should be rewarded for interacting with the ad.
            this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            // Called when the ad is closed.
            this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

            // Create an empty ad request.
            AdRequest request = this.CreateAdRequest();
            // Load the rewarded ad with the request.
            this.rewardedAd.LoadAd(request);
        }


    }

    public void RequestBanner()
    {
        if (GameManager.instance.adsHelper.isShowBanner())
        {
            if (GameManager.Scurity.Checking(MetaData.ConstVariable.PlayFabPlayer.NoAds))
            {
                if (GameManager.Scurity.Reading(MetaData.ConstVariable.PlayFabPlayer.NoAds) == "true")
                    return;
            }


            System.Random random = new System.Random();
            if (random.Next(0, 100) > GameManager.instance.persantageBanner)
                return;

            PlayerLog.Instance.IncreaseBannerAdsCounter();

            // These ad units are configured to always serve test ads.

            string adUnitId = "unused";
#if UNITY_ANDROID

            if (isTest)
            {

                adUnitId = "ca-app-pub-3940256099942544/6300978111";


            }
            else
            {
                adUnitId = "ca-app-pub-1727878865389203/8654514637";
            }

#elif UNITY_IPHONE
        if (isTest)
        {

            adUnitId = "ca-app-pub-3940256099942544/6300978111";


        }
        else
        {
            adUnitId = "ca-app-pub-1727878865389203/2640890180";
        }


#endif

            // Clean up banner ad before creating a new one.
            if (this.bannerView != null)
            {
                this.bannerView.Destroy();
            }

            AdSize adaptiveSize =
                            AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            this.bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

            // Register for ad events.
            this.bannerView.OnAdLoaded += this.HandleAdLoaded;
            this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
            this.bannerView.OnAdOpening += this.HandleAdOpened;
            this.bannerView.OnAdClosed += this.HandleAdClosed;
            this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

            // Load a banner ad.
            this.bannerView.LoadAd(this.CreateAdRequest());
        }
        GameManager.instance.adsHelper.OnShowBanner();
    }

    public void ShowWinInterstitial()
    {
        if (GameManager.Scurity.Checking(MetaData.ConstVariable.PlayFabPlayer.NoAds))
        {
            if (GameManager.Scurity.Reading(MetaData.ConstVariable.PlayFabPlayer.NoAds) == "true")
                return;
        }

        if (GameManager.instance.adsHelper.isShowInterstatial())
        {
            if (this.interstitial.IsLoaded())
            {

                GameManager.instance.SoundManager.PauseMainMusic();

                this.interstitial.Show();
                PlayerLog.Instance.IncreaseInterstitialAdsCounter();
            }
            else
            {
                Debug.Log("Interstitial is not ready yet");
            }
            RequestInterstitial();

        }
        GameManager.instance.adsHelper.OnShowInterstatial();
    }

    public void ShowLoseInterstitial()
    {
        if (GameManager.Scurity.Checking(MetaData.ConstVariable.PlayFabPlayer.NoAds))
        {
            if (GameManager.Scurity.Reading(MetaData.ConstVariable.PlayFabPlayer.NoAds) == "true")
                return;
        }

        if (GameManager.instance.adsHelper.isShowLoseInterstatial())
        {
            if (this.interstitial.IsLoaded())
            {
                GameManager.instance.SoundManager.PauseMainMusic();

                this.interstitial.Show();
                PlayerLog.Instance.IncreaseInterstitialAdsCounter();
            }
            else
            {
                Debug.Log("Interstitial is not ready yet");
            }
            RequestInterstitial();

        }
        GameManager.instance.adsHelper.OnShowLoseInterstatial();
    }

    public void ShowRetryInterstitial()
    {
        if (GameManager.Scurity.Checking(MetaData.ConstVariable.PlayFabPlayer.NoAds))
        {
            if (GameManager.Scurity.Reading(MetaData.ConstVariable.PlayFabPlayer.NoAds) == "true")
                return;
        }

        if (GameManager.instance.adsHelper.isShowRetryInterstatial())
        {
            if (this.interstitial.IsLoaded())
            {
                GameManager.instance.SoundManager.PauseMainMusic();
                this.interstitial.Show();
                PlayerLog.Instance.IncreaseInterstitialAdsCounter();
            }
            else
            {
                Debug.Log("Interstitial is not ready yet");
            }
            RequestInterstitial();

        }
        GameManager.instance.adsHelper.OnShowRetryInterstatial();
    }



    public void ShowRewardedAd()
    {
        if (GameManager.instance.adsHelper.isShowReward())
        {
            if (this.rewardedAd.IsLoaded())
            {
                System.Random random = new System.Random();
                if (random.Next(0, 100) > GameManager.instance.persantageReward)
                    return;

                GameManager.instance.SoundManager.PauseMainMusic();

                this.rewardedAd.Show();
                PlayerLog.Instance.IncreaseRewardedCounter();
            }

            CreateAndLoadRewardedAd();
        }
        GameManager.instance.adsHelper.OnShowReward();
    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
        MonoBehaviour.print(String.Format("Ad Height: {0}, width: {1}",
            this.bannerView.GetHeightInPixels(),
            this.bannerView.GetWidthInPixels()));
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
                "HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeftApplication event received");
    }

    #endregion

    #region Interstitial callback handlers

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        print("HandleInterstitialLoaded event received");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        print(
            "HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }

    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        print("HandleInterstitialOpened event received");

        switch (rewardAdsTypes)
        {
            case RewardAdsTypes.SkipLevel:
                GameManagerIngame.Instance.LevelManager.SkipLevel();
                GameManagerIngame.Instance.LevelManager.HideFailedPanel();
                break;
            case RewardAdsTypes.EarnCoin10:
                ShopManager.Instance.AddAlfuCoin(10);
                break;
            case RewardAdsTypes.EarnCoin20:
                ShopManager.Instance.AddAlfuCoin(20);
                break;
            case RewardAdsTypes.Revive:
                FindObjectOfType<LevelManager>().Revive();
                FindObjectOfType<LevelManager>().levelFailedPanel.SetActive(false);
                PlayerLog.Instance.IncreaseReviveButtonCounter();
                AchievementsManager.Instance.IncreaseReviveTimes();
                break;
            case RewardAdsTypes.SkipDailyMission:
                ShopManager.Instance.AddAlfuCoin((int)MetaData.ConstVariable.DailyMission.missions[PlayerPrefs.GetInt("DailyMissionDay") - 1].reward.value);
                PlayerPrefs.SetInt("DailyMissionTimes", 0);
                PlayerPrefs.SetInt("DailyMissionCollected", 1);
                break;
            case RewardAdsTypes.WatchAdsWithSkin:
                GameManagerIngame.Instance.watchAdsWithSkin -= 1;
                break;
            default:
                break;
        }

        rewardAdsTypes = RewardAdsTypes.NoReward;


    }

    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        print("HandleInterstitialClosed event received");
        GameManager.instance.SoundManager.ContinueMainMusic();

    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        GameManager.instance.SoundManager.ContinueMainMusic();

        print("HandleInterstitialLeftApplication event received");
    }



    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");



    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: " + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: " + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        GameManager.instance.SoundManager.ContinueMainMusic();

        MonoBehaviour.print("HandleRewardedAdClosed event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);

        switch (rewardAdsTypes)
        {
            case RewardAdsTypes.SkipLevel:
                GameManagerIngame.Instance.LevelManager.SkipLevel();
                GameManagerIngame.Instance.LevelManager.HideFailedPanel();
                break;
            case RewardAdsTypes.EarnCoin10:
                ShopManager.Instance.AddAlfuCoin(10);
                break;
            case RewardAdsTypes.EarnCoin20:
                ShopManager.Instance.AddAlfuCoin(20);
                break;
            case RewardAdsTypes.Revive:
                FindObjectOfType<LevelManager>().Revive();
                FindObjectOfType<LevelManager>().levelFailedPanel.SetActive(false);
                PlayerLog.Instance.IncreaseReviveButtonCounter();
                AchievementsManager.Instance.IncreaseReviveTimes();
                break;
            case RewardAdsTypes.SkipDailyMission:
                ShopManager.Instance.AddAlfuCoin((int)MetaData.ConstVariable.DailyMission.missions[PlayerPrefs.GetInt("DailyMissionDay") - 1].reward.value);
                PlayerPrefs.SetInt("DailyMissionTimes", 0);
                PlayerPrefs.SetInt("DailyMissionCollected", 1);
                break;
            case RewardAdsTypes.WatchAdsWithSkin:
                GameManagerIngame.Instance.watchAdsWithSkin -= 1;
                break;
            default:
                break;
        }
        GameManager.instance.SoundManager.ContinueMainMusic();

        rewardAdsTypes = RewardAdsTypes.NoReward;
    }

    #endregion
}