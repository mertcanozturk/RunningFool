using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum PlayerState { Idle = 0, Running = 1, Jumping = 2, Death = 3, Winner = 4, TutorialCheckPoint1, TutorialCheckPoint2, TutorialCheckPoint3, }

    #region SerializeFields

    [SerializeField] private float maxJumpTime, minJumpTime;
    [SerializeField] private int maxJump;
    [SerializeField] private GameObject runParticle;
    [SerializeField] private Slider jumpSlider;

    #endregion

    #region Local Variables

    private int jumpCounter;
    private float endJumpTime;
    private bool endJumping = false;
    #endregion

    #region Public Variables

    public int tutorialCounter = 0;
    public bool canMove = true;
    public bool _isGrounded = true;
    public bool canJump = true;
    public float jumpTime;
    public Vector3 tutorial2PlayerPosition;
    public Vector3 tutorial2PlayerRotation;
    public PlayerState playerState;
    public GameObject PlayerMesh;
    public Transform jumpingTransform, idleTransform, instantiateTransform, winnerTransform;
    public SkinnedMeshRenderer meshRenderer;
    public Tutorial tutorial;
    public GameObject Arrow;
    public Outline_Controller outline_Controller;
    public bool isOfflinePlayer = false;
    #endregion

    #region Components

    private Animator m_animator;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    public PhotonView pV;


    private Animator Animator
    {
        get
        {
            if (m_animator == null && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                m_animator = GetComponentInChildren<Animator>();
            if (m_animator == null && GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.SinglePlayer)
                m_animator = GetComponent<Animator>();
            return m_animator;
        }
    }

    #endregion

    #region MonoBehavior Methods
    private void Awake()
    {
        if (isOfflinePlayer && !GameManager.instance.IsAIMode) gameObject.SetActive(false);

        if (GameManager.instance.IsAIMode)
            meshRenderer.material = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(PlayerPrefs.GetString("SelectedCharacter")).materialPath);
    }
    private void Start()
    {
        GetComponents();
        InitializeParameters();
        TutorialControl();

        if (GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.SinglePlayer)
            canMove = false;

        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer || GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
        {
            SetPlayerState(PlayerState.Idle);

        }
        else
        {
            playerState = PlayerState.Idle;
        }
    }

    void StartOtherPlayer()
    {
        // Make Smoothness fade
        Arrow.SetActive(false);

        // Set Position of Player
    }


    void StartMyPlayer()
    {
        // Make Smoothness good 
        // SetPositionOfPlayer
        //
        Arrow.SetActive(true);
        canMove = false;
        GameManagerIngame.Instance.LocalPlayer = this;
        if (!GameManager.instance.IsAIMode)
            pV.RPC(nameof(SyncMeshMaterial), RpcTarget.AllBuffered, MetaData.ConstVariable.Character.Find(PlayerPrefs.GetString("SelectedCharacter")).materialPath);
    }


    void TutorialControl()
    {
        if (GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.SinglePlayer) return;

        if (GameManagerIngame.Instance.LevelManager.Level == 1 && PlayerPrefs.GetString("OpenedWorld") == MetaData.ConstVariable.Planet.planets[0].key && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
        {
            GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.TUTORIAL;
            tutorial.ShowTouchTutorial();
            canJump = false;
        }
    }

    void Update()
    {
        if (canMove && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
            GetInput();

        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest && canMove)
            GetInput();
        else if (GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.SinglePlayer && canMove && !GameManager.instance.IsAIMode && pV.IsMine)
            GetInput();

        if (canMove && GameManager.instance.IsAIMode && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Multiplayer2)
            GetInput();

        if (playerState == PlayerState.Death && !IsGrounded && GameManagerIngame.Instance.GameState != GameManagerIngame.GAMESTATE.TUTORIAL)
            EndJump();

        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
            jumpSlider.value = jumpTime / maxJumpTime;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.SinglePlayer)
        {
            return;
        }

        if (collision.tag == "Tutorial" && tutorialCounter == 2 && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
        {
            tutorialCounter++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Multiplayer2)
        {
            if (!GameManager.instance.IsAIMode && pV != null)
                if (!pV.IsMine) return;
        }

        if (collision.tag == "OutsideMask")
        {
            jumpTime = 0;
            jumpCounter = 0;
            _isGrounded = true;

            if (GameManagerIngame.Instance.LevelManager.Level == 1 && PlayerPrefs.GetString("OpenedWorld") == MetaData.ConstVariable.Planet.planets[0].key && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
            {
                canJump = false;
            }
            else
            {
                canJump = true;
            }

            if (playerState == PlayerState.Jumping)
            {
                endJumping = false;
                SetPlayerState(PlayerState.Running);
            }

            if (playerState != PlayerState.Death && playerState != PlayerState.Winner)
            {
                if (GameManagerIngame.Instance.GameState != GameManagerIngame.GAMESTATE.WAITING)
                    GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.PLAYING;

                if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING)
                    runParticle.SetActive(true);
            }
        }

        if (GameManagerIngame.Instance.GameState != GameManagerIngame.GAMESTATE.GAME_OVER)
        {
            switch (collision.tag)
            {
                case "Enemy":
                    if (Settings.Instance.vibrationEnabled)
                        Vibration.Vibrate(250);
                    GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.GAME_OVER;
                    SetPlayerState(PlayerState.Death);

                    AchievementsManager.Instance.IncreaseDieCounter();
                    if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                    {
                        Destroy(collision.transform.parent.gameObject);

                    }
                    else
                    {
                        if (GameManager.instance.IsAIMode)
                            StartCoroutine(ExecuteShowMultiplayerDefeat());
                        else if (GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.Contest)
                        {
                            pV.RPC(nameof(MultiplayerWin), RpcTarget.Others);


                            StartCoroutine(ExecuteShowMultiplayerDefeat());

                            //pV.RPC(nameof(FinishMultiplayerGame), RpcTarget.All);
                        }
                        else
                        {
                            Destroy(collision.transform.parent.gameObject);

                            //StartCoroutine(ExecuteGoToMain(4));

                        }
                    }

                    if (Settings.Instance.gameSoundEnabled)
                    {
                        GameManager.instance.SoundManager.CollisionSound.Play();
                        GameManager.instance.SoundManager.PlayDeathSound();
                    }

                    break;
                case "Coin":
                    ShopManager.Instance.AddAlfuCoin(5);
                    Destroy(collision.gameObject);
                    break;
                case "Tutorial":
                    if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                    {
                        canJump = true;
                        if (GameManagerIngame.Instance.LevelManager.canTurn)
                        {
                            if (tutorialCounter == 0)
                            {
                                tutorial.ShowHoldItTutorial();
                            }
                            else if (tutorialCounter == 1)
                            {
                                tutorial.ShowHoldIt2Tutorial();
                            }
                            else if (tutorialCounter == 2)
                            {
                                tutorial.ShowHoldIt2Tutorial();
                                //jumpTime = jumpTime / 2;
                                Time.timeScale = 0;
                            }
                            else if (tutorialCounter == 3)
                            {
                                tutorial.ShowCompleteTutorial();
                                Time.timeScale = 0;
                            }
                        }

                        GameManagerIngame.Instance.isPaused = true;
                        GameManagerIngame.Instance.LevelManager.canTurn = false;
                        GameManagerIngame.Instance.LocalPlayer.SetPlayerState(Player.PlayerState.Idle);
                        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.TUTORIAL;
                        canMove = true;
                    }

                    break;
                default:
                    break;
            }
        }
    }

    #endregion

    #region Private Methods
    private void GetComponents()
    {
        m_animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        if (!GameManager.instance.IsAIMode)
            pV = GetComponent<PhotonView>();


        if (GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.SinglePlayer && !GameManager.instance.IsAIMode && GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.Contest)
        {
            if (pV.IsMine)
            {
                StartMyPlayer();
            }
            else
            {
                StartOtherPlayer();
            }

            // Add it into LevelManager
        }
        else
        {
            GameManagerIngame.Instance.LocalPlayer = this;
        }
    }

    private void InitializeParameters()
    {
        SetPlayerState(PlayerState.Idle);
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.WAITING;
        PlayerMesh.SetActive(true);
    }

    [PunRPC]
    public void ReadyForOnline()
    {
        GameManagerIngame.Instance.ObjectCreator.Ready();
    }

    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.TUTORIAL && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
            {
                GameManagerIngame.Instance.LevelManager.canTurn = true;
                SetPlayerState(PlayerState.Running);
                GameManagerIngame.Instance.isPaused = false;

                tutorialCounter++;

                if (tutorialCounter == 1)
                {
                    Time.timeScale = 1;
                    tutorial.HideoldItTutorial();
                    GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.PLAYING;
                }
                else if (tutorialCounter == 2)
                {
                    Time.timeScale = 1;
                    maxJumpTime = 0.5f; minJumpTime = 0.01f;
                    tutorial2PlayerPosition = transform.position;
                    tutorial.HideoldIt2Tutorial();
                    GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.PLAYING;

                }
                else if (tutorialCounter == 3)
                {
                    Time.timeScale = 1;
                    tutorial.HideoldIt2Tutorial();
                    GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.PLAYING;
                    GameManagerIngame.Instance.LevelManager.LevelCreator.SetupCharacterSpeed(1.5f);


                }
                else if (tutorialCounter == 4)
                {
                    foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy")) // Destroy all enemies
                    {
                        Destroy(enemy);
                    }
                    canJump = true;
                    canMove = true;
                    tutorial.HideCompleteTutorial();
                    GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.WAITING;
                    GameManagerIngame.Instance.LevelManager.LevelCreator.SetupCharacterSpeed(1.5f);
                    Time.timeScale = 1;
                }

            }

            if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.WAITING)
            {
                if (tutorialCounter == 0 && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                {
                    tutorial.HideTouchTutorial();
                }

                if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                    AchievementsManager.Instance.IncreasePlayTimes();

                if (GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.SinglePlayer)
                {
                    //pV.RPC(nameof(ReadyForOnline), RpcTarget.All);
                }

                SetPlayerState(PlayerState.Running);


                if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer || GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest)
                {
                    runParticle.SetActive(true);
                    GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.PLAYING;

                    if (GameManagerIngame.Instance.LevelManager.Level == 1 && PlayerPrefs.GetString("OpenedWorld") == MetaData.ConstVariable.Planet.planets[0].key && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                    {
                        canJump = false;
                    }
                    else
                    {
                        canJump = true;
                    }
                }


            }

            else if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING && playerState != PlayerState.Jumping && canJump)
            {
                if (playerState != PlayerState.Jumping && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                {
                    AchievementsManager.Instance.IncreaseJumpCounter();
                }

                SetPlayerState(PlayerState.Jumping);
                runParticle.SetActive(false);

                if (jumpCounter < maxJump)
                {
                    if (jumpCounter < maxJump)
                        Jump();
                    else
                        EndJump();
                }
            }
            else if (canJump)
            {
                jumpCounter++;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (tutorialCounter == 2 && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
            {
                GameManagerIngame.Instance.isPaused = false;
                GameManagerIngame.Instance.LevelManager.canTurn = true;
                Time.timeScale = 1;
                Jump();
                tutorial.HideoldIt2Tutorial();

            }
            else if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING && canJump)
            {
                if (jumpTime > maxJumpTime)
                {
                    EndJump();
                }
                else
                {
                    if (jumpCounter < maxJump)
                        Jump();
                    else
                    {
                        EndJump();
                    }
                }
            }
        }
        else
        {
            if (tutorialCounter == 2 && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
            {
                GameManagerIngame.Instance.isPaused = false;
                GameManagerIngame.Instance.LevelManager.canTurn = true;
                canJump = false;
                Time.timeScale = 1;
                transform.position = tutorial2PlayerPosition;
                if (tutorialCounter == 2)
                    GameManagerIngame.Instance.LevelManager.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -140));
                tutorialCounter--;

                SetPlayerState(PlayerState.Running);
                tutorial.ShowHoldIt2Tutorial();
            }
            else
            {
                if (jumpTime < minJumpTime && playerState == PlayerState.Jumping)
                {
                    Jump();
                }
                else
                {
                    EndJump();
                }
            }

        }
    }

    private void Jump()
    {
        _isGrounded = false;
        if (playerState == PlayerState.Jumping && jumpTime < maxJumpTime)
        {
            jumpTime += Time.deltaTime;

            if (transform.localPosition.y <= 2.5f)
            {
                if (transform.localPosition.y < 2f && GameManagerIngame.Instance.LevelManager.Level == 1 && PlayerPrefs.GetString("OpenedWorld") == MetaData.ConstVariable.Planet.planets[0].key && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                {
                    if (tutorialCounter < 2)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, jumpingTransform.position, Time.deltaTime * 5f);

                    }
                    else
                    {
                        transform.position = Vector2.MoveTowards(transform.position, jumpingTransform.position, Time.deltaTime * 2f);

                    }
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, jumpingTransform.position, Time.deltaTime * (5f - jumpTime * 5));

                }

            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, jumpingTransform.position, Time.deltaTime);
            }

        }
        else
        {
            SetPlayerState(PlayerState.Running);
        }
    }

    private void EndJump()
    {
        endJumping = true;
        transform.position = Vector2.MoveTowards(transform.position, idleTransform.position, Time.deltaTime * 2.5f);
    }
    #endregion

    #region Public Methods
    public PlayerState GetPlayerState() { return playerState; }

    /// <returns>Returns _isGrounded.</returns>
    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
    }

    /// <summary>
    /// This Method sets playerState enum variables and animator parameters.
    /// </summary>
    /// <param name="newState">Enum value</param>
    public void SetPlayerState(PlayerState newState)
    {
        if (Animator != null)
        {
            switch (newState)
            {
                case PlayerState.Idle:
                    if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                    {
                        PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    }
                    else
                    {
                        if (!GameManager.instance.IsAIMode && GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.Contest)
                            pV.RPC(nameof(ChangeMeshRotation), RpcTarget.All, new Vector3(0, 180, 0));
                        else
                            PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

                    }
                    Animator.SetBool("isRunning", false);
                    Animator.SetBool("isJumping", false);
                    Animator.SetBool("isDead", false);
                    Animator.SetBool("isWon", false);

                    if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                        canMove = true;
                    runParticle.SetActive(false);
                    break;
                case PlayerState.Running:
                    if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                    {
                        PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    }
                    else
                    {
                        if (!GameManager.instance.IsAIMode && GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.Contest)
                            pV.RPC(nameof(ChangeMeshRotation), RpcTarget.All, new Vector3(0, 90, 0));
                        else
                            PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    }
                    Animator.SetBool("isRunning", true);
                    Animator.SetBool("isJumping", false);
                    runParticle.SetActive(true);
                    canMove = true;
                    break;
                case PlayerState.Jumping:
                    Animator.SetBool("isJumping", true);
                    Animator.SetBool("isRunning", false);
                    canMove = true;
                    break;
                case PlayerState.Death:
                    if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                    {
                        PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    }
                    else
                    {
                        if (!GameManager.instance.IsAIMode && GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.Contest)
                            pV.RPC(nameof(ChangeMeshRotation), RpcTarget.All, new Vector3(0, 180, 0));
                        else
                        {
                            PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

                            if (GameManager.instance.IsAIMode)
                                GameManagerIngame.Instance.AIPlayer.SetPlayerState(AIPlayer.PlayerState.Winner);

                        }

                    }

                    Animator.SetBool("isDead", true);
                    canMove = false;
                    if (GameManagerIngame.Instance.GameState != GameManagerIngame.GAMESTATE.GAME_OVER)
                    {
                        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.GAME_OVER;
                    }
                    break;
                case PlayerState.Winner:
                    if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                    {
                        PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    }
                    else
                    {
                        if (!GameManager.instance.IsAIMode && GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.Contest)
                            pV.RPC(nameof(ChangeMeshRotation), RpcTarget.All, new Vector3(0, 180, 0));
                        else
                            PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

                    }
                    Animator.SetFloat("DanceType", Random.Range(0, 2));
                    Animator.SetBool("isWon", true);
                    GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.DANCE;

                    break;
            }
            playerState = newState;
        }
    }


    /// <summary>
    /// this method resets the player position rotation and variables. Also stops all coroutines.
    /// </summary>
    public void ResetPlayer()
    {
        if (GameManagerIngame.Instance.LevelManager.Level == 1 && PlayerPrefs.GetString("OpenedWorld") == MetaData.ConstVariable.Planet.planets[0].key)
            canJump = false;
        else
            canJump = true;

        SetPlayerState(PlayerState.Idle);
        GameManagerIngame.Instance.isPaused = false;
        Time.timeScale = 1;
        tutorialCounter = 0;
        jumpCounter = 0;
        jumpTime = 0;
        StopAllCoroutines();
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.WAITING;
        SetDeactiveRunParticle();
        PlayerMesh.transform.localScale = instantiateTransform.localScale;
        PlayerMesh.transform.transform.localPosition = instantiateTransform.transform.localPosition;
    }

    public void SetDeactiveRunParticle()
    {
        runParticle.SetActive(false);
    }

    public void SetActiveRunParticle()
    {
        runParticle.SetActive(true);
    }

    public void HidePlayerMesh()
    {
        PlayerMesh.SetActive(false);

    }

    public void ShowPlayerMesh()
    {
        PlayerMesh.SetActive(true);
    }

    public void PlayerWinAnimation()
    {
        // StartCoroutine(ExecutePlayerWinAnimation());
    }

    IEnumerator ExecutePlayerWinAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            PlayerMesh.transform.localScale = Vector3.MoveTowards(PlayerMesh.transform.localScale, winnerTransform.localScale, Time.deltaTime * 10);
            PlayerMesh.transform.transform.position = Vector3.MoveTowards(PlayerMesh.transform.position, winnerTransform.transform.position, Time.deltaTime * 0.0015f);

        }
    }

    public void MultiplayerWinner()
    {
        SetDeactiveRunParticle();
        SetPlayerState(PlayerState.Winner);
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.DANCE;
        PlayerWinAnimation();

        if (!GameManager.instance.IsAIMode)
        {
            pV.RPC(nameof(ChangeMeshRotation), RpcTarget.All, new Vector3(0, 0, 0));
        }
        else
        {
            PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            StartCoroutine(ExecuteShowMultiPlayerWin());
            //StartCoroutine(ExecuteGoToMain(4));
        }

    }

    #endregion


    #region RPC
    [PunRPC]
    public void MultiplayerWin()
    {
        GameManagerIngame.Instance.LocalPlayer.MultiplayerWinner();


        StartCoroutine(ExecuteShowMultiPlayerWin());
        //StartCoroutine(ExecuteGoToMain(4));

    }

    IEnumerator ExecuteShowMultiPlayerWin()
    {
        yield return new WaitForSeconds(4f);
        GameManagerIngame.Instance.LevelManager.ShowMultiplayerWin();
    }

    IEnumerator ExecuteShowMultiplayerDefeat()
    {
        yield return new WaitForSeconds(4f);
        GameManagerIngame.Instance.LevelManager.ShowMultiplayerDefeat();
    }

    [PunRPC]
    public void SyncMeshMaterial(string materialPath)
    {
        //MetaData.ConstVariable.Character.Find(PlayerPrefs.GetString("SelectedCharacter")).materialPath
        meshRenderer.material = Resources.Load<Material>(materialPath);
    }


    [PunRPC]
    public void FinishMultiplayerGame()
    {
        //if (PhotonNetwork.IsMasterClient)
        //    foreach (var item in GameObject.FindGameObjectsWithTag("Enemy"))
        //    {
        //            PhotonNetwork.Destroy(item.transform.parent.gameObject);
        //    }

        StartCoroutine(ExecuteGoToMain(4));
    }

    IEnumerator ExecuteGoToMain(float sec)
    {
        yield return new WaitForSeconds(sec);

        if (!GameManager.instance.IsAIMode && PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom.IsOpen)
            PhotonNetwork.LeaveRoom(true);

        SceneManager.LoadScene(MetaData.ConstVariable.Scenes.MainScene);
    }

    [PunRPC]
    public void ChangeMeshRotation(Vector3 rot)
    {
        PlayerMesh.transform.localRotation = Quaternion.Euler(rot);
    }

    #endregion
}
