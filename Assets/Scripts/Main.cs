using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Unity.VisualScripting;
using Newtonsoft.Json;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using ItemHandler;
using UnityEngine.SceneManagement;

//test
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
    }
    [System.Serializable]
    public class PlayersArray
    {
        public PlayerData[] items;
    }

    public class DatabaseManager : MonoBehaviour
    {
        private static PlayerData[] playerDatas;//Probléma lehet, de a szervere csatlakozasnal az osszes player adatot lekerdezzuk, ezen a változatatás szükséges
        public IEnumerator ServerConnection()//itt csatlakotunk a szerverhez és lementjuk a playerek adatait
        {

            using (UnityWebRequest webRequest = UnityWebRequest.Get("http://localhost:5269/UnityController"))
            {

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(webRequest.error);
                }
                else
                {
                    Debug.Log(webRequest.downloadHandler.text);

                    // JSON feldolgozása
                    playerDatas = JsonConvert.DeserializeObject<PlayerData[]>(webRequest.downloadHandler.text);
                    Debug.Log("Succesful Server connection");
                    UIFunctions.LogIn();

                }
            }

        }
        public static PlayerData GetData(PlayerData playerData)//itt a beerkezo hiányos playerData változó tartalma alapján kikeressuk a profilt és visszakuldjuk a profil adataival kitöltött playerData változót
        {
            try
            {
                foreach (PlayerData player in playerDatas)
                {
                    if (player.name == playerData.name && player.password == playerData.password && player.email == playerData.email)
                    {
                        return player;
                    }
                }
                Debug.LogError("Login Error, The user not found");
                return null;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                throw;
            }

        }

    }
    #endregion

    #region BackUI 
    //itt vannak jelen azon UI fumciok melyek nem lathatok, mint a: logIn, logOut


    public class UIFunctions : MonoBehaviour
    {
        private static void AccuntSincronisation(PlayerData playerData)
        {
            Main.id = playerData.id;
            Main.name = playerData.name;
            Main.email = playerData.email;
            Main.password = playerData.password;
        }
        private static void LocalAccuntPush(PlayerData playerData)
        {
            StreamWriter sw = new StreamWriter("User.txt");
            sw.WriteLine(playerData.name);
            sw.WriteLine(playerData.email);
            sw.WriteLine(playerData.password);
            sw.Close();
        }
        private static void ProfileBTStyle()
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

            playerData = DatabaseManager.GetData(playerData);
            if (playerData == null)
            {
                Debug.LogError("Login failed. User data is not available.");
                return; // Kilép a metódusból, ha a felhasználói adatok nem elérhetők
            }
            else
            {
                LocalAccuntPush(playerData);
                AccuntSincronisation(playerData);

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

                playerData = DatabaseManager.GetData(playerData);
                if (playerData == null)
                {
                    Debug.LogError("Login failed. User data is not available.");
                    return; // Kilép a metódusból, ha a felhasználói adatok nem elérhetők
                }
                else
                {
                    AccuntSincronisation(playerData);

                    Debug.Log($"server connection is avaiable for: {playerData.id} - {playerData.name} - {playerData.password} - {playerData.email}  user");
                    Debug.Log("Login succesful");
                    ProfileBTStyle();
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
    internal static class Main
    {
        public const int targetFPS = 60;
        private static List<Item> InvetnoryElemetList;
        public static float DefaultHeight = 0f;//az InGameUI allija be
        public static float DefaultWidth = 0f;//az InGameUI allija be
        public const float DefaultItemSlotSize = 10f;
        public const float SectorScale = 0.9f;//ez az inventorySlotok scale-je ez befolyásolja egy item meretet is
        public const float ItemCounterFontSize = 25f;
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
