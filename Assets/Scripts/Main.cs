using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using ItemHandler;
using UnityEngine.SceneManagement;
using ExelFileReader;
using System.Threading.Tasks;

namespace MainData
{
    #region DataBase Connection 
    public class ApiResponse
    {
        [JsonProperty("Player")]
        public PlayerData Player { get; set; }

        [JsonProperty("AdminDetails")]
        public Admin AdminDetails { get; set; }
    }

    public class PlayerData
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("IsAdmin")]
        public bool IsAdmin { get; set; }

        [JsonProperty("IsBanned")]
        public bool IsBanned { get; set; }

        [JsonProperty("Achievements")]
        public List<Achievement> Achievements { get; set; }

        [JsonProperty("Admin")]
        public Admin Admin { get; set; }

        [JsonProperty("Statistics")]
        public List<Statistic> Statistics { get; set; }
    }

    public class Achievement
    {
        // További tulajdonságok, ha szükséges
    }

    public class Statistic
    {
        // További tulajdonságok, ha szükséges
    }

    public class Admin
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("PlayerId")]
        public int PlayerId { get; set; }

        [JsonProperty("DevConsole")]
        public bool DevConsole { get; set; }

        [JsonProperty("Player")]
        public PlayerData Player { get; set; }
    }

    public static class DatabaseManager
    {
        public static async Task<PlayerData> GetDataAsync(string name, string password)
        {
            string url = $"https://localhost:5266/api/Player/GetByName/{name}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogWarning(webRequest.error);
                    return null;
                }
                else
                {
                    Debug.Log("Successful Server connection");
                    string jsonResponse = webRequest.downloadHandler.text;
                    return JsonConvert.DeserializeObject<ApiResponse>(jsonResponse).Player;
                }
            }
        }
    }
    #endregion

    #region BackUI 
    //itt vannak jelen azon UI fumciok melyek nem lathatok, mint a: logIn, logOut
    public class UIFunctions
    {
        public static void ProfileBTStyle()
        {
            string[] namesParts = Main.playerData.Name.Split(' ');
            string monogram = "";
            foreach (string item in namesParts)
            {
                monogram += item.First();
            }
            GameObject.Find("ProfileButton").GetComponentInChildren<TMP_Text>().text = monogram;
        }
        //NewLogin
        public static async void LogIn(string name, string email, string password)//new or another accunt login
        {
            
            Main.playerData = await DatabaseManager.GetDataAsync(name, password);

            if (Main.playerData == null)
            {
                Debug.LogWarning("Login failed. User data is not available.");
                return;
            }
            else
            {
                StreamWriter sw = new StreamWriter("AutoLogUser.txt");
                sw.WriteLine(Main.playerData.Name);
                sw.WriteLine(Main.playerData.Email);
                sw.Close();

                Debug.Log($"login completed: {Main.playerData.Id} - {Main.playerData.Name} - {Main.playerData.Password} - {Main.playerData.Email}  user");
                ProfileBTStyle();
            }
        }
        //AutoLogin
        public static async Task<bool> AutoLogIn()//auto login
        {
            if (File.Exists("AutoLogUser.txt"))
            {
                StreamReader sr = new StreamReader("AutoLogUser.txt");

                Main.playerData = await DatabaseManager.GetDataAsync(sr.ReadLine(), sr.ReadLine());

                if (Main.playerData == null)
                {
                    Debug.LogError("Login failed. User data is not available.");
                    return true;
                }
                else
                {
                    Debug.Log($"login completed: {Main.playerData.Id} - {Main.playerData.Name} - {Main.playerData.Password} - {Main.playerData.Email}  user");
                    return true;
                }
            }
            else
            {
                Debug.Log("AutoLogUser.txt not exists. --> Auto log failed");
                return true;
            }
        }
        public static void LogOut()
        {

        }
    }
    #endregion

    #region Main
    // itt tároljuk a user adatait és azon globális kostruktorokat amelyke az egesz játék alatt ervenyesek pl:targetFPS
    //továbbá eljárásokat melyek bármikor elérhetőknek kell, hogy legyenek pl:Save, Load
    public static class Main
    {
        public const int targetFPS = 60;
        private static List<Item> InvetnoryElemetList;
        public static float DefaultHeight = 0f;//az InGameUI allija be
        public static float DefaultWidth = 0f;//az InGameUI allija be
        public const float DefaultItemSlotSize = 10f;
        public const float SectorScale = 1.3f;//ez az inventorySlotok scale-je ez befolyásolja egy item meretet is
        public static void Save()
        {

        }

        public static void Load()
        {

        }

        public static PlayerData playerData;

        public class AdvancedItemHandler
        {
            public const string PartPath = "GameElements/ItemOPart";
            public const string CPPath = "GameElements/ItemOCP";
            public const string AdvancedItemDataFilePath = "Assets/Resources/Items/AdvancedItemData.xlsx";

            public static Func<string,(int height,int width)> ImgSizeGetter = (string path) =>
            {
                Texture2D texture = Resources.Load<Texture2D>(path);
                return (texture.height, texture.width);
            };

            public static ExelFileReader.ExelFileReader AdvancedItemDatas = new ExelFileReader.ExelFileReader(AdvancedItemDataFilePath,ImgSizeGetter);
        }
    }
    #endregion

    class SupportScripts : MonoBehaviour
    {
        public static float[] Aranyszamitas(float[] szamok, float max)
        {
            float szam4 = max / szamok.Sum();
            float[] retunvalues = new float[szamok.Length];
            for (int i = 0; i < retunvalues.Length; i++)
            {
                retunvalues[i] = szam4 * szamok[i];
            }
            return retunvalues;
        }
        public static GameObject CreatePrefab(string path)
        {
            //Debug.Log($"CreatePrefab path: {path}");
            GameObject prefab = Instantiate(Resources.Load<GameObject>(path));
            //Debug.Log($"CreatePrefab path: {path} DONE");
            if (prefab != null)
            {
                prefab.name = path.Split('/').Last();
                return prefab;
            }
            else
            {
                Debug.LogError($"{path} prefab nem található!");
                return null;
            }
        }
        public static void SceneChange(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
