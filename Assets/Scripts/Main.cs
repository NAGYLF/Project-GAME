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
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using Unity.VisualScripting;

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
    public class DatabaseManager
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

        public static void LoadAdvancedItems()
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
            private const string AdvancedItemDataFilePath = "Assets/Resources/Items/AdvancedItemData.xlsx";

            private static ItemPartData[] ItemPartDatas;
            private static MainItem[] MainItems;
            public static void AdvancedItemHanderDataLoad()
            {
                IWorkbook wb;
                using (var fs = new FileStream(AdvancedItemDataFilePath, FileMode.Open, FileAccess.Read)) wb = new XSSFWorkbook(fs);
                
                ISheet s = wb.GetSheetAt(1);
                object temporaryList = new List<MainItem>();
                for (int i = 1; i <= s.LastRowNum; i++)
                {
                    var r = s.GetRow(i);
                    string mainItemName = r.GetCell(0).ToString();
                    string[] NecessaryItemNames = r.GetCell(1).ToString().Split(';');
                    ((List<MainItem>)temporaryList).Add(new MainItem(mainItemName, NecessaryItemNames));
                }
                MainItems = ((List<MainItem>)temporaryList).ToArray();
         
                s = wb.GetSheetAt(0);
                temporaryList = new List<ItemPartData>();
                for (int i = 1; i <= s.LastRowNum;)
                {
                    var r = s.GetRow(i);
                    if (r.GetCell(0) != null)
                    {
                        string partName = r.GetCell(0).ToString();
                        string imagePath = r.GetCell(1).ToString();
                        List<CP> cps = new();
                        do
                        {
                            string pointName = r.GetCell(2).ToString();
                            int.TryParse(r.GetCell(3).ToString(), out int layer);
                            string[] compatibleItemNames = r.GetCell(4).ToString().Split(';');
                            short.TryParse(r.GetCell(5).ToString(), out short pixel_1_X);
                            short.TryParse(r.GetCell(6).ToString(), out short pixel_1_Y);
                            short.TryParse(r.GetCell(7).ToString(), out short pixel_2_X);
                            short.TryParse(r.GetCell(8).ToString(), out short pixel_2_Y);
                            cps.Add(new CP(pointName, layer, compatibleItemNames, pixel_1_X, pixel_1_Y, pixel_2_X, pixel_2_Y,imagePath));
                            r = s.GetRow(++i);
                        }
                        while (r != null && r.GetCell(0) == null);
                        ((List<ItemPartData>)temporaryList).Add(new ItemPartData(partName, imagePath, cps.ToArray()));
                    }
                }
                ItemPartDatas = ((List<ItemPartData>)temporaryList).ToArray();
            }
            public static ItemPartData GetPartData(string ItemName)
            {
                return ItemPartDatas.Where(x => x.PartName == ItemName).FirstOrDefault();
            }
            public static MainItem GetMainItemData(string MainItemName)
            {
                return MainItems.Where(x => x.MainItemName == MainItemName).FirstOrDefault();
            }
        }
        public struct ItemPartData
        {
            //ez egy part neve és egyben annak az itemnek a neve amié a part
            public string PartName { private set; get; }
            public string ImagePath { private set; get; }
            public MainItem? MainItem { private set; get; }
            public CP[] CPs { private set; get; }
            public ItemPartData(string partName, string imagePath, CP[] cPs)
            {
                PartName = partName;
                ImagePath = imagePath;
                MainItem = AdvancedItemHandler.GetMainItemData(PartName);
                CPs = cPs;
            }
        }
        public struct MainItem
        {
            public string MainItemName { private set; get; }
            public string[] NecessaryItemNames { private set; get; }
            public MainItem(string mainItemName, string[] necessaryItemNames)
            {
                MainItemName = mainItemName;
                NecessaryItemNames = necessaryItemNames;
            }
        }
        public struct CP
        {
            public string PointName { private set; get; }
            public int Layer { private set; get; }
            public string[] CompatibleItemNames { private set; get; }
            public Vector2 AnchorMin1 { get; private set; }
            public Vector2 AnchorMax1 { get; private set; }
            public Vector2 AnchorMin2 { get; private set; }
            public Vector2 AnchorMax2 { get; private set; }
            public CP(string pointName, int layer, string[] compatibleItemNames, short pixel_1_X, short pixel_1_Y, short pixel_2_X, short pixel_2_Y,string partImgPath)
            {
                Texture2D texture = Resources.Load<Texture2D>(partImgPath);
                float imgWidth = texture.width;
                float imgHeight = texture.height;

                PointName = pointName;
                Layer = layer;
                CompatibleItemNames = compatibleItemNames;
                AnchorMin1 = new Vector2(pixel_1_X / (float)imgWidth, pixel_1_Y / (float)imgHeight);
                AnchorMax1 = new Vector2(pixel_1_X / (float)imgWidth, pixel_1_Y / (float)imgHeight);
                AnchorMin2 = new Vector2(pixel_2_X / (float)imgWidth, pixel_2_Y / (float)imgHeight);
                AnchorMax2 = new Vector2(pixel_2_X / (float)imgWidth, pixel_2_Y / (float)imgHeight);
            }
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
