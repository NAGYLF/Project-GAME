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

namespace MainData
{
    #region DataBase Connection 
    /*Ez az a programresz ahol a player információkat az API-on keresztul lekerdezzuk*/
    [System.Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public string email;
        public string password;
        bool IsAdmin;
        public Achievement[] achievements;
        public Admin admin;
        public Statistics[] statistics;
    }
    public class Achievement
    {
        public int id;
        public int playerId;
        public bool firstBlood;
        public bool rookieWork;
        public bool youAreOnYourOwnNow;
        public string player;
    }
    public class Admin
    {
        public int id;
        public int playerId;
        public bool devConsole;
        public string player;
    }
    public class Statistics
    {
        public int id;
        public int playerId;
        public int deathCount;
        public int score;
        public int enemiesKilled;
        public string player;
    }
    /*{
  "id": 0,
  "name": "string",
  "password": "string",
  "email": "string",
  "isAdmin": true,
  "achievements": [
    {
      "id": 0,
      "playerId": 0,
      "firstBlood": true,
      "rookieWork": true,
      "youAreOnYourOwnNow": true,
      "player": "string"
    }
  ],
  "admin": {
    "id": 0,
    "playerId": 0,
    "devConsole": true,
    "player": "string"
  },
  "statistics": [
    {
      "id": 0,
      "playerId": 0,
      "deathCount": 0,
      "score": 0,
      "enemiesKilled": 0,
      "player": "string"
    }
  ]
}*/
    [System.Serializable]
    public class PlayersArray
    {
        public PlayerData[] items;
    }
    public static class DatabaseManager
    {
        public static IEnumerator GetData(string name, string password, PlayerData playerData)
        {
            string url = $"https://localhost:5269/api/Player/GetByName/{name}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Elküldjük a GET kérést és megvárjuk a befejeződést
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(webRequest.error);
                    playerData = null;
                }
                else
                {
                    Debug.Log("Successful Server connection");
                    string jsonResponse = webRequest.downloadHandler.text;
                    // JSON deszerializálása a PlayerData típusra
                    playerData = JsonConvert.DeserializeObject<PlayerData>(jsonResponse);
                }
            }
        }
    }
    #endregion

    #region BackUI 
    //itt vannak jelen azon UI fumciok melyek nem lathatok, mint a: logIn, logOut


    public class UIFunctions : MonoBehaviour
    {
        public static void ProfileBTStyle()
        {
            string[] namesParts = Main.name.Split(' ');
            string monogram = "";
            foreach (string item in namesParts)
            {
                monogram += item.First();
            }
            GameObject.Find("ProfileButton").GetComponentInChildren<TMP_Text>().text = monogram;
        }
        //NewLogin
        public static void LogIn(string name, string email, string password)//new or another accunt login
        {

            PlayerData playerData = new PlayerData();

            playerData.name = name;
            playerData.email = email;
            playerData.password = password;

            DatabaseManager.GetData(name, password, playerData);

            if (playerData == null)
            {
                Debug.LogError("Login failed. User data is not available.");
                return; // Kilép a metódusból, ha a felhasználói adatok nem elérhetők
            }
            else
            {
                StreamWriter sw = new StreamWriter("User.txt");
                sw.WriteLine(playerData.name);
                sw.WriteLine(playerData.email);
                sw.WriteLine(playerData.password);
                sw.Close();

                Main.id = playerData.id;
                Main.name = playerData.name;
                Main.email = playerData.email;
                Main.password = playerData.password;

                Debug.Log($"server connection is avaiable for: {playerData.id} - {playerData.name} - {playerData.password} - {playerData.email}  user");
                Debug.Log("Login succesful");
                ProfileBTStyle();
                Main.logged = true;
            }


        }
        //AutoLogin
        public static void LogIn()//auto login
        {
            if (File.Exists("User.txt"))
            {

                PlayerData playerData = new PlayerData();

                StreamReader sr = new StreamReader("User.txt");
                playerData.name = sr.ReadLine();
                playerData.email = sr.ReadLine();
                playerData.password = sr.ReadLine();

                DatabaseManager.GetData(playerData.name, playerData.password, playerData);

                if (playerData == null)
                {
                    Debug.LogError("Login failed. User data is not available.");
                    return; // Kilép a metódusból, ha a felhasználói adatok nem elérhetők
                }
                else
                {
                    Main.id = playerData.id;
                    Main.name = playerData.name;
                    Main.email = playerData.email;
                    Main.password = playerData.password;

                    Debug.Log($"server connection is avaiable for: {playerData.id} - {playerData.name} - {playerData.password} - {playerData.email}  user");
                    Debug.Log("Login succesful");
                    Main.logged = true;
                }
            }
            else
            {
                Debug.Log("User.txt not exists. --> Auto log failed");
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
        public const float SectorScale = 1.1f;//ez az inventorySlotok scale-je ez befolyásolja egy item meretet is
        public static void Save()
        {

        }

        public static void Load()
        {

        }

        public static bool logged = false;
        public static int id = -1;
        public static string name;
        public static string email;
        public static string password;

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

            public static ExelFileReader.ExelFileReader AdvancedItemDatas = new ExelFileReader.ExelFileReader(PartPath,CPPath,AdvancedItemDataFilePath,ImgSizeGetter);
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
