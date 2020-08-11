using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlfuCoinStore : MonoBehaviour
{
    public Button[] buttonNoAds;
    public Text textNoAds;
    public Button buttonPackage1;
    public Text textPackage1;
    public Button buttonPackage2;
    public Text textPackage2;
    public Button buttonPackage3;
    public Text textPackage3;

    public Button PromoCodeButton;
    public GameObject PromoPanel, ShopPanel;
    public TMPro.TMP_InputField promoText;
    public GameObject invalidPromoCode, codeverified;

    [SerializeField] GameObject achievementPanel;


    private void Start()
    {
        foreach (var item in buttonNoAds)
        {
            item.onClick.AddListener(() => { AlfucodePurchaser.instance.BuyNoAds(); });
        }
        textNoAds.text = AlfucodePurchaser.instance.SetValueItemsFromStore(MetaData.Package.NoAds);
        buttonPackage1.onClick.AddListener(() => { AlfucodePurchaser.instance.Buy(MetaData.Package.package1); });
        textPackage1.text = AlfucodePurchaser.instance.SetValueItemsFromStore(MetaData.Package.package1);
        buttonPackage2.onClick.AddListener(() => { AlfucodePurchaser.instance.Buy(MetaData.Package.package2); });
        textPackage2.text = AlfucodePurchaser.instance.SetValueItemsFromStore(MetaData.Package.package2);
        buttonPackage3.onClick.AddListener(() => { AlfucodePurchaser.instance.Buy(MetaData.Package.package3); });
        textPackage3.text = AlfucodePurchaser.instance.SetValueItemsFromStore(MetaData.Package.package3);
    }

    public void OpenStore()
    {
        PlayerLog.Instance.IncreaseStoreButtonCounter();
        if (FindObjectOfType<UIManager>() != null && achievementPanel != null && achievementPanel.activeSelf == false)
            FindObjectOfType<UIManager>().HidePlayButton();
        if (achievementPanel == null)
            FindObjectOfType<UIManager>().HidePlayButton();


        ShopPanel.SetActive(true);
    }

    public void CloseStore()
    {
        Time.timeScale = 1;

        if (achievementPanel != null)
            if (achievementPanel.activeSelf == true)
                achievementPanel.SetActive(false);

        if (FindObjectOfType<UIManager>() != null)
            FindObjectOfType<UIManager>().ShowPlayButton();

        ShopPanel.SetActive(false);
    }

    public void FreeCoin()
    {

#if UNITY_ANDROID || UNITY_IOS
        AdsManager.instance.rewardAdsTypes = AdsManager.RewardAdsTypes.EarnCoin10;
        AdsManager.instance.ShowRewardedAd();
        AdsManager.instance.CreateAndLoadRewardedAd();
#endif

#if UNITY_EDITOR
        ShopManager.Instance.AddAlfuCoin(50);
#endif

    }

    public void OpenPromoCodePanel()
    {
        if (FindObjectOfType<UIManager>() != null)
            FindObjectOfType<UIManager>().HidePlayButton();

        PromoPanel.SetActive(true);
        ShopPanel.SetActive(false);
    }


    public void EnterPromoCode()
    {
        string result = PromoCode.instance.EnterPromoCode(promoText.text);
        promoText.text = "";
        if (result == "codeverified")
        {
            invalidPromoCode.SetActive(false);
            codeverified.SetActive(true);
            return;
        }

        if (result == "true")
        {
            StartCoroutine(ShowUnlockedSkins(result));
        }
        else
        {
            invalidPromoCode.SetActive(true);
            codeverified.SetActive(false);
        }
    }

    IEnumerator ShowUnlockedSkins(string result)
    {
        invalidPromoCode.SetActive(false);
        codeverified.SetActive(true);
        yield return new WaitForSeconds(1f);
        codeverified.SetActive(false);
        PromoPanel.SetActive(false);
        var skins = result.Split(',');

        foreach (var item in skins)
        {
            GameManager.instance.skinUnlocked.isClicked = false;

            GameManager.instance.skinUnlocked.skinImage.sprite = Resources.Load<Sprite>(MetaData.ConstVariable.Character.Find(item).iconFilePath);

            GameManager.instance.skinUnlocked.EquipButtonClicked = () =>
            {
                PlayerPrefs.SetString("SelectedCharacter", item);
                GameManager.instance.skinUnlocked.CancelButton_OnClick();
                EconomyScript.Instance.SetSkinToDataPlayer(item);

            };

            GameManager.instance.skinUnlocked.Show();

            yield return new WaitUntil(() => GameManager.instance.skinUnlocked.isClicked == true);

        }

        FindObjectOfType<UIManager>().ShowPlayButton();

    }


    public void ClosePromoCodePanel()
    {
        invalidPromoCode.SetActive(false);
        codeverified.SetActive(false);

        PromoPanel.SetActive(false);
        ShopPanel.SetActive(false);

        if (achievementPanel != null)
            if (achievementPanel.activeSelf == true)
                achievementPanel.SetActive(false);

        if (FindObjectOfType<UIManager>() != null)
            FindObjectOfType<UIManager>().ShowPlayButton();
        Time.timeScale = 1;

    }

}

