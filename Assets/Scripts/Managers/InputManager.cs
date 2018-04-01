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
                if (!GameManager.Instance.player[i].Alive)
                {
                    continue;
                }

                //always be updating the player i aim direction.
                GameManager.Instance.player[i].AimInDirection(playerControl[i].Direction(GameManager.Instance.player[i].transform.position));

                //if player i is moving, or has just stopped moving .. update player i movement.
                if (playerControl[i].Moving || playerControl[i].Stopped)
                {
                    GameManager.Instance.player[i].Move(playerControl[i].Horizontal, playerControl[i].Vertical);
                }

                //if player i pressed down his "Shoot" button once.
                if ((!playerControl[i].joystick && Input.GetKeyDown(playerControl[i].Shoot)) || (playerControl[i].joystick && playerControl[i].shootReady  && Input.GetAxisRaw("Shoot") == 1f ))
                {
                    GameManager.Instance.player[i].Shoot();

                    if (playerControl[i].joystick)
                    {
                        playerControl[i].shootReady = false;
                    }
                }

                //if player is holding his "Shoot" button
                //if player i pressed down his "Shoot" button once.
                if ((!playerControl[i].joystick && Input.GetKey(playerControl[i].Shoot)) || (playerControl[i].joystick && Input.GetAxisRaw("Shoot") == 1f))
                {
                    GameManager.Instance.player[i].ShootContinuous();
                }

                //if player i is pressing down his "Dash" button.
                if ((!playerControl[i].joystick && playerControl[i].Moving && Input.GetKeyDown(playerControl[i].Dash)) || (playerControl[i].joystick && playerControl[i].dashReady && Input.GetAxisRaw("Dash") == 1f))
                {
                    GameManager.Instance.player[i].Dash();

                    if (playerControl[i].joystick)
                    {
                        playerControl[i].dashReady = false;
                    }
                }

                //if player i is pressing the action button
                if(Input.GetKeyDown(playerControl[i].Action))
                {
                    GameManager.Instance.player[i].Interact();
                }

                if (playerControl[i].joystick && !playerControl[i].shootReady && Input.GetAxisRaw("Shoot") < 1f)
                {
                    playerControl[i].shootReady = true;
                }
                if (playerControl[i].joystick && !playerControl[i].dashReady && Input.GetAxisRaw("Dash") < 1f)
                {
                    playerControl[i].dashReady = true;
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

    public bool joystick;
    public bool shootReady = true;
    public bool dashReady = true;

    //player inputs to listen to. //todo support gamepads
    public KeyCode MoveUp, MoveDown, MoveLeft, MoveRight, Shoot, Dash, Ability1, Ability2, Ability3, Action;

    //check if player is moving.
    public bool Moving {
        get
        {
            return (!joystick && (Input.GetKey(MoveUp) || Input.GetKey(MoveDown) ||
                   Input.GetKey(MoveLeft) || Input.GetKey(MoveRight))) || (joystick && (Mathf.Abs(Input.GetAxis("Horizontal")) > .2f || Mathf.Abs(Input.GetAxis("Vertical")) > .2f));
        }
    }

    //check if player just stopped.
    public bool Stopped
    {
        get
        {
            return (!joystick && (Input.GetKeyUp(MoveUp) || Input.GetKeyUp(MoveDown) ||
                   Input.GetKeyUp(MoveLeft) || Input.GetKeyUp(MoveRight))) || (joystick && (Mathf.Abs(Input.GetAxis("Horizontal")) < .2f || Mathf.Abs(Input.GetAxis("Vertical")) < .2f)); ;
        }
    }

    //get player horizontal movement.
    public float Horizontal
    {
        get
        {
            float val = 0;
            if (!joystick && Input.GetKey(MoveLeft))
            {
                val = -moveValue;
            }
            else if (!joystick && Input.GetKey(MoveRight))
            {
                val = moveValue;
            }
            else if(joystick && Mathf.Abs(Input.GetAxis("Horizontal")) > .2f)
            {
                val = Input.GetAxis("Horizontal");
                val = val < 0 ? -moveValue : moveValue;
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
            if (!joystick && Input.GetKey(MoveDown))
            {
                val = -moveValue;
            }
            else if (!joystick && Input.GetKey(MoveUp))
            {
                val = moveValue;
            }
            else if (joystick && Mathf.Abs(Input.GetAxis("Vertical")) > .2f)
            {
                val = Input.GetAxis("Vertical");
                val = val < 0 ? -moveValue : moveValue;
            }
            return val;
        }
    }

    float hor2 = .05f;
    float ver2 = .05f;

    //get player aim target.
    private Vector3 Aim{
        get
        {
            //todo support gamepads.
            if (!joystick)
            {
                return Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                if(Mathf.Abs(Input.GetAxis("Horizontal2")) > .05f)
                {
                    hor2 = Input.GetAxis("Horizontal2");
                }
                if (Mathf.Abs(Input.GetAxis("Vertical2")) > .05f)
                {
                    ver2 = Input.GetAxis("Vertical2");
                }

                Vector3 pos = GameManager.Instance.player[1].transform.position;
                pos = new Vector3(pos.x + hor2, 0, pos.z + ver2);
                return pos;
            }
        }
    }

    //get player aim direction.
    public Vector3 Direction(Vector3 origin)
    {
        return (Aim - origin).normalized;
    }
}
