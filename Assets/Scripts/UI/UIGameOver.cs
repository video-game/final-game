using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameOver : UIMenu
{
    public override void Init(string name)
    {
        //run the base class Init method
        base.Init(name);

        //add listener to the OnGameOver event
        //This menu is not opened by a button press, but instead opens when the GameManager is set to EndGame.
        GameManager.Instance.OnGameOver += Toggle;
    }

    public void MainMenuButton()
    {
        SceneLoader.Instance.Load("MainMenu");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
