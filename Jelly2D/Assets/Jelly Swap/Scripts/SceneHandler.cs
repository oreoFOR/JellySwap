using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.buildIndex);
    }
}
