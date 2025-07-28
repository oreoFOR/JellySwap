using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject confettis;
    [SerializeField] GameObject GOP;
    [SerializeField] GameObject GOPFail;
    [SerializeField] GameObject GameUI;
    [SerializeField] GameObject startUI;
    [SerializeField] GameObject progressUI;

    [SerializeField] LevelManager levelManager;
    [SerializeField] UIManager uiManager;

    [SerializeField] float timeScale = 1;
    private void Awake()
    {
        Time.timeScale = 0;
        uiManager.UpdateUI();
    }
    public void DropJelly()
    {
        Time.timeScale = timeScale;
        startUI.SetActive(false);
        GameUI.SetActive(true);
        progressUI.SetActive(true);
        //TinySauce.OnGameStarted();
    }
    public void LevelComplete()
    {
        GOP.SetActive(true);
        confettis.SetActive(true);
        GameUI.SetActive(false);
        levelManager.IncrementLevelNum();
        //TinySauce.OnGameFinished(true,1, PlayerPrefs.GetInt("Level Num").ToString());
    }
    public void LevelFailed()
    {
        GOPFail.SetActive(true);
        GameUI.SetActive(false);
        //TinySauce.OnGameFinished(false, 1, PlayerPrefs.GetInt("Level Num").ToString());
    }
}
