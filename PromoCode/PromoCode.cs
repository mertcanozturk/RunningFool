using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoCode : MonoBehaviour
{
    public static PromoCode instance;
    public AlfucodeCore.PromoCode Promo = new AlfucodeCore.PromoCode();
    AlfucodeCore.PackagePromo packagePromo = new AlfucodeCore.PackagePromo();
    string str = string.Empty;

    private void Awake()
    {
        instance = this;
    }

  

    public string EnterPromoCode(string code)
    {
        bool pd = false;
        string GetHash = GameManager.Scurity.GetHash(code);
        str = string.Empty;
        foreach (var item in Promo.Package)
        {
            if(item.HashPromo == GetHash && CheckPlayerPromo(item.PromoName))
            {
                pd = true;
                packagePromo = item;
                break;
            }
        }
        
        if(pd)
        {            
            AlfucodeAPIServer.instance.OnUpdateProfile += OnUpdate;
            str = GameManager.Player.PromoCode + "_"+ packagePromo.PromoName;
            AlfucodeAPIServer.instance.ConnectWithSubServer(MetaData.ConstVariable.PlayFab.ProjectName, packagePromo.PromoName);
            AlfucodeAPIServer.instance.UpdatePlayerData(MetaData.ConstVariable.PlayFabPlayer.PromoCode, str);
            return "true";
        }
        else
        {
            return "false";
        }

    }

    private void OnUpdate()
    {
        AlfucodeAPIServer.instance.OnUpdateProfile -= OnUpdate;
        GameManager.Player.PromoCode = str;
        if (packagePromo != null)
        {
            foreach (var item in packagePromo.ListPrize)
            {
                if (item.isRandom)
                {
                    int getrandom = UnityEngine.Random.RandomRange(0, item.ListRandomPrize.Count);
                    string id = item.ListRandomPrize[getrandom].VirtualItem.ToString();
                    //give skin by id

                    if (ShopManager.Instance.BuyCharacter(id))
                    {
                        ShopManager.Instance.AddAlfuCoin(MetaData.ConstVariable.Character.Find(id).price);

                        ServerData.Instance.SaveSkinInfo(id);
                    }

                }
                else
                {
                    if (item.Category == MetaData.Category.VirtualCurrency)
                    {
                        int itemamount = item.Amount;
                        //Give coin itemamount value .
                        ShopManager.Instance.AddAlfuCoin(itemamount);
                    }
                    else
                    {
                        string id = item.VirtualItem.ToString();

                        if (ShopManager.Instance.BuyCharacter(id))
                        {
                            ShopManager.Instance.AddAlfuCoin(MetaData.ConstVariable.Character.Find(id).price);

                            ServerData.Instance.SaveSkinInfo(id);
                        }

                        //give skin by id
                    }
                }

            }
        }
    }

    public bool CheckPlayerPromo(string Package)
    {
        if (string.IsNullOrEmpty(GameManager.Player.PromoCode))
            return true;
        else
        {
            string[] PackageName = GameManager.Player.PromoCode.Split('_');
            foreach (var item in PackageName)
            {
                if (item == Package)
                    return false;
            }

            return true;
        }
                
    }
}

