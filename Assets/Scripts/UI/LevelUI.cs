using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    private Button exitToMenuBtn;
    private TMP_Text score;
    private TMP_Text turnsLeft;

    private GameObject gameEndPanel;
    private TMP_Text gameEndPanel_resultText;
    private TMP_Text gameEndPanel_totalScore;
    private Button gameEndPanel_exitToMenuBtn;

    // Start is called before the first frame update
    void Start()
    {
        gameEndPanel = transform.Find("GameEndPanel").gameObject;
        gameEndPanel_resultText = gameEndPanel.transform.Find("GameResultText").GetComponent<TMP_Text>();
        gameEndPanel_totalScore = gameEndPanel.transform.Find("TotalScore").GetComponent<TMP_Text>();
        gameEndPanel_exitToMenuBtn = gameEndPanel.transform.Find("ToMenuBtn").GetComponent<Button>();

        score = transform.Find("Score").GetComponent<TMP_Text>();
        turnsLeft = transform.Find("Turns").GetComponent<TMP_Text>();
        exitToMenuBtn = transform.Find("ExitToMenuBtn").GetComponent<Button>();

        gameEndPanel_exitToMenuBtn.onClick.AddListener(GameManager.Instance.OnExitToMenuBtnPressed);
        exitToMenuBtn.onClick.AddListener(GameManager.Instance.OnExitToMenuBtnPressed);
        gameEndPanel.SetActive(false);

        Player.TurnsChanged += OnTurnsChanged;
        Player.ScoreChanged += OnScoreChanged;
        Player.PassedLevel += OnGameWon;
        Player.GameOver += OnGameOver;
    }

    private void OnScoreChanged(int playerScore)
    {
        score.text = $"Score: {playerScore}";
    }

    private void OnTurnsChanged(int playerTurns)
    {
        turnsLeft.text = $"Turns: {playerTurns}";
    }

    private void DisableLevelUIElements()
    {
        score.gameObject.SetActive(false);
        turnsLeft.gameObject.SetActive(false);
        exitToMenuBtn.gameObject.SetActive(false);
    }

    private void OnGameOver()
    {
        DisableLevelUIElements();
        gameEndPanel.SetActive(true);
        gameEndPanel_totalScore.text = score.text;
        gameEndPanel_resultText.text = "Game Over!";
    }

    private void OnGameWon()
    {
        DisableLevelUIElements();
        gameEndPanel.SetActive(true);
        gameEndPanel_totalScore.text = score.text;
        gameEndPanel_resultText.text = "Уровень пройден!";
    }
}
