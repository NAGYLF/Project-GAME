using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MainData.SupportScripts;
using UI;

public class InGameMenu : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void Back()
    {
        InGameUI.InGameMenuOpenClose.Action();
    }
    public void Settings()
    {

    }
    public void Save()
    {

    }
    public void Load()
    {

    }
    public void ExitToTheMainMenu()
    {
        SceneChange("Main Menu");
    }
    public void ExitToTheDesktop()
    {
        Application.Quit();
    }
}
