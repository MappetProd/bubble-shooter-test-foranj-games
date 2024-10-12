using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject();
                _instance = obj.AddComponent<GameManager>();
                DontDestroyOnLoad(obj);
                obj.name = "GameManager";
            }
            return _instance;
        }
    }

    private void Start()
    {
        Player.GameOver += OnGameEnded;
        Player.PassedLevel += OnGameEnded;
    }

    private void OnGameEnded()
    {
        DisableScriptsOfType<PlayerInput>();
        DisableScriptsOfType<BubbleMovement>();
        DisableScriptsOfType<ShotHandler>();
        DisableScriptsOfType<TrajectoryRenderer>();
    }

    private void DisableScriptsOfType<T>() where T : MonoBehaviour
    {
        T[] scripts = GameObject.FindObjectsOfType<T>();
        foreach (T s in scripts)
        {
            s.enabled = false;
        }
    }

    public void OnNewGameBtnPressed()
    {
        SceneManager.LoadScene("Gameplay");
    }
    public void OnAboutGameBtnPressed()
    {
        SceneManager.LoadScene("AboutGame");
    }

    public void OnExitToMenuBtnPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
