using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{

    [SerializeField] private SkinnedMeshRenderer playerMesh;
    private void Start()
    {
        if (!PlayerPrefs.HasKey("SelectedCharacter"))
            PlayerPrefs.SetString("SelectedCharacter", "skin1");
        playerMesh.material = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(PlayerPrefs.GetString("SelectedCharacter")).materialPath);
    }
    private void OnMouseDown()
    {

        PlayerLog.Instance.IncreasePlayButtonCounter();

        GameManager.instance.GameMode = MetaData.GameMode.Mission;
        GameManagerIngame.Instance.GameMode = GameManagerIngame.Mode.SinglePlayer;
        if (!PlayerPrefs.HasKey("CurrentLevel"))
        {
            PlayerPrefs.SetInt("CurrentLevel", 1);
            PlayerPrefs.SetString("OpenedWorld", MetaData.ConstVariable.Planet.planets[0].key);
            PlayerPrefs.SetString("OpenedJsonFile", MetaData.ConstVariable.Planet.planets[0].jsonFileName);
        }

        PlayerPrefs.SetInt("LevelSelected", 1);
        GameManagerIngame.Instance.isChallengeGame = false;
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.GameScene);

    }

}
