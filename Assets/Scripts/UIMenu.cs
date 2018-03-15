using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour {

    //allowPause determines if you can open the pause menu over this menu.
    public bool allowPause = false;
    public bool hiddenAtStart = true;
    public bool CanBeClosed = true;

    //init virtual functions used to set some Initial state
    public virtual void Init() {
        Hide(hiddenAtStart);

        if (!hiddenAtStart)
        {
            Open();
        }
    }

    public virtual void Init(string name) {
        this.name = name;
        Init();
    }

    public virtual void Hide(bool hide)
    {
        this.gameObject.SetActive(!hide);
    }

    //Passes this Menu to the UIManager to open.
    public virtual void Open()
    {
        UIManager.Instance.OpenMenu(this);
    }

    //Close the last open menu in the UIManager, which should be this one.
    public virtual void Close()
    {
        UIManager.Instance.CloseLast();
    }

    //helper function to allow for toggling open/close
    public virtual void Toggle(bool open)
    {
        if (open)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

}
