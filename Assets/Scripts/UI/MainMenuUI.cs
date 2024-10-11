using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    private GameObject confirmExitPanel;
    private GameObject mainMenuPanel;

    private Button newGameBtn;
    private Button aboutGameBtn;
    private Button exitBtn;
    private Button confirmExitBtn;
    private Button confirmStayBtn;

    // Start is called before the first frame update
    void Start()
    {
        confirmExitPanel = transform.Find("ConfirmExit").gameObject;
        mainMenuPanel = transform.Find("MainMenu").gameObject;
        mainMenuPanel.SetActive(true);
        confirmExitPanel.SetActive(false);

        newGameBtn = mainMenuPanel.transform.Find("NewGameBtn").GetComponent<Button>();
        newGameBtn.onClick.AddListener(GameManager.Instance.OnNewGameBtnPressed);
        exitBtn = mainMenuPanel.transform.Find("ExitBtn").GetComponent<Button>();
        exitBtn.onClick.AddListener(OnExitBtnPressed);
        aboutGameBtn = mainMenuPanel.transform.Find("AboutBtn").GetComponent<Button>();
        aboutGameBtn.onClick.AddListener(GameManager.Instance.OnAboutGameBtnPressed);

        confirmExitBtn = confirmExitPanel.transform.Find("ConfirmExitBtn").GetComponent<Button>();
        confirmExitBtn.onClick.AddListener(OnComfirmExitBtnPressed);

        confirmStayBtn = confirmExitPanel.transform.Find("ConfirmStayBtn").GetComponent<Button>();
        confirmStayBtn.onClick.AddListener(OnStayBtnPressed);
    }

    public void OnExitBtnPressed()
    {
        mainMenuPanel.SetActive(false);
        confirmExitPanel.SetActive(true);
    }

    public void OnStayBtnPressed()
    {
        mainMenuPanel.SetActive(true);
        confirmExitPanel.SetActive(false);
    }

    public void OnComfirmExitBtnPressed()
    {
        Application.Quit();
    } 
}
