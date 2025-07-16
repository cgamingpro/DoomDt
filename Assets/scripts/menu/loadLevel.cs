using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class loadLevel : MonoBehaviour
{
    public int sceneindix;

    public void loadScene()
    {
        SceneManager.LoadScene(sceneindix);
    }
}
