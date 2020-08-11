using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts;
using System.IO;
using UnityEngine.UI;
using DG.Tweening;
public class Contest : MonoBehaviour
{
    private string jsonData = "";
    private float startTime;
    private JsonDataConvert levelInfos1;
    private JsonDataConvert levelInfos2;

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject collectableScore;
    [SerializeField] private GameObject collectableCoin;
    [SerializeField] private TMP_Text playerName;

    [SerializeField] SkinnedMeshRenderer[] playerskinRenderers;


    public bool isStarted = false;
    public Image startIn, three, two, one, run;
    public TMP_Text timeText, roundText, scoreText;
    public GameObject scoreObject;
    public float time = 0;

    public float score = 0;

    public int roundNumber = 0;
    public int levelNumber = 0;

    private int randomLevel = 1;
    private int randomWorld = 1;

    private LevelCreator _levelCreator;
    public LevelCreator LevelCreator
    {
        get
        {
            if (_levelCreator == null)
                _levelCreator = FindObjectOfType<LevelCreator>();
            return _levelCreator;
        }
    }

    void Awake()
    {
        GetJsonData();
        GameManager.instance.IsAIMode = false;
        GameManagerIngame.Instance.GameMode = GameManagerIngame.Mode.Contest;
        StartCoroutine(WaitForGameStart());
        playerName.text = GameManager.Player.NamePlayer;
    }

