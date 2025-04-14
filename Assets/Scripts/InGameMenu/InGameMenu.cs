using UnityEngine;
using static MainData.SupportScripts;
using UI;
using MainData;
using Newtonsoft.Json;
using PlayerInventoryClass;
using static PlayerInventoryClass.PlayerInventory;

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
    public async void Save()
    {
        Debug.LogWarning(Main.playerData.Statistics[0].score);
        bool result = await DatabaseManager.SetStatistic(Main.Token, Main.playerData.Statistics[0]);
        //if (!result)
        //{
        //    string token = await DatabaseManager.GetTokenAsync(Main.playerData.Email,Main.playerData.Password);
        //    PlayerData data = await DatabaseManager.GetDataAsync(token);
        //    if (data != null)
        //    {
        //        Main.Token = token;
        //        result = await DatabaseManager.SetStatistic(Main.Token, Main.playerData.Statistics[0]);
        //        if (!result)
        //        {
        //            Debug.LogWarning("hibas mentes");
        //        }
        //    }
        //}

        //LevelManager levelManager = InGameUI.PlayerInventory.GetComponent<PlayerInventory>().levelManager;
        //string json = JsonConvert.SerializeObject(levelManager, Formatting.Indented, new JsonSerializerSettings
        //{
        //    NullValueHandling = NullValueHandling.Ignore,
        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //});
        //Debug.Log(json);
    }
    public async void Load()
    {
        PlayerData playeData = await DatabaseManager.GetDataAsync(Main.Token);
        Main.playerData.Statistics[0] = playeData.Statistics[0];
        Debug.LogWarning(Main.playerData.Statistics[0].score);
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
