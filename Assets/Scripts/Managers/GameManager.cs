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

    public SharedItem sharedItems;

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

    public GameObject EnemyContainer;
    public GameObject NPCContainer;
    public GameObject PlayerContainer;

    [HideInInspector]
    public ResourceHud resourceHud;
    public override void CopyValues(GameManager copy)
    {
        playerCount = copy.playerCount;
        playerPrefab = copy.playerPrefab;
        playerModelPrefab = copy.playerModelPrefab;
        paused = copy.paused;
    }

    public void SetPlayerCount(int count)
    {
        playerCount = count;
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
        sharedItems = new SharedItem(0, 3);
        SpawnPlayers();
        UIManager.Instance.InstantiateResourceHud(sharedItems.Money, sharedItems.Revives);
    }


    private void SpawnPlayers()
    {
        player = new List<Player>();

        if (LevelManager.Instance.StartLocation.Count > 0)
        {
            for (int i = 0; i < playerCount; i++)
            {
                player.Add(Instantiate(playerPrefab, LevelManager.Instance.StartLocation[0]).GetComponent<Player>());
                player[i].transform.parent = PlayerContainer.transform;
                player[i].Init(playerModelPrefab[i]);
                player[i].OnPlayerDeath += CheckGameStatus;
            }
        }

        UIManager.Instance.InstantiatePlayerHud(player);

        if (cursorTexture)
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }

    private void CheckGameStatus()
    {
        int dead = 0;
        for (int i = 0; i < player.Count; i++)
        {
            if (!player[i].Alive)
            {
                dead++;
            }
        }

        if(dead == player.Count)
        {
            EndGame();
        }
    }
}


public class SharedItem
{
    public delegate void ValueChangeDelegate(int value);
    public ValueChangeDelegate OnMoneyChange;
    public ValueChangeDelegate OnReviveChange;

    int revives;
    public int Revives { get { return revives; } }

    int money;
    public int Money { get { return money; } }


    public SharedItem(int m, int r)
    {
        money = m;
        revives = r;
    }

    public bool ChangeMoney(int amount)
    {
        if (money + amount >= 0 || amount > 0)
        {
            money += amount;
            money = Mathf.Clamp(money, 0, int.MaxValue);

            if(OnMoneyChange != null)
            {
                OnMoneyChange(money);
            }
            return true;
        }
        return false;
    }

    public bool ChangeRevives(int amount)
    {
        if (revives + amount >= 0 || amount > 0)
        {
            if(amount > 0)
            {
                AudioManager.Instance.PlayAudioClip("RevivePickup");
            }
            else
            {
                AudioManager.Instance.PlayAudioClip("PlayerRevive");
            }
            revives += amount;
            revives = Mathf.Clamp(revives, 0, 20);

            if(OnReviveChange != null)
            {
                OnReviveChange(revives);
            }
            return true;
        }
        return false;
    }

}
