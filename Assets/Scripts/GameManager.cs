using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
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
