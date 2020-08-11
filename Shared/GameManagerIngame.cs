using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerIngame : Singleton<GameManagerIngame>
{
    public event System.Action<Player> OnLocalPlayerJoined;

    public bool isPaused;
    public bool isChallengeGame = false;

    public int gamePlayTime;
    public int watchAdsWithSkin = 3;
    public int GamePlayTime
    {
        get { return gamePlayTime; }
        set
        {
            gamePlayTime = value;

            PlayerPrefs.SetInt("GamePlayTime", value);
        }
    }

    public int score;
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            PlayerPrefs.SetInt("TotalScore", score);
            AlfucodeAPILeaderBored.instance.CreatePlayerStatisticDefinition(score);

            foreach (var item in MetaData.ConstVariable.Achievements.Find(MetaData.ConstVariable.Achievements.Type.Reach))
            {
                if (item.times > score)
                {
                    item.completed = true;
                }
            }
        }
    }

    public enum Mode { SinglePlayer, Multiplayer2, Multiplayer3, Multiplayer4, Contest }

    public Mode GameMode;

    private void Start()
    {
        DontDestroyOnLoad(this);

        PlayerPrefs.SetInt("MainOpenCount", 0);

        if (PlayerPrefs.HasKey("TotalScore"))
        {
            score = PlayerPrefs.GetInt("TotalScore");
        }
        if (PlayerPrefs.HasKey("GamePlayTime"))
        {
            gamePlayTime = PlayerPrefs.GetInt("GamePlayTime");
        }

        isPaused = true;
        Application.targetFrameRate = 300;
        GameState = GAMESTATE.ENTRY;
        GameMode = Mode.SinglePlayer;
    }


    public Player m_LocalPlayer;
    public Player LocalPlayer
    {
        get
        {
            return m_LocalPlayer;
        }
        set
        {
            m_LocalPlayer = value;

            if (OnLocalPlayerJoined != null)
            {
                OnLocalPlayerJoined(m_LocalPlayer);
            }
        }
    }

    public AIPlayer m_AIPlayer;
    public AIPlayer AIPlayer
    {
        get
        {
            return m_AIPlayer;
        }
        set
        {
            m_AIPlayer = value;
        }
    }


    private LevelManager m_levelManager;
    public LevelManager LevelManager
    {
        get
        {
            if (m_levelManager == null)
                m_levelManager = FindObjectOfType<LevelManager>();
            return m_levelManager;
        }

    }
    ObjectCreator m_ObjectCreator;
    public ObjectCreator ObjectCreator
    {
        get
        {
            if (m_ObjectCreator == null)
                m_ObjectCreator = FindObjectOfType<ObjectCreator>();
            return m_ObjectCreator;
        }
    }

    Contest m_Contest;
    public Contest Contest
    {
        get
        {
            if (m_Contest == null)
                m_Contest = FindObjectOfType<Contest>();
            return m_Contest;
        }
    }

    ContestUI m_ContestUI;
    public ContestUI ContestUI
    {
        get
        {
            if (m_ContestUI == null)
                m_ContestUI = FindObjectOfType<ContestUI>();
            return  m_ContestUI;;
        }
    }

    MainManager m_MainManager;
    public MainManager MainManager
    {
        get
        {
            if (m_MainManager == null)
                m_MainManager = FindObjectOfType<MainManager>();
            return m_MainManager;
        }
    }

    public enum GAMESTATE
    {
        ENTRY,
        MAIN_MENU,
        PLANETS,
        LEVELS,
        LOADING,
        WAITING,
        PAUSED,
        PLAYING,
        DANCE,
        LEVEL_UP,
        GAME_OVER,
        SHOP,
        TUTORIAL
    }

    public GAMESTATE m_gameState;

    /// <summary>
    /// This method sets GAMESTATE enum variable and calls LocalPlayer and LevelManager methods.
    /// </summary>
    public GAMESTATE GameState
    {
        get
        {
            return m_gameState;
        }
        set
        {
            m_gameState = value;
            switch (m_gameState)
            {
                case GAMESTATE.ENTRY:
                    //SceneManager.LoadScene(MetaData.ConstVariable.Scenes.MainScene);
                    break;
                case GAMESTATE.MAIN_MENU:
                    break;
                case GAMESTATE.LOADING:
                    break;
                case GAMESTATE.WAITING:
                    if (LocalPlayer != null)
                        LocalPlayer.SetPlayerState(Player.PlayerState.Idle);
                    isPaused = true;
                    break;
                case GAMESTATE.PAUSED:
                    if (LocalPlayer != null)
                        LocalPlayer.SetPlayerState(Player.PlayerState.Idle);
                    isPaused = true;
                    break;
                case GAMESTATE.PLAYING:
                    if (LocalPlayer != null)
                        LocalPlayer.SetPlayerState(Player.PlayerState.Running);
                    isPaused = false;
                    break;
                case GAMESTATE.LEVEL_UP:
                    //isPaused = true;
                    break;
                case GAMESTATE.GAME_OVER:
                    if (GameMode == Mode.SinglePlayer || GameMode == Mode.Contest)
                    {
                        LevelManager.ShowFailedPanel();
                        isPaused = true;
                    }

                    /*
                    LevelManager.ResetLevel();
                    LevelManager.DestroyObjects();
                    LevelManager.InitializeParameters();
                    */
                    break;
                default:
                    break;
            }
        }
    }

}
