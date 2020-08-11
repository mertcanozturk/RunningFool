using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    private int coin;
    public int alfuCoin;

    public int AlfuCoin
    {
        get
        {
            return alfuCoin;
        }
    }
    public int Coin
    {
        get
        {
            return coin;
        }
    }

    #region Private Methods
    void Awake()
    {
        InitializeParameters();
        PlayerPrefs.SetInt("skin1", 1);
    }

    void InitializeParameters()
    {
        if (PlayerPrefs.HasKey("CurrentCoin"))
        {
            coin = PlayerPrefs.GetInt("CurrentCoin");
            alfuCoin = PlayerPrefs.GetInt("CurrentAlfuCoin");
        }
        else
        {
            Debug.Log("First Time");
            coin = 45;
            alfuCoin = 45;
            PlayerPrefs.SetInt("CurrentCoin", 45);
            PlayerPrefs.SetInt("CurrentAlfuCoin", 45);
            SaveAllKeys();
        }
    }

    void SaveAllKeys()
    {
        // Do None Purchased 
        foreach (var character in MetaData.ConstVariable.Character.characters)
        {
            if (!PlayerPrefs.HasKey(character.key))
                PlayerPrefs.SetInt(character.key, 0);
        }
    }

    #endregion

    #region Public Methods
    /// <summary>
    /// This method add coins;
    /// </summary>
    /// <param name="amount">coin amount</param>
    public void AddCoin(int amount)
    {
        coin += amount;
        SaveCoin();
    }

    public void AddAlfuCoin(int amount)
    {
        alfuCoin += amount;
        SaveCoin();
    }
    public void SaveCoin()
    {
        PlayerPrefs.SetInt("CurrentCoin", coin);
        PlayerPrefs.SetInt("CurrentAlfuCoin", alfuCoin);
        ServerData.Instance.SaveAlfuCoin(alfuCoin);
    }

    /// <summary>
    /// This method reduces coin. Careful!! No negative value control. Coin can be negative.
    /// </summary>
    /// <param name="amount">coin amount</param>
    public void ReduceCoin(int amount)
    {
        coin -= amount;
        SaveCoin();
    }

    /// <summary>
    /// This method reduces alfuCoin. Careful!! No negative value control. Coin can be negative.
    /// </summary>
    /// <param name="amount">coin amount</param>
    public void ReduceAlfuCoin(int amount)
    {
        alfuCoin -= amount;
        SaveCoin();
    }

    /// <summary>
    /// You can buy character with Metadata.ShopItemPrices Dictionary keys.
    /// </summary>
    /// <param name="key">use metadata shop item keys.</param>
    /// <returns></returns>
    public bool BuyCharacter(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            if (PlayerPrefs.GetInt(key) == 0)
            {
                //PlayerPrefs.SetString("SelectedCharacter", key);

                var character = MetaData.ConstVariable.Character.Find(key);

                if (character.coinType == MetaData.CoinTypes.Gold)
                {
                    ReduceCoin(character.price);
                }
                else
                {
                    ReduceAlfuCoin(character.price);
                }

                PlayerPrefs.SetInt(key, 1);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {

            return false;
        }
    }
    #endregion
}
