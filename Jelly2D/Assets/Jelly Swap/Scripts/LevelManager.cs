using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    int levelNum;
    public void IncrementLevelNum()
    {
        levelNum = PlayerPrefs.GetInt("Level Num");
        levelNum += 1;
        PlayerPrefs.SetInt("Level Num", levelNum);
    }
}
