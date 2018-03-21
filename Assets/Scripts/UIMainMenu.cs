using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : UIMenu
{
    public void NewGameButton()
    {
        //todo: clean game state
        SceneLoader.Instance.Load("Overworld scene");
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenMenu("Settings");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

}
