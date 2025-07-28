using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    private void Start()
    {
        int num = PlayerPrefs.GetInt("Level Num");
        if(num > 2)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
