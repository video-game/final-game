using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINewGame : UIMenu
{
    private void Awake()
    {
        GameManager.Instance.SetPlayerCount(1);    
    }

    public void StartGameButton()
    {
        UIManager.Instance.OpenMenu("PreGameMenu");
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
