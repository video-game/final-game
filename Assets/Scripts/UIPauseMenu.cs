using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenu : UIMenu
{
    public override void Init(string name)
    {
        //run the base class Init method
        base.Init(name);

        //add listener to the OnPause event
        //This menu is not opened by a button press, but instead opens when the GameManager is set to Pause.
        GameManager.Instance.OnPause += Toggle;
    }

    public void ReturnButton()
    {
        GameManager.Instance.PauseGame();
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenMenu("Settings");
    }

    public void QuitMenuButton()
    {
        UIManager.Instance.OpenMenu("Quit");
    }

    private void BeforeDestroy()
    {
        //remove listener (just good practice)
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnPause -= Toggle;
        }
    }

}
