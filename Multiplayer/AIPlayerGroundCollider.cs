using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerGroundCollider : MonoBehaviour
{
    [SerializeField] AIPlayer AI;

    public bool isTriggered;

    Enemy triggeredEnemy;

    private void Start()
    {
        isTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered)
        {
            if (collision.tag == "Enemy")
            {
                if (collision != null)
                    if (collision.transform.parent.GetComponent<MultiplayerEnemy>().enemyType == MultiplayerEnemy.EnemyType.Moving)
                    {
                        AI.JumpForMovingObject();
                    }
                    else
                    {
                        AI.JumpRandomlyHigh();
                    }

                isTriggered = true;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            isTriggered = false;
        }
    }
}
