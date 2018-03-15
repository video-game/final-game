using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneLoader : SingletonMB<SceneLoader>
{
    //a variable to hold a reference to the AsyncOperation
    private AsyncOperation async;


    //variables to hold references for the loadingscreen prefab, and it's instance.
    public UILoadingScreen loadingUIPrefab;
    private UILoadingScreen loadingUIinstance;

    public override void CopyValues(SceneLoader copy)
    {
        if(copy.loadingUIPrefab != null)
        {
            loadingUIPrefab = copy.loadingUIPrefab;
        }

    }

    IEnumerator LoadScene(string scene)
    {
        Debug.Log("scene: " + scene);
        if (scene == "")
            yield break;

        //instantiate the loading screen, and pause momentarily while the screen fades in.
        loadingUIinstance = Instantiate(loadingUIPrefab) as UILoadingScreen;
        while (!loadingUIinstance.ready)
        {
            yield return null;
        }

        async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        if (async == null)
        {
            loadingUIinstance.Destroy();
            yield break;
        }

        //update progress bar as scene loads.
        while (!async.isDone)
        {
            loadingUIinstance.SetPercentage(async.progress);
            yield return null;
        }
        //scene has finished loading, Get rid of the loading screen. (Destroy(True) fades out before destroying)
        loadingUIinstance.SetPercentage(1f);

        //unload the old Scene.
        async = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        while (!async.isDone)
        {
            yield return null;
        }
    }

    public void Load(string scene)
    {
        Camera.main.GetComponent<AudioListener>().enabled = false;
        StartCoroutine(LoadScene(scene));
    }
}