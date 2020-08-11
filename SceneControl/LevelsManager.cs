using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelsManager : MonoBehaviour
{
    public List<Button> levelButtons;

    public Sprite noLevelSprite, playSprite;

    public Image background;

    public Sprite[] backgrounds;

    public TextMeshProUGUI planetName;

    string jsonData = "";
    private JsonDataConvert levelInfos;

    void Start()
    {
        SetBackground();
        DeSerializeJsonData();
        AddListenerToButtons();
        PlayerLog.Instance.IncreaseLevelsButtonCounter();
    }

    void SetBackground()
    {
        if (MetaData.ConstVariable.Planet.planets[0].key == PlayerPrefs.GetString("OpenedWorld"))
        {
            background.sprite = backgrounds[0];
        }
        if (MetaData.ConstVariable.Planet.planets[1].key == PlayerPrefs.GetString("OpenedWorld"))
        {
            background.sprite = backgrounds[1];
        }
        if (MetaData.ConstVariable.Planet.planets[2].key == PlayerPrefs.GetString("OpenedWorld"))
        {
            background.sprite = backgrounds[2];
        }
    }


    private void DeSerializeJsonData()
    {
        if (jsonData == "")
        {
            string path = "";
            planetName.text = MetaData.ConstVariable.Planet.Find( PlayerPrefs.GetString("OpenedWorld")).name;

#if UNITY_ANDROID
            path = "jar:file://" + Application.dataPath + "!/assets/" + PlayerPrefs.GetString("OpenedJsonFile") + ".json";
            WWW www = new WWW(path);
            while (!www.isDone) { }
            jsonData = www.text;
            levelInfos = JsonUtility.FromJson<JsonDataConvert>(jsonData);
#endif

#if UNITY_EDITOR || UNITY_IOS
            path = Application.streamingAssetsPath + "/" + PlayerPrefs.GetString("OpenedJsonFile") + ".json";
            StreamReader reader = new StreamReader(path);
            jsonData = reader.ReadToEnd();
            levelInfos = JsonUtility.FromJson<JsonDataConvert>(jsonData);
#endif
        }

    }

    private void AddListenerToButtons()
    {
        int level = 0;
        foreach (var btn in levelButtons)
        {
            int _level = ++level;

            btn.onClick.AddListener(() => { OpenLevel(_level); });

            if (PlayerPrefs.HasKey(PlayerPrefs.GetString("OpenedWorld") + "_Level_" + level + "_Passed"))
            {
                Sprite[] sprites = Resources.LoadAll<Sprite>(levelInfos.Levels[_level - 1].Sprite);
                //levelButtons[_level - 1].transform.Find("Image").GetComponent<Image>().rectTransform.sizeDelta = new Vector2(256, 256);
                levelButtons[_level - 1].transform.Find("Image").GetComponent<Image>().sprite = sprites[0];
            }
            else
            {
                levelButtons[_level - 1].transform.Find("Play").GetComponent<Image>().enabled = true;
                levelButtons[_level - 1].transform.Find("Play").GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                btn.onClick.AddListener(() => { OpenLevel(_level); });
                break;
            }

        }
    }

    private void OpenLevel(int level)
    {
        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.SetInt("LevelSelected", 1);
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.GameScene);
    }

    public void Back()
    {
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.PlanetsScene);
    }
}
