using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCreator : MonoBehaviour
{
    #region SerializeFields
    float turnSpeed = 1;
    [SerializeField] private GameObject insideMask;
    [SerializeField] private GameObject outsideMask;
    [SerializeField] private Image background;
    [SerializeField] private Image continuePanelBackground;
    [SerializeField] private Sprite[] circleSprites;
    [SerializeField] private SkinnedMeshRenderer characterMesh;

    List<GameObject> createdEnemies = new List<GameObject>();

    #endregion

    #region Private Methods
    private void Awake()
    {
        StartCoroutine(CreateSelectedCharacter());
        GameManagerIngame.Instance.LevelManager.canTurn = true;

        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Multiplayer2 || GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Multiplayer3 || GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Multiplayer4)
            outsideMask.GetComponent<SpriteRenderer>().sprite = circleSprites[Random.Range(0, circleSprites.Length)];

    }
    private void FixedUpdate()
    {
        if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING && GameManagerIngame.Instance.LevelManager.canTurn &&
            GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
        {
            transform.Rotate(Vector3.forward * -turnSpeed);
        }

        if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING && GameManagerIngame.Instance.LevelManager.canTurn &&
            GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.Contest && GameManagerIngame.Instance.Contest.isStarted)
        {
            transform.Rotate(Vector3.forward * -turnSpeed);
        }
        else if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING && GameManagerIngame.Instance.LevelManager.canTurn &&
            GameManagerIngame.Instance.GameMode != GameManagerIngame.Mode.SinglePlayer && GameManagerIngame.Instance.ObjectCreator.isStarted)
        {
            transform.Rotate(Vector3.forward * -turnSpeed);
        }
    }

    IEnumerator CreateSelectedCharacter()
    {
        yield return new WaitUntil(() => GameManagerIngame.Instance.LocalPlayer != null);



        if (GameManagerIngame.Instance.GameMode == GameManagerIngame.Mode.SinglePlayer)
        {
            GameManagerIngame.Instance.LocalPlayer.meshRenderer.material = Resources.Load<Material>(MetaData.ConstVariable.Character.Find(PlayerPrefs.GetString("SelectedCharacter")).materialPath);

        }
        else
        {

        }



    }
    #endregion

    #region Public Methods

    /// <summary>
    /// This method changes the Character speed.
    /// </summary>
    /// <param name="turnSpeed"> Character turn speed</param>
    public void SetTurnSpeed(float turnSpeed)
    {
        this.turnSpeed = turnSpeed;
    }

    /// <summary>
    /// This method returns Character speed;
    /// </summary>
    public float GetTurnSpeed()
    {
        return turnSpeed;
    }

    /// <summary>
    /// This method starts to create coin 
    /// </summary>
    /// <param name="probability">Coin create rate</param>
    /// <returns></returns>
    public IEnumerator StartCoinCreateProbability(float probability)
    {
        while (true)
        {
            if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING)
            {
                yield return new WaitForSeconds(1f);
                float rnd = Random.Range(0.0f, 0.99f);
                if (rnd < probability)
                {
                    GameObject createdCoin;
                    int rndInt = Random.Range(0, 2);
                    if (rndInt == 1)
                    {
                        int rot = Random.Range(130, 230);
                        createdCoin = Instantiate(Resources.Load<GameObject>("Prefabs/Coin"), new Vector3(0, 0, 0), Quaternion.Euler(0, 0, rot));

                    }
                    else
                    {
                        int rot = Random.Range(-50, 50);
                        createdCoin = Instantiate(Resources.Load<GameObject>("Prefabs/Coin"), new Vector3(0, 0, 0), Quaternion.Euler(0, 0, rot));
                    }
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// This Method setups the inside and outside mask.
    /// </summary>
    /// <param name="levelInfo">Need json convert to LevelInfo class</param>
    public void SetupLevel(LevelInfo levelInfo)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(levelInfo.Sprite);
        // insideMask.GetComponent<SpriteRenderer>().sprite = sprites[0];
        outsideMask.GetComponent<SpriteRenderer>().sprite = circleSprites[Random.Range(0, circleSprites.Length)];
    }

    /// <summary>
    /// This method changes the Character speed.
    /// </summary>
    /// <param name="speed">Character turn speed</param>
    public void SetupCharacterSpeed(float speed)
    {
        turnSpeed = speed;
    }
    IEnumerator SetupEnemy(EnemyInfo enemy)
    {
        while (GameManagerIngame.Instance.GameState != GameManagerIngame.GAMESTATE.PLAYING)
        {
            yield return null;
        }
        float time = 0;

        while (time <= enemy.CreationTime)
        {
            while (GameManagerIngame.Instance.isPaused)
            {
                yield return null;
            }
            time += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);

        }
        time = 0;

        GameObject createdEnemy = Instantiate(Resources.Load<GameObject>("Prefabs/" + enemy.Enemy), new Vector3(0, 0, 0), new Quaternion(0, 0, enemy.RotationZ, 60));
        if (createdEnemy.GetComponent<Enemy>() != null)
            createdEnemy.GetComponent<Enemy>().SetSpeed(enemy.Speed); // Set Enemy Speed
        createdEnemies.Add(createdEnemy);
        while (time <= enemy.LifeTime)
        {
            while (GameManagerIngame.Instance.isPaused)
            {
                yield return null;
            }
            time += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        createdEnemies.Remove(createdEnemy);

        Destroy(createdEnemy);
    }

    /// <summary>
    /// This method creates the enemies.
    /// </summary>
    /// <param name="enemies">this method requires to Enemy list.</param>
    public void SetupEnemies(List<EnemyInfo> enemies)
    {
        foreach (var enemy in enemies)
        {
            StartCoroutine(SetupEnemy(enemy));
        }
    }

    /// <summary>
    /// This method stops all coroutines. (for disable next time enemy creation)
    /// </summary>
    public void StopCoroutines()
    {
        StopAllCoroutines();
    }


    public void ResetLevel()
    {
        StopCoroutines();
        foreach (var item in createdEnemies)
        {
            Destroy(item);
        }
    }
    /// <summary>
    /// This method changes level background.
    /// </summary>
    /// <param name="path">Sprite path from json.</param>
    public void SetupBackgroundImage(string path)
    {
        background.sprite = Resources.Load<Sprite>(path);
        continuePanelBackground.sprite = Resources.Load<Sprite>(path);
    }

    #endregion
}
