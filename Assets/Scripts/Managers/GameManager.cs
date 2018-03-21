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

    public GameObject playerPrefab;
    public List<GameObject> playerModelPrefab;

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

    [SerializeField]
    private Texture2D cursorTexture;

    //Very crappy solution, works for now.
    public GameObject Tombstone;

    public override void CopyValues(GameManager copy)
    {
        playerCount = copy.playerCount;
        playerPrefab = copy.playerPrefab;
        playerModelPrefab = copy.playerModelPrefab;
        paused = copy.paused;
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
        if (paused)
        {
            PauseGame();
        }
        SpawnPlayers();
    }


    private void SpawnPlayers()
    {
        player = new List<Player>();

        if (LevelManager.Instance.StartLocation.Count > 0)
        {
            for (int i = 0; i < playerCount; i++)
            {
                player.Add(Instantiate(playerPrefab, LevelManager.Instance.StartLocation[0]).GetComponent<Player>());
                player[i].Init(playerModelPrefab[i]);
            }
        }

        UIManager.Instance.InstantiatePlayerHud(player);

        if (cursorTexture)
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }
}
