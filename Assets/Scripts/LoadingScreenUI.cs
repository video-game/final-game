using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UILoadingScreen : MonoBehaviour
{
    //references to all of loadin
    public UnityEngine.UI.RawImage loadScreen;
    public GameObject loadingBarPanel;
    public UnityEngine.UI.Slider LoadingBar;
    public TextMeshProUGUI LoadingText;

    public bool ready;
    public float fadeTime = .8f;

    public void SetPercentage(float progress)
    {
        if (!loadingBarPanel.activeInHierarchy)
        {
            loadingBarPanel.SetActive(true);
        }

        LoadingBar.value = progress;
        LoadingText.text = progress * 100f + "%";

        if(progress == 1f)
        {
            Destroy(true);
        }
    }

    IEnumerator Fade(bool fadeIn, bool destroy = false)
    {
        ready = false;

        if (loadingBarPanel.activeInHierarchy)
        {
            yield return new WaitForSeconds(.1f);
            loadingBarPanel.SetActive(false);
        }

        float alphaMin = 0;
        float alphaMax = 255;
        float alpha = fadeIn ? alphaMin : alphaMax;
        float time = 0;

        loadScreen.color = new Color(loadScreen.color.r, loadScreen.color.g, loadScreen.color.b, alpha);

        while(fadeIn ? alpha < alphaMax : alpha > alphaMin)
        {
            alpha = Mathf.Lerp(fadeIn ? alphaMin : alphaMax, fadeIn ? alphaMax : alphaMin, time / fadeTime);
            loadScreen.color = new Color(loadScreen.color.r, loadScreen.color.g, loadScreen.color.b, alpha / alphaMax);
            time += Time.deltaTime;
            yield return null;
        }

        if (destroy)
        {
            Destroy();
        }

        ready = true;
    }

    private void Awake()
    {
        StartCoroutine(Fade(true));
        DontDestroyOnLoad(this.gameObject);
    }

    public void Destroy(bool fade = false)
    {
        if (fade)
        {
            StartCoroutine(Fade(false, true));
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

}
