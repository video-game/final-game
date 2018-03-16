using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingsMenu : UIMenu
{
    public void ReturnButton()
    {
        UIManager.Instance.CloseLast();
    }

}
