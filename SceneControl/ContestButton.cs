using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContestButton : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer playerMesh;

    [SerializeField] private ContestUI contest;

    [SerializeField] private GameObject mainUI, mainBackground;

    [SerializeField] Animator animator;

    ScalebigbongScript scalebigbong;
    private void Start()
    {
        scalebigbong = GetComponent<ScalebigbongScript>();
        CheckNewContest();
        if (PlayerPrefs.HasKey("SelectedCharacter"))
            playerMesh.material = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(PlayerPrefs.GetString("SelectedCharacter")).materialPath);
    }

    private void CheckNewContest()
    {
        if (AlfucodeAPIContest.instance.contestStatus == MetaData.StatusContest.NewContest)
        {
            scalebigbong.enabled = true;
        }
        else if (AlfucodeAPIContest.instance.contestStatus == MetaData.StatusContest.PastConetest)
        {
            scalebigbong.enabled = false;
            playerMesh.material = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(GameManager.instance.skin1Player).materialPath);

        }
    }

    private void OnMouseDown()
    {
        GameManagerIngame.Instance.isChallengeGame = false;
        FindObjectOfType<UIManager>().HidePlayButton();
        contest.GoToContestMain();
        mainUI.SetActive(false);
        mainBackground.SetActive(false);
    }
}
