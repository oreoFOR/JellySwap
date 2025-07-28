using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] GameObject [] obstacles;
    [SerializeField]Transform endPoint;
    [SerializeField] GameObject FinishLine;
    [SerializeField] GameManager manager;
    [SerializeField] SwitchShape switcher;
    [SerializeField] SwitchShape enemySwitcher;
    [SerializeField] Vector2 maxminLevelLength;
    [SerializeField] int earlyLvlLength;
    private void Awake()
    {
        int courseLength = PlayerPrefs.GetInt("Level Num") < 3 ? earlyLvlLength : Random.Range((int)maxminLevelLength.x, (int)maxminLevelLength.y);
        for (int i = 0; i < courseLength; i++)
        {
            int randOb = Random.Range(0, obstacles.Length);
            Obstacle currentObstacle = Instantiate(obstacles[randOb], endPoint.position, Quaternion.identity).GetComponent<Obstacle>();
            endPoint = currentObstacle.endPoint;
        }
        FinishLine finish = Instantiate(FinishLine, endPoint.position, Quaternion.identity).GetComponent<FinishLine>();
        finish.gm = manager;
        switcher.finish = finish.transform.position.y;
        enemySwitcher.finish = finish.transform.position.y;
    }
}
