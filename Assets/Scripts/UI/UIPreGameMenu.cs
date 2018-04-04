using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPreGameMenu : UIMenu
{
    public void ContinueButton()
    {
        SceneLoader.Instance.Load("Overworld scene");
    }

    public void ReturnButton()
    {
        UIManager.Instance.CloseLast();
    }

}
