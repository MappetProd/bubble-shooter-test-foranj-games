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

    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnNewGameBtnPressed()
    {
        /*# if UNITY_EDITOR
        EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
        #endif

        #if UNITY_64
        #endif*/
        SceneManager.LoadScene("Gameplay");
        //Player player = Player.Instance;
    }
    public void OnAboutGameBtnPressed()
    {
        SceneManager.LoadScene("AboutGame");
    }

    public void OnExitToMenuBtnPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
