using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelNumTxt;
    public void UpdateUI()
    {
        int levelNum = PlayerPrefs.GetInt("Level Num");
        levelNumTxt.text = "Level " + (levelNum + 1).ToString();
    }
}
