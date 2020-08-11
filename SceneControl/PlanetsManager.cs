using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlanetsManager : MonoBehaviour
{
    [SerializeField] private Text[] worldTexts;
    [SerializeField] private GameObject[] worlds;
    [SerializeField] private GameObject[] RequiredText;
    [SerializeField] private Image[] filledImages;
    [SerializeField] private TextMeshProUGUI[] completionRate;
    [SerializeField] private Sprite activePlanetSprite;
    [SerializeField] private Image[] planetNumbersImage;

    private void Start()
    {
        GetPlayedLevels();
        CheckRequiredScores();
        SetSliderValues();
    }

    private void GetPlayedLevels()
    {
        int levelCounter;

        int worldCounter = 0;

        bool isPassed = false;

        int currentWorldCounter = 0;

        foreach (var world in MetaData.ConstVariable.Planet.planets)
        {
            levelCounter = 1;

            for (int i = 0; i < 20; i++)
            {

                if (PlayerPrefs.HasKey(world.key + levelCounter))
                {
                    levelCounter++;
                    isPassed = true;
                    if (!isPassed) currentWorldCounter++;
                }
            }

            isPassed = false;
            worldCounter += 1;
        }

        for (int i = 0; i < currentWorldCounter + 1; i++)
        {
            planetNumbersImage[i].sprite = activePlanetSprite;
        }
    }

    private void CheckRequiredScores()
    {
        int planetIndex = 0;
        foreach (var world in worlds)
        {
            if (MetaData.ConstVariable.Planet.planets[planetIndex].requiredScore > GameManagerIngame.Instance.Score)
            {
                RequiredText[planetIndex].SetActive(true);

                RequiredText[planetIndex].GetComponent<TextMeshProUGUI>().text = MetaData.ConstVariable.Planet.planets[planetIndex].requiredScore.ToString();
                worlds[planetIndex].transform.Find("Locked").GetComponent<Image>().enabled = true;
                filledImages[planetIndex].gameObject.SetActive(false);
                completionRate[planetIndex].gameObject.SetActive(false);
                world.GetComponent<Animator>().enabled = false;
            }
            else
            {
                RequiredText[planetIndex].SetActive(false);
                worlds[planetIndex].transform.Find("Line").GetComponent<Image>().enabled = true;
                worlds[planetIndex].transform.Find("Locked").GetComponent<Image>().enabled = false;
                filledImages[planetIndex].gameObject.SetActive(true);
                completionRate[planetIndex].gameObject.SetActive(true);
                worlds[planetIndex].GetComponent<Button>().interactable = true;
            }
            planetIndex++;
        }
    }

    private void SetSliderValues()
    {
        int planetIndex = 0;
        foreach (var world in MetaData.ConstVariable.Planet.planets)
        {
            int level = 0;

            for (int i = 0; i <= 20; i++)
            {
                if (PlayerPrefs.HasKey(world.key + "_Level_" + i + "_Passed"))
                {
                    level = i;
                }
            }

            if (level != 0)
            {
                RequiredText[planetIndex].SetActive(false);
                worlds[planetIndex].GetComponent<Button>().interactable = true;
                filledImages[planetIndex].gameObject.SetActive(true);
                completionRate[planetIndex].gameObject.SetActive(true);
                worlds[planetIndex].transform.Find("Locked").GetComponent<Image>().enabled = false;
                filledImages[planetIndex].fillAmount = ( 1 - 1.0f / 20.0f * level);
                completionRate[planetIndex].text = "%" + ((int)((1 - filledImages[planetIndex].fillAmount) * 100)).ToString();
            }

            planetIndex++;
        }
    }

    public void OpenEmojiPlanets()
    {
        PlayerPrefs.SetString("OpenedJsonFile", MetaData.ConstVariable.Planet.planets[0].jsonFileName);
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.LevelsScene);
        PlayerPrefs.SetString("OpenedWorld", MetaData.ConstVariable.Planet.planets[0].key);
    }

    public void OpenSamplePlanet()
    {
        PlayerPrefs.SetString("OpenedJsonFile", MetaData.ConstVariable.Planet.planets[1].jsonFileName);
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.LevelsScene);
        PlayerPrefs.SetString("OpenedWorld", MetaData.ConstVariable.Planet.planets[1].key);
    }

    public void OpenSample1Planet()
    {
        PlayerPrefs.SetString("OpenedJsonFile", MetaData.ConstVariable.Planet.planets[2].jsonFileName);
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.LevelsScene);
        PlayerPrefs.SetString("OpenedWorld", MetaData.ConstVariable.Planet.planets[2].key);
    }

    public void OpenSample2Planet()
    {
        PlayerPrefs.SetString("OpenedJsonFile", MetaData.ConstVariable.Planet.planets[3].jsonFileName);
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.LevelsScene);
        PlayerPrefs.SetString("OpenedWorld", MetaData.ConstVariable.Planet.planets[3].key);
    }
    public void OpenSample3Planet()
    {
        PlayerPrefs.SetString("OpenedJsonFile", MetaData.ConstVariable.Planet.planets[4].jsonFileName);
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.LevelsScene);
        PlayerPrefs.SetString("OpenedWorld", MetaData.ConstVariable.Planet.planets[4].key);
    }
    public void Back()
    {
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.MainScene);
    }
}
