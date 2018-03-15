using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuitMenu : UIMenu
{

    public override void Init()
    {
        base.Init();
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
