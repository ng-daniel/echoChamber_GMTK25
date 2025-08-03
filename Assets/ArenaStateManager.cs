using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaStateManager : MonoBehaviour
{

    public enum GameState
    {
        INTRO,
        GAME,
        WIN,
        DEATH
    }
    private static GameState currentState;

    EnemySpawnManager enemySpawnManager;



    void Start()
    {
        currentState = GameState.INTRO;
        enemySpawnManager = GameObject.FindFirstObjectByType<EnemySpawnManager>();
    }

    public GameState GetGameState()
    {
        return currentState;
    }

    public void OnIntroState()
    {
        print("entering INTRO state");
    }
    public void OnGameState()
    {
        print("entering GAME state");
    }
    public void OnWinState()
    {
        print("entering WIN state");
    }
    public void OnDeathState()
    {
        print("entering DEATH state");
    }
}
