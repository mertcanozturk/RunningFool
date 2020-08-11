using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    #region SerializeFields
    [SerializeField] private float timeCounter;
    [SerializeField] private TextMeshProUGUI levelText, nextLevelText, completionRate, planetName, levelFailedCounterText, AlfuCoinText, reviveCoinText;
    [SerializeField] private Transform spriteMaskTransform;
    [SerializeField] private GameObject spriteMaskPrefab;
    [SerializeField] private List<GameObject> maskPool = new List<GameObject>();
    [SerializeField] private Slider slider, levelRateSlider;
    [SerializeField] private WatchAdsUnlockSkin watchAdsUnlockSkin;
    [SerializeField] private Image reviveRadialImage;

    #region Public Variables

    public int level = 1;
    public int SpriteMaskCount;
    public bool canTurn;
    public delegate void UpLevel();
    public event UpLevel LevelUpEvent;

    public GameObject insideMask;
    public GameObject outsideMask;

    public GameObject levelFailedPanel, PausePanel, completePanel, storePanel;

    public LevelCreator LevelCreator
    {
        get
        {
            if (_levelCreator == null)
                _levelCreator = FindObjectOfType<LevelCreator>();
            return _levelCreator;
        }
    }

    #endregion

    [Header("Multiplayer References")]
    [SerializeField] private GameObject LosePanel;
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private Sprite LosePanelBackground, WinPanelBackground;
    [SerializeField] private Image BackgroundImage;
    [SerializeField] private ObjectCreator objectCreator;
    [SerializeField] private GameObject WinLosePlayer;
    [SerializeField] private Animator WinLosePlayerAnimator;
    [SerializeField] private GameObject winParticle;
    [SerializeField] private SkinnedMeshRenderer WinLosePlayerMeshRenderer;
    [SerializeField] private Button btnPause;
    [SerializeField] private TMP_Text playerText;


    [Header("Contest References")]
    [SerializeField] private Contest contest;

    public GameObject alfucoinBar, pauseButton;

    public Image backgroundImage;

    public Sprite completeBackground, mainBackground;

    public TMP_Text score, timeText, mainScoreText;


    #endregion

    #region Local Variables
    private bool[] spriteMasks;

    private string jsonData = "";
    private JsonDataConvert levelInfos;
    private LevelCreator _levelCreator;
    private UIManager uiManager;
    private bool isReviveTime = false;
    private float reviveTime = 0;
    #endregion



    #region MonoBehavior
    private void Start()
    {

        spriteMasks = new bool[180];
        StartCoroutine(StartGame());
        AdsManager.instance.RequestInterstitial();


    }

    IEnumerator StartGame()
    {
        yield return new WaitUntil(() => GameManagerIngame.Instance.LocalPlayer != null);

        if (GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.SinglePlayer &&
            GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.Contest)
        {

            playerText.text = GameManager.Player.NamePlayer;
            string materialPath = MetaData.ConstVariable.Character.Find(PlayerPrefs.GetString("SelectedCharacter")).materialPath;
            WinLosePlayerMeshRenderer.material = Resources.Load<Material>(materialPath);
        }
        else if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
        {
            InitializeParameters();
        }
        else if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
        {
            int reviveCoin = (int)(5 * Mathf.Pow((PlayerPrefs.GetInt("DailyReviveCount") + 1), 2) + 20);
            reviveCoinText.text = reviveCoin.ToString();
        }
    }

    private void FixedUpdate()
    {
        if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING)
        {
            if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer
                || GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
                CreateMaskIstance();
        }

    }
    private void Update()
    {
        levelRateSlider.value = (SpriteMaskCount / 179.0f);
        AlfuCoinText.text = ShopManager.Instance.AlfuCoin.ToString();

        if (isReviveTime)
        {
            reviveTime += Time.deltaTime;
            reviveRadialImage.fillAmount = 1 - reviveTime / 5;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LevelUp();
        }
    }

    #endregion

    #region Private Methods
    private void DeSerializeJsonData()
    {
        if (jsonData == "")
        {
            string path = "";
            planetName.text = MetaData.ConstVariable.Planet.Find(PlayerPrefs.GetString("OpenedWorld")).name;


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

        LevelCreator.SetupLevel(levelInfos.Levels[level - 1]);

        LevelCreator.SetupCharacterSpeed(levelInfos.Levels[level - 1].CharacterSpeed);

        LevelCreator.SetupEnemies(levelInfos.Levels[level - 1].Enemies);

        LevelCreator.StartCoroutine(LevelCreator.StartCoinCreateProbability(levelInfos.Levels[level - 1].CoinRate));

        LevelCreator.SetupBackgroundImage(levelInfos.Levels[level - 1].Background);
    }
    private void CreateMaskIstance()
    {
        if (LevelCreator.transform.rotation.eulerAngles.z > 0)
        {
            if (spriteMasks[Mathf.FloorToInt(LevelCreator.transform.rotation.eulerAngles.z) / 2] != true &&
                    GameManagerIngame.Instance.LocalPlayer.GetPlayerState() == Player.PlayerState.Running)  // if sprite mask is not created in the same position
            {
                if (maskPool.Count < 181)  // Mask Object Pooling
                {
                    GameObject mask = Instantiate(spriteMaskPrefab, spriteMaskTransform.position, spriteMaskTransform.rotation); //create Sprite Mask
                }
                else
                {
                    maskPool[0].transform.position = spriteMaskTransform.position;
                    maskPool[0].transform.rotation = spriteMaskTransform.rotation;
                    maskPool.RemoveAt(0);
                }

                SpriteMaskCount++;
                SetCompletionRate();
                spriteMasks[Mathf.FloorToInt(LevelCreator.gameObject.transform.rotation.eulerAngles.z) / 2] = true; // save spriteMask rotation.

                if (SpriteMaskCount == 180)
                {
                    if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                        LevelUp();
                    else if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
                        NextRound();

                }

            }
        }
    }

    private void SetCompletionRate()
    {
        int rate = SpriteMaskCount * 100 / 180;
        completionRate.text = rate.ToString();
    }

    private IEnumerator ExecuteContinue()
    {
        GameManagerIngame.Instance.LocalPlayer.SetPlayerState(Player.PlayerState.Idle);
        HideFailedPanel();
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.WAITING;
        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
        {
            GameManagerIngame.Instance.LocalPlayer.canMove = true;
            GameManagerIngame.Instance.LevelManager.canTurn = true;
        }
        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator TimeCount()
    {
        while (true)
        {
            if (!GameManagerIngame.Instance.isPaused && (GameManagerIngame.Instance.LocalPlayer.GetPlayerState() == Player.PlayerState.Jumping || GameManagerIngame.Instance.LocalPlayer.GetPlayerState() == Player.PlayerState.Running))
            {
                yield return new WaitForSeconds(1f);
                timeCounter += 1;
                GameManagerIngame.Instance.GamePlayTime += 1;
            }
            yield return null;
        }

    }

    private void LevelUp()
    {
        PlayerPrefs.SetInt(PlayerPrefs.GetString("OpenedWorld") + "_Level_" + level + "_Passed", 1);

        Debug.Log("LevelUP");

        int score = level * 3 + (int)Mathf.Clamp(40 - timeCounter, 0, 100 - timeCounter);

        AchievementsManager.Instance.LevelUp(level - 1, timeCounter, score);

        AchievementsManager.Instance.ResetLevelCounters();

        if (PlayerPrefs.HasKey(PlayerPrefs.GetString("OpenedWorld") + "_" + level + "_score"))
        {
            GameManagerIngame.Instance.Score -= PlayerPrefs.GetInt(PlayerPrefs.GetString("OpenedWorld") + "_" + level + "_score");
        }

        GameManagerIngame.Instance.Score += score;
        PlayerPrefs.SetInt(PlayerPrefs.GetString("OpenedWorld") + "_" + level + "_score", score);

        ServerData.Instance.SaveLevelData(MetaData.ConstVariable.Planet.FindIndex(PlayerPrefs.GetString("OpenedWorld")), level, score);

        StartCoroutine(ExecuteLevelUp());

        GameManager.instance.SoundManager.PlayWinSound();

    }

    private IEnumerator ExecuteLevelUp()
    {
        PlayerPrefs.SetInt(PlayerPrefs.GetString("OpenedWorld") + level, 1);

        level++;
        GameManagerIngame.Instance.LocalPlayer.canJump = false;

        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.SetInt("LevelSelected", 1);
        DestroyObjects();
        //player.HidePlayerMesh();
        GameManagerIngame.Instance.LocalPlayer.SetDeactiveRunParticle();
        GameManagerIngame.Instance.LocalPlayer.SetPlayerState(Player.PlayerState.Winner);
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.DANCE;
        GameManagerIngame.Instance.LocalPlayer.PlayerWinAnimation();
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        insideMask.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
        GameManagerIngame.Instance.LocalPlayer.GetComponent<SpriteRenderer>().enabled = false;
        outsideMask.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(4f);


        AdsManager.instance.ShowWinInterstitial();


        if (level > 20)
        {

            PlayerPrefs.SetInt("CurrentLevel", 1);
            PlayerPrefs.SetInt("LevelSelected", 1);

            //For go to next planet 
            int planetIndex = 0;
            foreach (var planet in MetaData.ConstVariable.Planet.planets)
            {

                if (planet.key == PlayerPrefs.GetString("OpenedWorld"))
                {
                    if (planetIndex < MetaData.ConstVariable.Planet.planets.Length)
                    {
                        PlayerPrefs.SetString("OpenedWorld", MetaData.ConstVariable.Planet.planets[planetIndex + 1].key);
                        PlayerPrefs.SetInt("LevelSelected", 1);
                        PlayerPrefs.SetInt("CurrentLevel", 1);
                        PlayerPrefs.SetString("OpenedJsonFile", MetaData.ConstVariable.Planet.planets[planetIndex + 1].jsonFileName);
                        break;
                    }
                    else
                    {
                        PlayerPrefs.SetInt("CurrentLevel", 20);
                        PlayerPrefs.SetInt("LevelSelected", 1);
                    }
                }

                planetIndex++;
            }



            GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.MAIN_MENU;
            SceneManager.LoadScene(MetaData.ConstVariable.Scenes.MainScene);
        }
        else
        {
            PlayerPrefs.SetInt("CurrentLevel", level);
            PlayerPrefs.SetInt("LevelSelected", 1);
            ResetLevel();
            GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.LEVEL_UP;
            completePanel.SetActive(true);
            watchAdsUnlockSkin.RandomWatchAdsUnlockSkin();

            if (LevelUpEvent != null)
                LevelUpEvent();

        }

    }

    private void Pause()
    {
        Time.timeScale = 0;
        GameManagerIngame.Instance.isPaused = true;
    }

    private void Continue()
    {
        Time.timeScale = 1;
        GameManagerIngame.Instance.isPaused = false;
    }

    IEnumerator ReviveCounter(int inSec)
    {
        int sec = 0;
        reviveRadialImage.fillAmount = 1;
        reviveTime = 0;
        isReviveTime = true;
        while (sec < inSec)
        {
            levelFailedCounterText.text = (inSec - sec).ToString();
            yield return new WaitForSeconds(1f);
            sec++;
        }
        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
        {
            AlfucodeAPIContest.instance.CreatePlayerStatisticDefinition(AlfucodeAPIContest.instance.playerAround.Leaderboard[0].StatValue + (int)GameManagerIngame.Instance.Contest.score);
            GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.MAIN_MENU;
            SceneManager.LoadScene(MetaData.ConstVariable.Scenes.MainScene);
        }
        else
        {
            Retry(); levelFailedPanel.SetActive(false);
            isReviveTime = false;
        }

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// This method gives the initial values of the variables required for the game to start.
    /// </summary>
    public void InitializeParameters()
    {
        Time.timeScale = 1;
        StopAllCoroutines();
        StartCoroutine(TimeCount());
        timeCounter = 0;
        if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.LEVEL_UP)
        {
            GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.PLAYING;
        }
        else
        {
            GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.WAITING;
        }


        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            if (PlayerPrefs.GetInt("LevelSelected") == 1)
            {
                level = PlayerPrefs.GetInt("CurrentLevel");

                PlayerPrefs.SetInt("LevelSelected", 0);
                GameManagerIngame.Instance.LocalPlayer.canJump = false;
            }
        }
        else
        {
            GameManagerIngame.Instance.LocalPlayer.canJump = false;

            level = 1; // for first gameplay
        }

        if (level == 1 && PlayerPrefs.GetString("OpenedWorld") == MetaData.ConstVariable.Planet.planets[0].key)
        {
            GameManagerIngame.Instance.LocalPlayer.canJump = false;
            pauseButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            pauseButton.GetComponent<Button>().interactable = true;
            if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                GameManagerIngame.Instance.LocalPlayer.tutorial.gameObject.SetActive(false);
            GameManagerIngame.Instance.LocalPlayer.canJump = true;
        }

        GameManagerIngame.Instance.LocalPlayer.SetDeactiveRunParticle();

        if (GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.SinglePlayer)
            return;

        levelText.text = level.ToString();

        int reviveCoin = (int)(5 * Mathf.Pow((PlayerPrefs.GetInt("DailyReviveCount") + 1), 2) + 20);
        reviveCoinText.text = reviveCoin.ToString();

        if (level == 20)
        {
            nextLevelText.text = "-";
        }
        else
        {
            nextLevelText.text = (level + 1).ToString();
        }

        spriteMasks = new bool[180];
        SpriteMaskCount = 0;
        //insideMask = GameObject.FindGameObjectWithTag("InsideMask");
        //  outsideMask = GameObject.FindGameObjectWithTag("OutsideMask");

        DeSerializeJsonData();
    }

    /// <summary>
    /// This method destroys enemy and mask objects in the scene.
    /// </summary>
    public void DestroyObjects()
    {
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy")) // Destroy all enemies
        {
            Destroy(enemy);
        }
        maskPool.Clear();
        foreach (var mask in GameObject.FindGameObjectsWithTag("Mask")) // Adding into Mask Pool
        {
            mask.transform.position = new Vector3(2000, 2000, 0);
            maskPool.Add(mask);
        }
        foreach (var score in GameObject.FindGameObjectsWithTag("Score")) // Destroy all enemies
        {
            Destroy(score);
        }

    }
    /// <summary>
    /// This method resets the scene after leveling.
    /// </summary>

    public void ResetLevel()
    {
        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer || GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
        {
            GameManagerIngame.Instance.LocalPlayer.ShowPlayerMesh();
            insideMask.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            GameManagerIngame.Instance.LocalPlayer.GetComponent<SpriteRenderer>().enabled = true;
            outsideMask.GetComponent<SpriteRenderer>().enabled = true;
            LevelCreator.StopAllCoroutines();
            LevelCreator.transform.rotation = new Quaternion(0, 0, 0, 0);
            spriteMasks = new bool[180];
            SpriteMaskCount = 0;
            SetCompletionRate();
            GameManagerIngame.Instance.LocalPlayer.ResetPlayer();
        }

    }

    public void ShowMultiplayerDefeat()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(item);
        }

        btnPause.gameObject.SetActive(false);
        WinLosePlayer.SetActive(true);
        WinLosePlayerAnimator.runtimeAnimatorController = Resources.Load("Animators/LoseAnimatorController") as RuntimeAnimatorController;
        objectCreator.FinishGame();
        LosePanel.SetActive(true);
        BackgroundImage.sprite = LosePanelBackground;
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        insideMask.SetActive(false);
        outsideMask.SetActive(false);

        AdsManager.instance.ShowLoseInterstitial();
    }

    public void ShowMultiplayerWin()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(item);
        }


        btnPause.gameObject.SetActive(false);
        WinLosePlayerAnimator.runtimeAnimatorController = Resources.Load("Animators/WinAnimatorController") as RuntimeAnimatorController;

        WinLosePlayer.SetActive(true);
        winParticle.SetActive(true);
        objectCreator.FinishGame();

        WinPanel.SetActive(true);
        BackgroundImage.sprite = WinPanelBackground;

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        insideMask.SetActive(false);
        outsideMask.SetActive(false);

        AdsManager.instance.ShowWinInterstitial();
    }

    public void ShowFailedPanel()
    {
        AdsManager.instance.ShowLoseInterstitial();
        levelFailedPanel.SetActive(true);
        StartCoroutine(ReviveCounter(5));
    }
    public void HideFailedPanel()
    {
        levelFailedPanel.SetActive(false);
    }
    public void SkipLevel()
    {
        LevelUp();
    }
    public void Retry()
    {
        ResetLevel();
        DestroyObjects();
        InitializeParameters();
    }
    public void Revive()
    {
        Time.timeScale = 1;
        StartCoroutine(ExecuteContinue());
        AchievementsManager.Instance.IncreaseReviveTimes();
    }
    public int Level
    {
        get { return level; }
    }

    #endregion


    #region Contest Methods

    private void NextRound()
    {
        DestroyObjects();
        ResetLevel();
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.LEVEL_UP;

        backgroundImage.sprite = completeBackground;

        GameManagerIngame.Instance.LocalPlayer.gameObject.SetActive(false);

        outsideMask.SetActive(false);
        insideMask.SetActive(false);
        mainScoreText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        WinLosePlayer.SetActive(true);
        score.text = "Score : +" + ((int)GameManagerIngame.Instance.Contest.score).ToString();
        completePanel.SetActive(true);

        alfucoinBar.SetActive(false);
        pauseButton.SetActive(false);

        watchAdsUnlockSkin.RandomWatchAdsUnlockSkin();
    }

    #endregion

    #region UI Functions
    public void Back()
    {
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.MAIN_MENU;

        GameManagerIngame.Instance.isPaused = false;
        Time.timeScale = 1;

        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
        {
            AlfucodeAPIContest.instance.CreatePlayerStatisticDefinition(AlfucodeAPIContest.instance.playerAround.Leaderboard[0].StatValue + (int)GameManagerIngame.Instance.Contest.score);
        }

        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.MainScene);

    }

    public void MultiplayerRetry()
    {
        AdsManager.instance.ShowRetryInterstitial();
        Photon.Pun.PhotonNetwork.Destroy(SyncSceneScript.SyncSceneObject.gameObject);
        Photon.Pun.PhotonNetwork.Disconnect();
        GameManagerIngame.Instance.GameMode = GameManagerIngame.Mode.Multiplayer2;
        GameManager.instance.GameMode = MetaData.GameMode.Online;
        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.SyncScean, LoadSceneMode.Single);

    }

    public void SkipLevelButton()
    {
#if UNITY_IOS || UNITY_ANDROID
        AdsManager.instance.rewardAdsTypes = AdsManager.RewardAdsTypes.SkipLevel;
        AdsManager.instance.ShowRewardedAd();
        AdsManager.instance.CreateAndLoadRewardedAd();
#endif

#if UNITY_EDITOR

        SkipLevel();
        HideFailedPanel();
#endif
    }

    public void GoToShop()
    {
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.SHOP;

        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.ShopScene);
        Time.timeScale = 1;

    }

    public void GoToLevels()
    {
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.LEVELS;
        GameManagerIngame.Instance.LocalPlayer.ResetPlayer();

        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.LevelsScene);
        Time.timeScale = 1;

    }

    public void RetryButton()
    {
        AdsManager.instance.ShowRetryInterstitial();

        Retry();
        HideFailedPanel();
        GameManagerIngame.Instance.LocalPlayer.ResetPlayer();
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.WAITING;
        PausePanel.SetActive(false);
        completePanel.SetActive(false);

    }

    public void ReviveButton()
    {
        StopAllCoroutines();
        AdsManager.instance.rewardAdsTypes = AdsManager.RewardAdsTypes.Revive;
        AdsManager.instance.ShowRewardedAd();
    }

    public void ReviveWithCoin()
    {
        PlayerLog.Instance.IncreaseReviveButtonCounter();

        int reviveCoin = (int)(5 * Mathf.Pow((PlayerPrefs.GetInt("DailyReviveCount") + 1), 2) + 20);

        if (ShopManager.Instance.AlfuCoin >= reviveCoin)
        {
            StopAllCoroutines();

            ShopManager.Instance.AddAlfuCoin(-reviveCoin);

            PlayerPrefs.SetInt("DailyReviveCount", PlayerPrefs.GetInt("DailyReviveCount") + 1);

            reviveCoin = (int)(5 * Mathf.Pow((PlayerPrefs.GetInt("DailyReviveCount") + 1), 2) + 20);

            reviveCoinText.text = reviveCoin.ToString();
            Revive();
            levelFailedPanel.SetActive(false);

            reviveCoin = (int)(5 * Mathf.Pow((PlayerPrefs.GetInt("DailyReviveCount") + 1), 2) + 20);
            reviveCoinText.text = reviveCoin.ToString();
        }
        else
        {
            Time.timeScale = 0;
            storePanel.SetActive(true);
        }
    }

    public void PauseButton()
    {
        PlayerLog.Instance.IncreasePauseButtonCounter();
        Pause();
        PausePanel.SetActive(true);
    }
    public void ContinueButton()
    {
        Continue();
        PausePanel.SetActive(false);
    }

    public void LevelCompleteRetryButton()
    {
        PlayerLog.Instance.IncreaseRetryButtonCounter();
        level--;
        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.SetInt("LevelSelected", 1);

        Retry();
        HideFailedPanel();
        PausePanel.SetActive(false);
        completePanel.SetActive(false);


    }
    public void LevelCompleteContinueButton()
    {
        PlayerLog.Instance.IncreaseContinueButtonCounter();
        completePanel.SetActive(false);
        ResetLevel();
        InitializeParameters();


        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
        {
            outsideMask.SetActive(true);
            insideMask.SetActive(true);
            WinLosePlayer.SetActive(false);
            backgroundImage.sprite = mainBackground;

            GameManagerIngame.Instance.LocalPlayer.gameObject.SetActive(true);

            GameManagerIngame.Instance.Contest.NextRound();
            pauseButton.SetActive(true);
            alfucoinBar.SetActive(true);
        }
    }



    #endregion
}
