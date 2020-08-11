using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Shop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI alfuCoinText, priceText;
    [SerializeField] private Button priceButton, selectButton;
    [SerializeField] private GameObject[] skins;
    [SerializeField] private SkinnedMeshRenderer characterMesh;
    [SerializeField] private GameObject player, shopPanel;

    private int coin, alfuCoin;
    private string selectedCharacterKey;
    private void Start()
    {
        PlayerLog.Instance.IncreaseSkinsButtonCounter();

        selectedCharacterKey = PlayerPrefs.GetString("SelectedCharacter");
        characterMesh.material = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(selectedCharacterKey).materialPath);

        UpdateUI();

        foreach (var item in InitializerManager.instance.Data.SettingGame.Store.Store)
        {
            //this mean Virtual Item Id.
            //Debug.Log(item.ItemId);
            //what is type for buy (AC = AlfucodeCoin , RM = RealMoney , AD = Ads)
            //Debug.Log(item.VirtualCurrencyPrices.ElementAt(0).Key);
            //this mean the Price by Alfucoin.
            //Debug.Log(item.VirtualCurrencyPrices.ElementAt(0).Value);

            var it = InitializeDataScript.instance.IAP.VI.FirstOrDefault(x => x.ItemID == item.ItemId);
            if (it != null && MetaData.ConstVariable.Character.Find(it.ItemID) != null)
            {
                MetaData.ConstVariable.Character.Find(it.ItemID).price = int.Parse(item.VirtualCurrencyPrices.ElementAt(0).Value.ToString());
            }
        }


    }

    void UpdateCoin()
    {
        if (alfuCoin != ShopManager.Instance.AlfuCoin)
        {
            coin = ShopManager.Instance.Coin;
            alfuCoin = ShopManager.Instance.AlfuCoin;
            alfuCoinText.text = alfuCoin.ToString();
            UpdateUI();
        }
    }

    private void Update()
    {
        UpdateCoin();
    }


    public void PlayerActive()
    {
        player.SetActive(true);
    }

    public void PlayerDeactive()
    {
        player.SetActive(false);
    }

    public void OpenShop()
    {
        PlayerLog.Instance.IncreaseStoreButtonCounter();
        player.SetActive(false);
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        player.SetActive(true);
        shopPanel.SetActive(false);
    }

    void UpdateUI()
    {

        int skinIndex = 0;
        int index = -1;

        foreach (var character in MetaData.ConstVariable.Character.characters)
        {
            index++;
            if (PlayerPrefs.HasKey(character.key))
            {
                if (PlayerPrefs.GetInt(character.key) == 1)
                {
                    int _skinIndex = skinIndex;
                    int _index = index;
                    skins[_skinIndex].GetComponentInChildren<Button>().interactable = true;
                    skins[_skinIndex].transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(character.iconFilePath);

                    character.state = MetaData.ConstVariable.Character.STATE.PURCHASED;
                    skins[_skinIndex].GetComponentInChildren<Button>().onClick.AddListener(() => { SelectClick(_index); });
                    skins[_skinIndex].transform.Find("LockImage").GetComponent<Image>().enabled = false;

                    skinIndex++;
                }
            }
        }

        index = -1;
        foreach (var character in MetaData.ConstVariable.Character.characters)
        {
            index++;

            if (PlayerPrefs.HasKey(character.key))
            {

                if (character.price < alfuCoin && PlayerPrefs.GetInt(character.key) == 0)
                {
                    int _skinIndex = skinIndex;
                    int _index = index;

                    skins[_skinIndex].GetComponentInChildren<Button>().interactable = true;
                    skins[_skinIndex].transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(character.iconFilePath);

                    character.state = MetaData.ConstVariable.Character.STATE.PURCHASABLE;
                    skins[_skinIndex].GetComponentInChildren<Button>().onClick.AddListener(() => { SelectClick(_index); });
                    skins[_skinIndex].transform.Find("LockImage").GetComponent<Image>().enabled = true;
                    skinIndex++;

                }

            }
        }

        index = -1;
        foreach (var character in MetaData.ConstVariable.Character.characters)
        {
            index++;


            if (PlayerPrefs.HasKey(character.key))
            {
                if (character.price > alfuCoin && PlayerPrefs.GetInt(character.key) == 0)
                {
                    int _skinIndex = skinIndex;
                    int _index = index;

                    skins[_skinIndex].transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>(character.iconFilePath);
                    skins[_skinIndex].GetComponentInChildren<Button>().interactable = true;
                    character.state = MetaData.ConstVariable.Character.STATE.NOCOIN;
                    skins[_skinIndex].GetComponentInChildren<Button>().onClick.AddListener(() => { SelectClick(_index); });
                    skins[_skinIndex].transform.Find("LockImage").GetComponent<Image>().enabled = true;
                    skinIndex++;
                }

            }
        }


    }

    void SelectClick(int productIndex)
    {
        characterMesh.material = Resources.Load<Material>(MetaData.ConstVariable.Character.characters[productIndex].materialPath);

        if (MetaData.ConstVariable.Character.characters[productIndex].key == selectedCharacterKey)
        {
            selectButton.gameObject.SetActive(false);
            priceButton.gameObject.SetActive(false);

        }
        else if (MetaData.ConstVariable.Character.characters[productIndex].state == MetaData.ConstVariable.Character.STATE.NOCOIN)
        {
            selectButton.gameObject.SetActive(false);
            priceButton.gameObject.SetActive(true);

            priceText.text = MetaData.ConstVariable.Character.characters[productIndex].price.ToString();
            priceButton.interactable = false;
        }
        else if (MetaData.ConstVariable.Character.characters[productIndex].state == MetaData.ConstVariable.Character.STATE.PURCHASABLE)
        {
            selectButton.gameObject.SetActive(false);
            priceButton.gameObject.SetActive(true);
            priceText.text = MetaData.ConstVariable.Character.characters[productIndex].price.ToString();
            priceButton.interactable = true;
            priceButton.onClick.RemoveAllListeners();
            priceButton.onClick.AddListener(() => { BuyClick(MetaData.ConstVariable.Character.characters[productIndex].key); });
        }
        else if (MetaData.ConstVariable.Character.characters[productIndex].state == MetaData.ConstVariable.Character.STATE.PURCHASED)
        {
            selectButton.gameObject.SetActive(true);
            priceButton.gameObject.SetActive(false);
            selectButton.interactable = true;
            selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
            selectButton.onClick.AddListener(() => { ChangeSelectedCharacter(MetaData.ConstVariable.Character.characters[productIndex].key); });
        }
    }

    void ChangeSelectedCharacter(string key)
    {
        selectedCharacterKey = key;
        PlayerPrefs.SetString("SelectedCharacter", key);
        selectButton.gameObject.SetActive(false);
        priceButton.gameObject.SetActive(false);
        EconomyScript.Instance.SetSkinToDataPlayer(key);
    }

    void BuyClick(string key)
    {
        if (ShopManager.Instance.BuyCharacter(key))
        {
            Debug.Log("The purchase was successful.");
            selectButton.gameObject.SetActive(true);
            priceButton.gameObject.SetActive(false);
            selectButton.interactable = true;
            selectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Select";
            selectButton.onClick.AddListener(() => { ChangeSelectedCharacter(key); });

            ServerData.Instance.SaveSkinInfo(key);
        }
        else
        {
            Debug.Log("An error occurred while purchasing.");
        }
    }

    public void EarnCoinWithWatchAds()
    {
#if UNITY_ANDROID || UNITY_IOS
        AdsManager.instance.rewardAdsTypes = AdsManager.RewardAdsTypes.EarnCoin20;
        AdsManager.instance.ShowRewardedAd();
        AdsManager.instance.CreateAndLoadRewardedAd();
#endif

#if UNITY_EDITOR
        ShopManager.Instance.AddCoin(50);
        Debug.Log("Added 50 AlfuCoin.");
#endif
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.MainScene);
    }
}
