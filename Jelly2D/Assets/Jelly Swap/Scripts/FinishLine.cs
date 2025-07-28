using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    bool playerFinished;

    public GameManager gm;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !playerFinished)
        {
            playerFinished = true;
            gm.LevelComplete();
        }
        else if(collision.gameObject.CompareTag("Enemy") && !playerFinished)
        {
            print("enenmy finished first");
            playerFinished = true;
            gm.LevelFailed();
        }
    }
}
