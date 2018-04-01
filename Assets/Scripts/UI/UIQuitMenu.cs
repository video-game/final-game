using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuitMenu : UIMenu
{
    public void CancelButton()
    {
        UIManager.Instance.CloseLast();
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
