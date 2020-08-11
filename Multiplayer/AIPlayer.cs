using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public enum PlayerState { Idle = 0, Running = 1, Jumping = 2, Death = 3, Winner = 4, TutorialCheckPoint1, TutorialCheckPoint2, TutorialCheckPoint3, }

    #region SerializeFields

    [SerializeField] private float maxJumpTime;
    [SerializeField] private GameObject runParticle;
    [SerializeField] private ObjectCreator objectCreator;
    #endregion

    #region Local Variables

    #endregion

    #region Public Variables

    public bool canMove = true;
    public bool _isGrounded = false;
    public bool canJump = true;
    public float jumpTime;
    public PlayerState playerState;
    public GameObject PlayerMesh;
    public Transform jumpingTransform, idleTransform, instantiateTransform, winnerTransform;
    public SkinnedMeshRenderer meshRenderer;
    #endregion

    #region Components

    private Animator m_animator;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;

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

    private void Start()
    {
        GetComponents();
        InitializeParameters();
        StartCoroutine(AIDesicion());
    }


    IEnumerator AIDesicion()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);

            if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING)
            {
                float mindistance = objectCreator.FindNearestEnemyDistance(GameManagerIngame.Instance.LevelManager.transform.eulerAngles.z);

                if (mindistance > 150 && !isJumping)
                {
                    if (Random.value > 0.9)
                    {
                        JumpRandomlyHigh();
                    }
                }
            }

        }

    }

    public bool isJumping;

    void Update()
    {

        if (isJumping)
            Jump();
        else
            EndJump();

        if (playerState == PlayerState.Death && !IsGrounded)
            EndJump();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "OutsideMask")
        {
            jumpTime = 0;
            _isGrounded = true;
            if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING)

                SetPlayerState(PlayerState.Running);
        }
        if (collision.tag == "Enemy")
        {
            isJumping = false;
            SetPlayerState(PlayerState.Death);
            GameManagerIngame.Instance.LocalPlayer.MultiplayerWinner();
        }
    }
    #endregion

    #region Private Methods
    private void GetComponents()
    {
        m_animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void InitializeParameters()
    {
        SetPlayerState(PlayerState.Idle);
        GameManagerIngame.Instance.GameState = GameManagerIngame.GAMESTATE.WAITING;
        PlayerMesh.SetActive(true);
        GameManagerIngame.Instance.AIPlayer = this;
        meshRenderer.material = Resources.Load<Material>(MetaData.ConstVariable.Character.characters[Random.Range(1,27)].materialPath);

    }

    private void Jump()
    {
        _isGrounded = false;
        if (playerState == PlayerState.Jumping && jumpTime < maxJumpTime)
        {
            jumpTime += Time.deltaTime;

            if (transform.localPosition.y <= 2.4f)
            {
                if (transform.localPosition.y < 2f && GameManagerIngame.Instance.LevelManager.Level == 1 && PlayerPrefs.GetString("OpenedWorld") == MetaData.ConstVariable.Planet.planets[0].key && GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                {
                    transform.position = Vector2.MoveTowards(transform.position, jumpingTransform.position, Time.deltaTime * 5f);
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, jumpingTransform.position, Time.deltaTime * 3f);

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

                    Animator.SetBool("isRunning", false);
                    Animator.SetBool("isJumping", false);
                    Animator.SetBool("isDead", false);
                    Animator.SetBool("isWon", false);
                    runParticle.SetActive(false);
                    break;
                case PlayerState.Running:
                    if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
                    {
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
                    Animator.SetBool("isDead", true);
                    PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    canMove = false;
                    break;
                case PlayerState.Winner:
                    PlayerMesh.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
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
        StartCoroutine(ExecutePlayerWinAnimation());
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


    #endregion

    #region Public AIMethods

    public void JumpRandomlyHigh()
    {
        if (!_isGrounded) return;
        StartCoroutine(ExecuteJump(Random.Range(0.2f, 0.36f)));
    }

    public void JumpForMovingObject()
    {
        if (!_isGrounded) return;
        StartCoroutine(ExecuteJump(0.2f));
    }

    IEnumerator ExecuteJump(float jumpTime)
    {

        SetPlayerState(PlayerState.Jumping);
        isJumping = true;

        yield return new WaitForSeconds(Random.Range(0.25f, 0.4f));


        isJumping = false;

    }

    #endregion


}
