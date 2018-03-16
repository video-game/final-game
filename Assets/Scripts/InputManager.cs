using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a singleton class that handles all user input.
public class InputManager : SingletonMB<InputManager> {


    //the keyCode for the pause
    public KeyCode pause;
    //a list of all player controls, uses same index as player list. (GameManager.Instance.Player)
    public List<PlayerControl> playerControl;

    private void Update()
    {
        //if game not over and pause key is pressed.
        if (Input.GetKeyDown(pause) && !GameManager.Instance.GameOver)
        {
            //if pause is allowed, pause .. else close current menu. (if possible)
            if (UIManager.Instance.AllowPause)
            {
                GameManager.Instance.PauseGame();
            }
            else
            {
                UIManager.Instance.CloseLast();
            }
        }

        //if game is not over and is not paused.
        if (!GameManager.Instance.Paused && !GameManager.Instance.GameOver && UIManager.Instance.openMenu.Count == 0)// && GameManager.Instance.Player.Count > 0)
        {
            for (int i = 0; i < playerControl.Count; i++)
            {
                //if player i is pressing down his "Shoot" button.
                if (Input.GetKeyDown(playerControl[i].Shoot))
                {
                    //Debug.Log("Player " + i + " Shoot");
                    //GameManager.Instance.Player[i].Left();
                }
                //if player i is pressing down his "Dash" button.
                if (Input.GetKeyDown(playerControl[i].Dash))
                {
                    //Debug.Log("Player " + i + " Dash");
                    //GameManager.Instance.Player[i].Action();
                }
            }
        }
    }
}

//a struct to organize each players custom inputs. 
[System.Serializable]
public struct PlayerControl
{
    //these are not final ofcourse.
    public KeyCode Shoot, Dash, Ability1, Ability2, Ability3;
}
