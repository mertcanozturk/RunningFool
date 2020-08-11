using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playButton,onlineButton,contestButton;
    public Button OnlineButton;


    private void Start()
    {
        OnlineButton.onClick.AddListener(() =>
        {
            GameManagerIngame.Instance.GameMode = GameManagerIngame.Mode.Multiplayer2;
            GameManager.instance.GameMode = MetaData.GameMode.Online;
            SceneManager.LoadScene(MetaData.ConstVariable.Scenes.SyncScean, LoadSceneMode.Single);
        });
    }
    public void ShowPlayButton()
    {
        playButton.SetActive(true);
        onlineButton.SetActive(true);
        contestButton.SetActive(true);
    }
    public void HidePlayButton()
    {
        playButton.SetActive(false);
        onlineButton.SetActive(false);
        contestButton.SetActive(false);
    }
}
