using UnityEngine;
using static MainData.SupportScripts;
using UI;
using MainData;

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
        //Debug.LogWarning(Main.playerData.Statistics[0].score);
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
