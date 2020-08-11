using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlacePlayers : MonoBehaviour
{
    [System.Serializable]
    public class PlayerPos
    {
        public GameObject IdlePosition, jumpingPosition;
    }

    [SerializeField] PlayerPos[] PlayerPositions;
    [SerializeField] Transform parent;

    public Player localplayer;
    public AIPlayer aiPlayer;

    void Start()
    {
        StartCoroutine(Place());
    }


    IEnumerator Place()
    {
        yield return new WaitForSeconds(0.5f);

        if (GameManager.instance.IsAIMode)
        {
            GameManagerIngame.Instance.LevelManager.canTurn = true;


        }
        else
        {
            //localplayer.gameObject.SetActive(false);
            aiPlayer.gameObject.SetActive(false);

            GameObject player;
            if (PhotonNetwork.IsMasterClient)
            {
                player = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), PlayerPositions[0].IdlePosition.transform.position, PlayerPositions[0].IdlePosition.transform.rotation);
                player.transform.SetParent(parent);

                player.GetComponent<Player>().jumpingTransform = PlayerPositions[0].jumpingPosition.transform;
                player.GetComponent<Player>().idleTransform = PlayerPositions[0].IdlePosition.transform;
            }
            else
            {
                player = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), PlayerPositions[1].IdlePosition.transform.position, PlayerPositions[1].IdlePosition.transform.rotation);
                player.transform.SetParent(parent);

                player.GetComponent<Player>().jumpingTransform = PlayerPositions[1].jumpingPosition.transform;
                player.GetComponent<Player>().idleTransform = PlayerPositions[1].IdlePosition.transform;
            }

            GameManagerIngame.Instance.LevelManager.canTurn = true;
        }

        if (SyncSceneScript.SyncSceneObject.PanelGame.gameObject.activeSelf)
            SyncSceneScript.SyncSceneObject.PanelGame.gameObject.SetActive(false);

    }

}
