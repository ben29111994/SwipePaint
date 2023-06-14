using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject inGameUI;
    public GameObject completeUI;

    public void OnReplay()
    {
        Show_InGameUI();
        GameManager.instance.GenerateLevel();
    }

    public void Continue()
    {
        Show_InGameUI();
        GameManager.instance.GenerateLevel();
    }

    public void Show_InGameUI()
    {
        inGameUI.SetActive(true);
        completeUI.SetActive(false);
    }

    public void Show_CompleteUI()
    {
         completeUI.SetActive(true);
    }
}
