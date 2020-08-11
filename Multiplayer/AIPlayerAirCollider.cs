using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerAirCollider : MonoBehaviour
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
                isTriggered = true;

                AI.JumpRandomlyHigh();
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
