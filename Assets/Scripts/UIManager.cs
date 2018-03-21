using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIManager : SingletonMB<UIManager>
{
    //a reference list of menus and open menus
    public List<Menu> menu;
    public Stack<UIMenu> openMenu = new Stack<UIMenu>();

    //UIContainer will be the parent of all the menus
    public GameObject UIContainer;

    //HudContainer will be the parent of all huds.
    public GameObject HudContainer;

    //a references list of player huds.
    public List<PHud> playerHud;

    //A property that returns if the current menu can be paused over
    public bool AllowPause { get { return openMenu.Count != 0 ? openMenu.Peek().allowPause : true; } }

    private void Start()
    {
        InstantiateMenus();
    }

    //instantiate all the prefabs and initialize if need be.
    private void InstantiateMenus()
    {
        for (int i = 0; i < menu.Count; i++)
        {
            menu[i].instance = Instantiate(menu[i].prefab, UIContainer.transform).GetComponent<UIMenu>();
            menu[i].instance.Init(menu[i].name);
        }
    }

    //instantiate all the prefabs and initialize if need be.
    public void InstantiatePlayerHud(List<Player> player)
    {
        for (int i = 0; i < player.Count; i++)
        {
            playerHud[i].instance = Instantiate(playerHud[i].prefab, HudContainer.transform).GetComponent<PlayerHud>();
            playerHud[i].instance.Init(player[i]);
        }
    }

    //Hide but don't close the current open menu.
    public void HideLast()
    {
        if(openMenu.Count != 0)
        {
            openMenu.Peek().Hide(true);
        }
    }

    //Unhide the last open menu.
    public void ShowLast()
    {
        if (openMenu.Count != 0)
        {
            openMenu.Peek().Hide(false);
        }
    }

    //Hide current menu, and then Open and Unhide the new menu.
    public void OpenMenu(UIMenu menu)
    {
        HideLast();
        openMenu.Push(menu);
        ShowLast();
    }

    //open menu by it's name
    public void OpenMenu(string name)
    {
        OpenMenu(menu.First(menu => menu.name == name).instance);
    }

    //Close current menu, and unhide previous open menu (if any)
    public void CloseLast()
    {
        if (openMenu.Count != 0 && openMenu.Peek().CanBeClosed)
        {
            openMenu.Pop().Hide(true);
            ShowLast();
        }
    }


    //Close all open menus. And Clear List.
    public void Clear()
    {
        for (int i = 0; i < openMenu.Count; i++)
        {
            CloseLast();
        }
        openMenu.Clear();
    }

}

//mini class that holds menu information.
//struct wasn't compatible with List element manipulation.
[System.Serializable]
public class Menu{
    public string name;
    public GameObject prefab;
    [System.NonSerialized]
    public UIMenu instance;
}

[System.Serializable]
public class PHud
{
    public GameObject prefab;
    [System.NonSerialized]
    public PlayerHud instance;
}