    private void Start()
    {
        foreach (var item in playerskinRenderers)
        {
            item.material = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(PlayerPrefs.GetString("SelectedCharacter")).materialPath);

        }

    }
    IEnumerator WaitForGameStart()
    {
        NextRound();

        yield return new WaitUntil(() => GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING);
        startTime = Time.time;
        isStarted = true;

        StartCoroutine(WaitForGameEnd());
    }

    IEnumerator WaitForGameEnd()
    {
        yield return new WaitUntil(() => GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.GAME_OVER);

        Debug.Log("Finished Time:" + time);
        // Save Time Info;
    }


    void CreateRound()
    {
        LevelCreator.ResetLevel();

        if (randomWorld == 1)
        {

            LevelCreator.SetupCharacterSpeed(levelInfos1.Levels[randomLevel].CharacterSpeed);

            LevelCreator.SetupEnemies(levelInfos1.Levels[randomLevel].Enemies);
        }
        else
        {
            LevelCreator.SetupCharacterSpeed(levelInfos2.Levels[randomLevel].CharacterSpeed);

            LevelCreator.SetupEnemies(levelInfos2.Levels[randomLevel].Enemies);
        }


        //  LevelCreator.SetupBackgroundImage(levelInfos.Levels[randomLevel].Background);
    }

    IEnumerator GenerateRound()
    {
        yield return new WaitUntil(() => GameManagerIngame.Instance.LocalPlayer != null);

        GetRandomLevel();

        GameManagerIngame.Instance.LocalPlayer.canMove = false;
        GameManagerIngame.Instance.LocalPlayer.canJump = false;

        scoreObject.SetActive(false);
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

        collectableCoinCount = 0;

        GameManagerIngame.Instance.LocalPlayer.canMove = true;
        GameManagerIngame.Instance.LocalPlayer.canJump = true;

        GameManagerIngame.Instance.LocalPlayer.Arrow.SetActive(false);


        roundNumber++;

        CreateRound();
        yield return new WaitForSeconds(2f);

        scoreObject.SetActive(true);

        StartCoroutine(CollectableScoreGenerator());
    }

    private void GetRandomLevel()
    {
        if (roundNumber < 2)
        {
            randomWorld = 1;
            randomLevel = Random.Range(7, 20);
        }
        else if (randomLevel < 5)
        {
            randomWorld = 2;
            randomLevel = Random.Range(1, 20);
        }
        else
        {
            randomLevel = Random.Range(1, 20);
            randomWorld = Random.Range(2, 4);
        }

        levelNumber = randomLevel * randomWorld;

        LevelCreator.SetupLevel(levelInfos2.Levels[randomLevel]);
    }

    public void NextRound()
    {
        if (roundNumber != 0)
            AdsManager.instance.ShowWinInterstitial();
        AdsManager.instance.RequestInterstitial();
        StartCoroutine(GenerateRound());
    }

    void GetJsonData()
    {

        if (jsonData == "")
        {
            string path = "";

#if UNITY_ANDROID
            path = "jar:file://" + Application.dataPath + "!/assets/sample.json";
            WWW www = new WWW(path);
            while (!www.isDone) { }
            jsonData = www.text;
            levelInfos1 = JsonUtility.FromJson<JsonDataConvert>(jsonData);

            path = "jar:file://" + Application.dataPath + "!/assets/emojis.json";
            WWW www2 = new WWW(path);
            while (!www2.isDone) { }
            jsonData = www2.text;
            levelInfos2 = JsonUtility.FromJson<JsonDataConvert>(jsonData);

#endif

#if UNITY_EDITOR || UNITY_IOS
            path = Application.streamingAssetsPath + "/sample.json";
            StreamReader reader = new StreamReader(path);
            jsonData = reader.ReadToEnd();
            levelInfos1 = JsonUtility.FromJson<JsonDataConvert>(jsonData);

            path = Application.streamingAssetsPath + "/emojis.json";
            StreamReader reader2 = new StreamReader(path);
            jsonData = reader2.ReadToEnd();
            levelInfos2 = JsonUtility.FromJson<JsonDataConvert>(jsonData);
#endif
        }
    }

    int collectableCoinCount = 0;

    IEnumerator CollectableScoreGenerator()
    {
        while (true)
        {
            yield return new WaitUntil(() => GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING);

            yield return new WaitForSeconds(Random.Range(6, 15));

            float rotation = Random.Range(-180, 180);

            if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING && collectableCoinCount < 2)
            {
                GameObject obj = Instantiate(collectableScore, new Vector3(0, 0, 0), new Quaternion(0, 0, rotation, 60));
                collectableCoinCount++;
                yield return new WaitForSeconds(Random.Range(8, 15));

                Destroy(obj);

            }
        }
    }

    IEnumerator CollectableCoinGenerator()
    {
        while (true)
        {
            yield return new WaitUntil(() => GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING);

            yield return new WaitForSeconds(Random.Range(10, 25));

            float rotation = Random.Range(-180, 180);

            if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING)
            {
                GameObject obj = Instantiate(collectableCoin, new Vector3(0, 0, 0), new Quaternion(0, 0, rotation, 60));

                yield return new WaitForSeconds(Random.Range(8, 15));

                if (obj != null)
                    Destroy(obj);
            }
        }
    }

    public void CollectScore(int collectedScore)
    {
        StartCoroutine(CollectAnimation(collectedScore));
    }

    IEnumerator CollectAnimation(int collectedScore)
    {
        scoreText.gameObject.transform.DOScale(1.7f, 0.2f);
        score += collectedScore / 3;
        yield return new WaitForSeconds(0.2f);

        scoreText.gameObject.transform.DOScale(1.7f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        score += collectedScore / 3;
        scoreText.gameObject.transform.DOScale(1.7f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        score += collectedScore / 3;

        scoreText.gameObject.transform.DOScale(1f, 0.2f);
    }

    IEnumerator FastCollectScore(int score)
    {
        for (int i = 0; i < score / 3; i++)
        {
            yield return new WaitForSeconds(0.1f);
            score += score / 3;
        }
    }


    public float levelTimer = 0;
    void Update()
    {
        if (isStarted && GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING && !GameManagerIngame.Instance.isPaused)
        {
            time += Time.deltaTime;
            levelTimer += Time.deltaTime;

            if (levelTimer < 15)
            {
                if (GameManagerIngame.Instance.LocalPlayer.GetPlayerState() == Player.PlayerState.Running)
                    score += ((float)levelNumber / 400);
                scoreText.text = ((int)score).ToString();
            }

        }
        else
        {
            levelTimer = 0;
        }

    }

}
