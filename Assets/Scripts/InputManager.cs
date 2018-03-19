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
            for (int i = 0; i < GameManager.Instance.player.Count; i++)
            {
                //always be updating the player i aim direction.
                GameManager.Instance.player[i].AimInDirection(playerControl[i].Direction(GameManager.Instance.player[i].transform.position));

                //if player i is moving, or has just stopped moving .. update player i movement.
                if (playerControl[i].Moving || playerControl[i].Stopped)
                {
                    GameManager.Instance.player[i].Move(playerControl[i].Horizontal, playerControl[i].Vertical);
                }

                //if player i is pressing down his "Shoot" button.
                if (Input.GetKeyDown(playerControl[i].Shoot))
                {
                    GameManager.Instance.player[i].Shoot();
                }
                //if player i is pressing down his "Dash" button.
                if (playerControl[i].Moving && Input.GetKeyDown(playerControl[i].Dash))
                {
                    GameManager.Instance.player[i].Dash();
                }
            }
        }
    }
}

//a struct to organize each players custom inputs. 
[System.Serializable]
public class PlayerControl
{
    const float moveValue = 0.5f;

    //player inputs to listen to. //todo support gamepads
    public KeyCode MoveUp, MoveDown, MoveLeft, MoveRight, Shoot, Dash, Ability1, Ability2, Ability3;

    //check if player is moving.
    public bool Moving {
        get
        {
            return (Input.GetKey(MoveUp) || Input.GetKey(MoveDown) ||
                   Input.GetKey(MoveLeft) || Input.GetKey(MoveRight));
        }
    }

    //check if player just stopped.
    public bool Stopped
    {
        get
        {
            return (Input.GetKeyUp(MoveUp) || Input.GetKeyUp(MoveDown) ||
                   Input.GetKeyUp(MoveLeft) || Input.GetKeyUp(MoveRight));
        }
    }

    //get player horizontal movement.
    public float Horizontal
    {
        get
        {
            float val = 0;
            if (Input.GetKey(MoveLeft))
            {
                val = -moveValue;
            }
            else if (Input.GetKey(MoveRight))
            {
                val = moveValue;
            }
            return val;
        }
    }

    //get player vertical movement.
    public float Vertical
    {
        get
        {
            float val = 0;
            if (Input.GetKey(MoveDown))
            {
                val = -moveValue;
            }
            else if (Input.GetKey(MoveUp))
            {
                val = moveValue;
            }
            return val;
        }
    }

    //get player aim target.
    private Vector3 Aim{
        get
        {
            //todo support gamepads.
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    //get player aim direction.
    public Vector3 Direction(Vector3 origin)
    {
        return (Aim - origin).normalized;
    }
}
