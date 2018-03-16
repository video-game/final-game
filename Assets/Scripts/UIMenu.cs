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
        //Hide if set to hiddenAtStart
        Hide(hiddenAtStart);

        //if not set to hiddenAtStart, start open.
        if (!hiddenAtStart)
        {
            Open();
        }
    }

    //override of Init that takes in a string parameter.
    //it sets the name as the string then calls the original Init afterwards.
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
