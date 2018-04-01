using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LoadingScreen : MonoBehaviour
{
    //references to all of loadin
    public UnityEngine.UI.RawImage loadScreen;
    public GameObject loadingBarPanel;
    public UnityEngine.UI.Slider LoadingBar;
    public TextMeshProUGUI LoadingText;

    //ready variable is used to delay operation while fading. (optional)
    public bool ready;
    public float fadeTime = .8f;

    //Set % of the loading bar slider and text.
    //progress is from 0 to 1.
    public void SetPercentage(float progress)
    {
        if (loadingBarPanel != null)
        { 
            if (!loadingBarPanel.activeInHierarchy)
            {
                loadingBarPanel.SetActive(true);
            }
        }

        LoadingBar.value = progress;
        LoadingText.text = Mathf.Round(progress * 100f) + "%";

        //if progress is at 1 (aka 100%)
        //start fading out.
        if(progress == 1f)
        {
            Destroy(true);
        }
    }

    IEnumerator Fade(bool fadeIn, bool destroy = false)
    {
        ready = false;

        //if the loadingBar is still active, delay for 100ms and then deactivate.
        //without the delay, it could load to dissapear before actually displaying the 100% progress.
        if (loadingBarPanel.activeInHierarchy)
        {
            yield return new WaitForSecondsRealtime(.1f);
            loadingBarPanel.SetActive(false);
        }

        float alphaMin = 0;
        float alphaMax = 255;
        float alpha = fadeIn ? alphaMin : alphaMax;
        float time = 0;

        //normalize the current alpha.
        loadScreen.color = new Color(loadScreen.color.r, loadScreen.color.g, loadScreen.color.b, alpha);

        //fade In or Out in fadeTime seconds.
        while (fadeIn ? alpha < alphaMax : alpha > alphaMin)
        {
            alpha = Mathf.Lerp(fadeIn ? alphaMin : alphaMax, fadeIn ? alphaMax : alphaMin, time / fadeTime);
            loadScreen.color = new Color(loadScreen.color.r, loadScreen.color.g, loadScreen.color.b, alpha / alphaMax);

            //unscaled time so that it works while paused.
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        ready = true;

        if (destroy)
        {
            Destroy();
        }
    }

    private void Start()
    {
        //start fading in when initialized.
        StartCoroutine(Fade(true));
        //dont destroy on load.
        //The scene that initializes this might be unloaded while this is still fading.
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
