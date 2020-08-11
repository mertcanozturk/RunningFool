using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCreator : MonoBehaviourPun, IPunObservable
{
    public int ReadyPlayerCount = 0;
    public bool isStarted = false;

    Outline_Controller outline_Controller;

    public Image startIn, three, two, one, run;

    public string[] enemies;

    public string[] contestEnemies;

    public int maxEnemyCountInTheScene = 6;

    public List<GameObject> objects = new List<GameObject>();

    public List<int> rotations = new List<int>();

    float objectCreateStartedTime;

    private void Start()
    {
        StartCoroutine(StartTheGame());
    }
    IEnumerator StartTheGame()
    {
        yield return new WaitForSeconds(2f);
        StartGame();
    }

    public void Ready()
    {
        return;
    }

    IEnumerator StartForCreateObjects()
    {
        yield return new WaitForSeconds(1f);

        objectCreateStartedTime = Time.time;
        while (true)
        {

            if (objects.Count < maxEnemyCountInTheScene)
                StartCoroutine(CreateRandomObject());

            yield return new WaitForSeconds(Random.Range(Mathf.Max(0, (5 - ((Time.time - objectCreateStartedTime) / 6))), 6));

            if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.GAME_OVER)
                break;

        }

    }

    IEnumerator CreateRandomObject()
    {
        int randomRotation = Random.Range(-180, 180);

        GameObject obj;

        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Multiplayer2 || GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Multiplayer4 || GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Multiplayer3)
        {
            obj = PhotonNetwork.Instantiate(Path.Combine("Multiplayer", enemies[Random.Range(0, enemies.Length)]), Vector3.zero, new Quaternion(0, 0, randomRotation, 60));
        }
        else if (!GameManager.instance.IsAIMode || GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
        {
            obj = Instantiate((GameObject)Resources.Load(Path.Combine("Multiplayer", contestEnemies[Random.Range(0, enemies.Length)])), Vector3.zero, new Quaternion(0, 0, randomRotation, 60));

        }
        else
        {
            obj = Instantiate((GameObject)Resources.Load(Path.Combine("Multiplayer", enemies[Random.Range(0, enemies.Length)])), Vector3.zero, new Quaternion(0, 0, randomRotation, 60));
        }

        objects.Add(obj);
        yield return new WaitForSeconds(Random.Range(7, 12));
        objects.Remove(obj);
        if (!GameManager.instance.IsAIMode)
        {
            if (obj != null)
                PhotonNetwork.Destroy(obj);
        }
        else
        {
            if (obj != null)
                Destroy(obj);
        }

    }

    public void StartGame()
    {
        StartCoroutine(CountForStartTheGame());
    }

    IEnumerator CountForStartTheGame()
    {
        GameManagerIngame.Instance.LocalPlayer.canMove = false;
        GameManagerIngame.Instance.LocalPlayer.canJump = false;

        startIn.gameObject.SetActive(true);
        run.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);

        three.gameObject.SetActive(true);
        two.gameObject.SetActive(false);
        one.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);
        three.gameObject.SetActive(false);
        two.gameObject.SetActive(true);
        one.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        three.gameObject.SetActive(false);
        two.gameObject.SetActive(false);
        one.gameObject.SetActive(true);

        yield return new WaitForSeconds(1);
        run.gameObject.SetActive(true);
        startIn.gameObject.SetActive(false);
        three.gameObject.SetActive(false);
        two.gameObject.SetActive(false);
        one.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        isStarted = true;

        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.PLAYING;
        GameManagerIngame.Instance.LocalPlayer.SetPlayerState(Player.PlayerState.Running);
        run.gameObject.SetActive(false);

        //GameManagerIngame.Instance.LocalPlayer.Arrow.SetActive(false);
        GameManagerIngame.Instance.LocalPlayer.canMove = true;
        GameManagerIngame.Instance.LocalPlayer.canJump = true;


        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
            GameManagerIngame.Instance.LocalPlayer.Arrow.SetActive(false);


        if (GameManager.instance.IsAIMode)
        {
            StartCoroutine(StartForCreateObjects());

            GameManagerIngame.Instance.AIPlayer.SetPlayerState(AIPlayer.PlayerState.Running);

        }
        else if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartForCreateObjects());

        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    public void FinishGame()
    {
        StopAllCoroutines();

        if (GameManager.instance.IsAIMode)
        {
            foreach (var item in objects)
            {
                Destroy(item);
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
                foreach (var item in objects)
                {
                    if (item != null)
                        PhotonNetwork.Destroy(item);
                }
        }

    }

    public float FindNearestEnemyDistance(float rot)
    {
        float minDistance = 9999;
        if (objects.Count > 0)
        {
            foreach (var item in objects)
            {
                if (item != null)
                    if (Mathf.Abs(item.transform.eulerAngles.z - rot) < minDistance)
                        minDistance = item.transform.eulerAngles.z - rot;
            }
        }


        return minDistance;


    }
}
