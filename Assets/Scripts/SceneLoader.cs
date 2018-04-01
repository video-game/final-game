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
    public LoadingScreen loadingUIPrefab;
    private LoadingScreen loadingUIinstance;

    IEnumerator LoadScene(string scene)
    {
        if (scene == "" || !Application.CanStreamedLevelBeLoaded(scene))
        {
            Debug.LogWarning(scene == "" ? "Scene must have a name" : "Scene does not exist in build settings");
            yield break;
        }

        //instantiate the loading screen, and pause momentarily while the screen fades in.
        loadingUIinstance = Instantiate(loadingUIPrefab) as LoadingScreen;
        while (!loadingUIinstance.ready)
        {
            yield return null;
        }

        async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

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

    //load scene by name
    public void Load(string scene)
    {
        //There will temporarily be 2 AudioListeners as we are async loading the new scene, and then unloading the old.
        //Because of this we disable the old AudioListener.
        Camera.main.GetComponent<AudioListener>().enabled = false;
        StartCoroutine(LoadScene(scene));
    }
}