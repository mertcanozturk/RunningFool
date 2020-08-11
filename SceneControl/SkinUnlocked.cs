using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkinUnlocked : MonoBehaviour
{
    public Image skinImage;
    public Button ButtonEquip;
    public Button ButtonExit;
    public Button Shop;

    public GameManager.Callback EquipButtonClicked;
    public GameManager.Callback CancelClicked;


    public bool isClicked = false;

    void Start()
    {
        ButtonEquip.onClick.AddListener(EquipButtonOnClick);
        if (ButtonExit)
            ButtonExit.onClick.AddListener(CancelButton_OnClick);

        Shop.onClick.AddListener(ShopButtonOnclick);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CancelButton_OnClick();
        }
    }

    public void ShopButtonOnclick()
    {
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.ShopScene);
    }
    public void EquipButtonOnClick()
    {
        GameManager.instance.SoundManager.ButtonSound.Play();
        if (EquipButtonClicked != null)
            EquipButtonClicked();

        isClicked = true;
    }
    public void CancelButton_OnClick()
    {
        GameManager.instance.SoundManager.ButtonSound.Play();

        gameObject.SetActive(false);
        if (CancelClicked != null)
            CancelClicked();

        isClicked = true;
    }
}
