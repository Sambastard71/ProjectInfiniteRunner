using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    bool startGame = false;

    AsyncOperation asyncload;
    bool sceneIsLoaded  = false;
    public bool SceneIsLoaded
    {
        get
        {
            return sceneIsLoaded;
        }
    }
    
    public void StartLoadScene()
    {
        if (!sceneIsLoaded)
        {
            StartCoroutine(LoadScene()); 
        }
    }

    public void LoadGameScene()
    {
        startGame = true;
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1);

        asyncload = SceneManager.LoadSceneAsync("Game");

        asyncload.allowSceneActivation = false;

        while (!asyncload.isDone && !startGame)
        {
            Debug.Log(asyncload.progress);
            yield return null;
        }

        sceneIsLoaded = true;
        asyncload.allowSceneActivation = true;

        yield return null;
        //yield return new WaitForSeconds(Time.deltaTime);

        //yield return null;
    }

}
