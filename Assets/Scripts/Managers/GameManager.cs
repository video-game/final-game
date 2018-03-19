using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMB<GameManager>
{
    public delegate void PauseDelegate(bool isPaused);
    public PauseDelegate OnPause;
    public delegate void GameOverDelegate(bool isGameOver);
    public GameOverDelegate OnGameOver;

    //how many players
    public int playerCount;

    //Reference to player for other scripts.
    [HideInInspector]
    public List<Player> player;

    //a private bool to see if the game is paused
    //it is then escaped as a property (but only for get)
    private bool paused;
    public bool Paused { get { return paused; } }

    //a private bool to see if the game is over
    //it is then escaped as a property (but only for get)
    private bool gameOver;
    public bool GameOver { get { return gameOver; } }


    public override void CopyValues(GameManager copy)
    {
        playerCount = copy.playerCount;
    }

    //a function that pauses the game, stops gameTime shows the pauseScreen;
    public void PauseGame()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        if(OnPause != null)
        {
            OnPause(paused);
        }
    }

    // a function that signals the end of the game, set gameOver, stop time and show the GameOver UI
    public void EndGame()
    {
        gameOver = true;
        Time.timeScale = 0;
        if (OnGameOver != null)
        {
            OnGameOver(gameOver);
        }
    }


    private void Start()
    {
        Player[] p = GameObject.FindObjectsOfType<Player>();
        for (int i = 0; i < p.Length; i++)
        {
            player.Add(p[i]);
        }
    }
}
