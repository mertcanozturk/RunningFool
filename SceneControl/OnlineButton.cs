using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OnlineButton : MonoBehaviour
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
        GameManagerIngame.Instance.isChallengeGame = false;

        GameManagerIngame.Instance.GameMode = GameManagerIngame.Mode.Multiplayer2;
        GameManager.instance.GameMode = MetaData.GameMode.Online;
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.SyncScean, LoadSceneMode.Single);

    }
}
