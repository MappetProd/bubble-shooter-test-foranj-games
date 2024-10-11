using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AboutGameUI : MonoBehaviour
{
    private Button exitToMenuBtn;
    // Start is called before the first frame update
    void Start()
    {

        exitToMenuBtn = transform.Find("BackBtn").gameObject.GetComponent<Button>();
        exitToMenuBtn.onClick.AddListener(GameManager.Instance.OnExitToMenuBtnPressed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
