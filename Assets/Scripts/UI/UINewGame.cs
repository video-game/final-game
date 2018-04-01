using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINewGame : UIMenu
{
    private void OnEnable()
    {
        GameManager.Instance.SetPlayerCount(1);    
    }

    public void StartGameButton()
    {
        //todo: clean game state
        SceneLoader.Instance.Load("Overworld scene");
    }

    public void TogglePlayerCount(int val)
    {
        GameManager.Instance.SetPlayerCount(val);
    }

    public void ReturnButton()
    {
        UIManager.Instance.CloseLast();
    }

}
