using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{


    [SerializeField] private float speed = 1f;
    void FixedUpdate()
    {
        if (GameManagerIngame.Instance.LevelManager != null)
        {
            if (GameManagerIngame.Instance.GameState == GameManagerIngame.GAMESTATE.PLAYING /*||GameManager.Instance.GetGameState() == GameManager.GameState.GAME_OVER*/)
            {
                transform.Rotate(Vector3.forward * speed);
            }
        }
    }

    /// <summary>
    /// This method sets enemy rotate speed.
    /// </summary>
    /// <param name="speed">rotate speed value</param>
    public virtual void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
