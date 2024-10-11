using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;

    public static Player Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                _instance = obj.AddComponent<Player>();
                obj.name = "Player";
            }
            return _instance;
        }
    }

    private int currentScore;
    private int turnsLeft;

    public static Action<int> ScoreChanged;
    public static Action<int> TurnsChanged;
    public static Action PassedLevel;
    public static Action GameOver;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        turnsLeft = BubbleLevel.Instance.maxTurns;

        yield return new WaitUntil(() => TurnsChanged != null);
        TurnsChanged.Invoke(turnsLeft);
        ShotHandler.ShotHandled += SubstractTurn;
        Bubble.BubbleDestroyed += AddScore;
    }

    private void AddScore(int score)
    {
        currentScore += score;
        ScoreChanged.Invoke(currentScore);
        if (currentScore >= BubbleLevel.Instance.scoreToWin)
            PassedLevel.Invoke();
    }

    private void SubstractTurn()
    {
        turnsLeft -= 1;
        TurnsChanged.Invoke(turnsLeft);
        if (turnsLeft == 0)
            GameOver.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
