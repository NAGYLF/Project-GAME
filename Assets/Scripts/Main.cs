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
using System.Threading.Tasks;
using Items;
using ExcelDataReader;
using System.Data;
using System.Text;
using System.Net.Http;
using UnityEngine.Rendering.VirtualTexturing;
using System.Drawing;

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
        public static async Task<string> GetTokenAsync(string email, string password)
        {
            var payload = new {email,password };
            string json = JsonConvert.SerializeObject(payload);
            Debug.LogWarning(json);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            string url = "https://localhost:5266/api/auth/login";
            using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                var operation = webRequest.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogWarning(webRequest.error);
                    return null;
                }
                else
                {
                    Debug.Log("Successful Server connection");
                    string jsonResponse = webRequest.downloadHandler.text;
                    // A szerver visszatér egy objektummal, melynek van egy "Token" mezője
                    var tokenObj = JsonConvert.DeserializeAnonymousType(jsonResponse, new { Token = "" });

                    return tokenObj.Token;
                }
            }
        }
        public static async Task<PlayerData> GetDataAsync(string token)
        {
            // Token hozzáadása az URL-hez query paraméterként
            string url = $"https://localhost:5266/api/Player/GetByToken?token={UnityWebRequest.EscapeURL(token)}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                var operation = webRequest.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
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
        public static async Task<bool> LogIn(string name, string email, string password)//new or another accunt login
        {
            Main.Token = await DatabaseManager.GetTokenAsync(email, password);
            Main.playerData = await DatabaseManager.GetDataAsync(Main.Token);

            if (Main.playerData == null)
            {
                Debug.LogWarning("Login failed. User data is not available.");
                return false;
            }
            else
            {
                using (StreamWriter sw = new StreamWriter("AutoLogUser.txt"))
                {
                    await sw.WriteLineAsync(email);
                    await sw.WriteLineAsync(password);
                }

                Debug.Log($"login completed: {Main.playerData.Id} - {Main.playerData.Name} - {Main.playerData.Password} - {Main.playerData.Email}  user");

                ProfileBTStyle();

                return true;
            }
        }
        //AutoLogin
        public static async Task<bool> AutoLogIn()//auto login
        {
            Debug.Log("auto login start");
            if (File.Exists("AutoLogUser.txt"))
            {
                StreamReader sr = new StreamReader("AutoLogUser.txt");

                string email = sr.ReadLine();
                string password = sr.ReadLine();
                Main.Token = await DatabaseManager.GetTokenAsync(email, password);
                Main.playerData = await DatabaseManager.GetDataAsync(Main.Token);

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
        private static List<AdvancedItem> InvetnoryElemetList;
        public static float DefaultHeight = 0f;//az InGameUI allija be
        public static float DefaultWidth = 0f;//az InGameUI allija be
        public const float DefaultItemSlotSize = 10f;
        public const float EquipmentSlotColliderScale = 0.35f;
        public const float SectorScale = 1.3f;//ez az inventorySlotok scale-je ez befolyásolja egy item meretet is
        public const float CharacterHandSize = 2f;// az a tvaolsag ami a kéz (kör) átmérője lehet, ehez az ertekhez nagyitja vagy kicsinyiti egy item itemcompound-jat a first hand 2 refpontja közötti tavolsagot vaszi refrenciaul
        public static void Save()
        {

        }

        public static void Load()
        {

        }

        public static PlayerData playerData { get; set; }

        public static string Token { get; set; }

        public class DataHandler
        {
            public const string PartPath = "GameElements/ItemOPart";
            public const string CPPath = "GameElements/ItemOCP";
            public static string AdvancedItemDataFilePath = Path.Combine(Application.streamingAssetsPath, "AdvancedItemData.xlsx");

            public static PartData[] ItemPartDatas { get; private set; }
            public static MainItem[] MainItems { get; private set; }
            public static AdvancedItemStruct[] AdvancedItems { get; private set; } = new AdvancedItemStruct[0];

            public static Func<string,(int height,int width)> TextureSizeGetter = (string path) =>
            {
                Texture2D texture = Resources.Load<Texture2D>(path);
                return (texture.height, texture.width);
            };

            public static void AdvancedItemHanderDataLoad()
            {
                // Regisztráljuk a kódolási providert, hogy a régebbi code page-ek elérhetőek legyenek.
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var fs = new FileStream(AdvancedItemDataFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(fs))
                    {
                        // Konfiguráció: header sor nem használatos (UseHeaderRow = false)
                        var conf = new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = false
                            }
                        };

                        DataSet ds = reader.AsDataSet(conf);

                        for (int i = 3; i < 29; i++)
                        {
                            DataTable advancedItemTable = ds.Tables[i];

                            var advancedItemList = new List<AdvancedItemStruct>();

                            for (int j = 1; j < advancedItemTable.Rows.Count; j++)
                            {
                                DataRow row = advancedItemTable.Rows[j];

                                if (row[0] != DBNull.Value && !string.IsNullOrEmpty(row[0].ToString()))
                                {
                                    //advanced item data section
                                    int index = 0;

                                    string systemNames = row[index++].ToString();
                                    string itemName = row[index++].ToString();
                                    string type = row[index++].ToString();
                                    string desctription = row[index++].ToString();

                                    int quantity = int.Parse(row[index++].ToString());
                                    int maxStackSize = int.Parse(row[index++].ToString());
                                    int value = int.Parse(row[index++].ToString());
                                    int sizeX = int.Parse(row[index++].ToString());
                                    int sizeY = int.Parse(row[index++].ToString());

                                    string[] sizeChangerDataArray;
                                    SizeChanger sizeChanger;
                                    if (row[index] != DBNull.Value && !string.IsNullOrEmpty(row[index].ToString()))
                                    {
                                        sizeChangerDataArray = row[index++]?.ToString().Split(';');
                                        sizeChanger = new SizeChanger(int.Parse(sizeChangerDataArray[0]), int.Parse(sizeChangerDataArray[1]), char.Parse(sizeChangerDataArray[2]));
                                    }
                                    else
                                    {
                                        sizeChanger = new SizeChanger(0, 0, '-');
                                        index++;
                                    }

                                    bool isDropAble = Convert.ToBoolean(int.Parse(row[index++].ToString()));
                                    bool isRemoveAble = Convert.ToBoolean(int.Parse(row[index++].ToString()));
                                    bool isUnloadAble = Convert.ToBoolean(int.Parse(row[index++].ToString()));
                                    bool isModificationAble = Convert.ToBoolean(int.Parse(row[index++].ToString()));
                                    bool isOpenAble = Convert.ToBoolean(int.Parse(row[index++].ToString()));
                                    bool isUsable = Convert.ToBoolean(int.Parse(row[index++].ToString()));
                                    bool canReload = Convert.ToBoolean(int.Parse(row[index++].ToString()));

                                    string containerPath;
                                    if (row[index] != DBNull.Value && !string.IsNullOrEmpty(row[index].ToString()))
                                    {
                                        containerPath = row[index++].ToString();
                                    }
                                    else
                                    {
                                        containerPath = "-";
                                        index++;
                                    }
                                    //----------

                                    //weapon section
                                    int.TryParse(row[index++].ToString(), out int magasineSize);
                                    double.TryParse(row[index++].ToString(), out double spread);
                                    int.TryParse(row[index++].ToString(), out int fpm);
                                    double.TryParse(row[index++].ToString(), out double recoil);
                                    double.TryParse(row[index++].ToString(), out double accturacy);
                                    double.TryParse(row[index++].ToString(), out double range);
                                    double.TryParse(row[index++].ToString(), out double ergonomy);
                                    //----------

                                    //usable section
                                    int.TryParse(row[index++].ToString(), out int maxUse);
                                    int.TryParse(row[index++].ToString(), out int useLeft);
                                    //----------

                                    //ammo section
                                    float.TryParse(row[index++].ToString(), out float caliber);
                                    float.TryParse(row[index++].ToString(), out float cartridgeSize);
                                    float.TryParse(row[index++].ToString(), out float dmg);
                                    float.TryParse(row[index++].ToString(), out float apPower);
                                    float.TryParse(row[index++].ToString(), out float mass);
                                    float.TryParse(row[index++].ToString(), out float muzzleVelocity);
                                    //----------

                                    advancedItemList.Add(new AdvancedItemStruct(systemNames, itemName, type, desctription, quantity, maxStackSize, value, sizeX, sizeY, sizeChanger, isDropAble, isRemoveAble, isUnloadAble, isModificationAble, isOpenAble, isUsable, canReload, containerPath, magasineSize, spread, fpm, recoil, accturacy, range, ergonomy, useLeft, maxUse, caliber, cartridgeSize, dmg, apPower, mass, muzzleVelocity));
                                }
                            }
                            AdvancedItems = AdvancedItems.Concat(advancedItemList.ToArray()).ToArray();
                        }
                        // -------------------------------
                        // 1. MainItem adatok beolvasása (Sheet index 1)
                        // -------------------------------
                        DataTable mainItemsTable = ds.Tables[1];
                        var mainItemList = new List<MainItem>();

                        // Feltételezzük, hogy az első sor (index 0) header, ezért i=1-től indulunk.
                        for (int i = 1; i < mainItemsTable.Rows.Count; i++)
                        {
                            int index = 0;
                            DataRow row = mainItemsTable.Rows[i];
                            // Ellenőrizzük, hogy az első cella nem üres
                            if (row[index] != DBNull.Value && !string.IsNullOrEmpty(row[index].ToString()))
                            {
                                string mainItemName = row[index++].ToString();
                                string systemName = row[index++].ToString();
                                string type = row[index++].ToString();
                                string desctription = row[index++].ToString();
                                string[] necessaryItemType = row[index++].ToString().Split(';');
                                string connectedItemName = row[index++].ToString();
                                string shootSoundPath = row[index++].ToString();
                                string reloadSoundPath = row[index++].ToString();
                                string unloadSoundPath = row[index++].ToString();
                                string chamberSoundPath = row[index++].ToString();
                                string bulletTexturePath = row[index++].ToString();
                                mainItemList.Add(new MainItem(mainItemName, systemName, type, desctription, necessaryItemType, connectedItemName,shootSoundPath,reloadSoundPath,unloadSoundPath,chamberSoundPath,bulletTexturePath));
                            }
                        }
                        MainItems = mainItemList.ToArray();

                        // -------------------------------
                        // 2. ItemPartData adatok beolvasása (Sheet index 0)
                        // -------------------------------
                        DataTable partDataTable = ds.Tables[0];
                        var partDataList = new List<PartData>();
                        int i0 = 1; // Első sor header, ezért i=1

                        while (i0 < partDataTable.Rows.Count)
                        {
                            DataRow row = partDataTable.Rows[i0];
                            if (row[0] != DBNull.Value && !string.IsNullOrEmpty(row[0].ToString()))
                            {
                                string partName = row[0].ToString();
                                string imagePath = row[1].ToString();
                                var cpList = new List<CP>();
                                var spList = new List<SP>();

                                // A do-while ciklus: addig olvassuk a CP adatokat,
                                // amíg az aktuális sor első cellája üres (vagy null) és a harmadik cella nem üres.
                                do
                                {
                                    string pointType = row[2].ToString();
                                    if (pointType == "CP")
                                    {
                                        string pointName = row[3].ToString();
                                        int.TryParse(row[4].ToString(), out int layer);
                                        string[] compatibleItemNames = row[5].ToString().Split(';');
                                        short.TryParse(row[6].ToString(), out short pixel_1_X);
                                        short.TryParse(row[7].ToString(), out short pixel_1_Y);
                                        short.TryParse(row[8].ToString(), out short pixel_2_X);
                                        short.TryParse(row[9].ToString(), out short pixel_2_Y);
                                        cpList.Add(new CP(pointName, layer, compatibleItemNames, pixel_1_X, pixel_1_Y, pixel_2_X, pixel_2_Y, TextureSizeGetter(imagePath)));
                                    }
                                    else
                                    {
                                        string pointName = row[3].ToString();
                                        int.TryParse(row[4].ToString(), out int layer);

                                        short.TryParse(row[6].ToString(), out short pixel_1_X);
                                        short.TryParse(row[7].ToString(), out short pixel_1_Y);
                                        short.TryParse(row[8].ToString(), out short pixel_2_X);
                                        short.TryParse(row[9].ToString(), out short pixel_2_Y);
                                        spList.Add(new SP(pointName, layer, pixel_1_X, pixel_1_Y, pixel_2_X, pixel_2_Y, TextureSizeGetter(imagePath)));
                                    }
                                    i0++;
                                    if (i0 >= partDataTable.Rows.Count)
                                        break;
                                    row = partDataTable.Rows[i0];
                                }
                                while ((row[0] == DBNull.Value || string.IsNullOrEmpty(row[0].ToString()))
                                       && row[2] != DBNull.Value && !string.IsNullOrEmpty(row[2].ToString()));

                                partDataList.Add(new PartData(partName, imagePath, cpList.ToArray(), spList.ToArray(), GetMainItemData(partName)));
                            }
                            else
                            {
                                break;
                            }
                        }
                        ItemPartDatas = partDataList.ToArray();
                    }
                }
            }

            public static PartData GetPartData(string SystemName)
            {
                return ItemPartDatas.FirstOrDefault(x => x.PartName == SystemName);
            }
            public static MainItem GetMainItemData(string ItemName)
            {
                return MainItems.FirstOrDefault(x => x.ConnectedItemName == ItemName);
            }
            public static AdvancedItemStruct GetAdvancedItemData(string SystemName)
            {
                return AdvancedItems.FirstOrDefault(x => x.SystemName == SystemName);
            }
        }

        // -------------------------
        // Advanced Item definíciók
        // -------------------------
        public struct AdvancedItemStruct
        {
            public string SystemName { set; get; }
            public string ItemName { set; get; }
            public string Type { set; get; }
            public string Description { set; get; }
            public int Quantity { set; get; }
            public int MaxStackSize { set; get; }
            public int Value { set; get; }
            public int SizeX { set; get; }
            public int SizeY { set; get; }

            public SizeChanger SizeChanger { set; get; }

            public bool IsDropAble { set; get; }
            public bool IsRemoveAble { set; get; }
            public bool IsUnloadAble { set; get; }
            public bool IsModificationAble { set; get; }
            public bool IsOpenAble { set; get; }
            public bool IsUsable { set; get; }
            public bool CanReload { set; get; }

            public string ContainerPath { set; get; }

            public int MagasineSize { get; set; }
            public double Spread { get; set; }
            public int Fpm { get; set; }
            public double Recoil { get; set; }
            public double Accturacy { get; set; }
            public double Range { get; set; }
            public double Ergonomy { get; set; }

            public int UseLeft { get; set; }
            public int MaxUse { get; set; }

            public float Caliber { get; set; }
            public float CartridgeSize { get; set; }
            public float Dmg { get; set; }
            public float APPower { get; set; }
            public float Mass { get; set; }
            public float MuzzleVelocity { get; set; }

            public object Actions { get; set; }
            public AdvancedItemStruct(string systemName, string itemName, string type, string description, int quantity, int maxStackSize, int value, int sizeX, int sizeY, SizeChanger sizeChanger, bool isDropAble, bool isRemoveAble, bool isUnloadAble, bool isModificationAble, bool isOpenAble, bool isUsable, bool canReload, string containerPath, int magasineSize, double spread, int fpm, double recoil, double accturacy, double range, double ergonomy, int useLeft, int maxUse, float caliber,float cartridgeSize ,float dmg, float aPPower, float mass, float muzzleVelocity)
            {
                SystemName = systemName;
                ItemName = itemName;
                Type = type;
                Description = description;
                Quantity = quantity;
                MaxStackSize = maxStackSize;
                Value = value;
                SizeX = sizeX;
                SizeY = sizeY;

                SizeChanger = sizeChanger;

                IsDropAble = isDropAble;
                IsRemoveAble = isRemoveAble;
                IsUnloadAble = isUnloadAble;
                IsModificationAble = isModificationAble;
                IsOpenAble = isOpenAble;
                IsUsable = isUsable;
                CanReload = canReload;

                ContainerPath = containerPath;

                MagasineSize = magasineSize;
                Spread = spread;
                Fpm = fpm;
                Recoil = recoil;
                Accturacy = accturacy;
                Range = range;
                Ergonomy = ergonomy;

                UseLeft = useLeft;
                MaxUse = maxUse;

                Caliber = caliber;
                CartridgeSize = cartridgeSize;
                Dmg = dmg;
                APPower = aPPower;
                Mass = mass;
                MuzzleVelocity = muzzleVelocity;

                Actions = null;
            }
        }
        public struct SizeChanger
        {
            public int Plus { set; get; }
            public int MaxPlus { set; get; }
            public char Direction { set; get; }
            public SizeChanger(int plus, int maxPlus, char direction)
            {
                Plus = plus;
                MaxPlus = maxPlus;
                Direction = direction;
            }
        }


        // -------------------------
        // Part definíciók
        // -------------------------
        public struct PartData
        {
            // Ez egy part neve és egyben annak az itemnek a neve, amié a part
            public string PartName { set; get; }
            public string ImagePath { set; get; }
            public MainItem MainItem { set; get; }
            public CP[] CPs { set; get; }
            public SP[] SPs { set; get; }
            public PartData(string partName, string imagePath, CP[] cPs, SP[] sPs, MainItem MainItem)
            {
                PartName = partName;
                ImagePath = imagePath;
                this.MainItem = MainItem;
                CPs = cPs;
                SPs = sPs;
            }
        }

        public struct MainItem
        {
            public string MainItemName { set; get; }
            public string SystemName { set; get; }
            public string Type { set; get; }
            public string Desctription { set; get; }
            public string[] NecessaryItemTypes { set; get; }
            public string ConnectedItemName { set; get; }

            public string ShootSoundPath { set; get; }
            public string ReloadSoundPath { set; get; }
            public string UnloadSoundPath { set; get; }
            public string ChamberSoundPath { set; get; }

            public string BulletTexturePath { set; get; }
            public MainItem(string mainItemName, string systemName, string type, string desctription, string[] necessaryItemTypes, string connectedItemName, string shootSoundPath, string reloadSoundPath, string unloadSoundPath, string chamberSoundPath, string bulletTexturePath)
            {
                MainItemName = mainItemName;
                SystemName = systemName;
                Type = type;
                Desctription = desctription;
                NecessaryItemTypes = necessaryItemTypes;
                ConnectedItemName = connectedItemName;
                ShootSoundPath = shootSoundPath;
                ReloadSoundPath = reloadSoundPath;
                UnloadSoundPath = unloadSoundPath;
                ChamberSoundPath = chamberSoundPath;
                BulletTexturePath = bulletTexturePath;
            }
        }

        public struct CP
        {
            public string PointName { set; get; }
            public int Layer { set; get; }
            public string[] CompatibleItemNames { set; get; }
            public Vector2 AnchorMin1 { get; private set; }
            public Vector2 AnchorMax1 { get; private set; }
            public Vector2 AnchorMin2 { get; private set; }
            public Vector2 AnchorMax2 { get; private set; }

            public CP(string pointName, int layer, string[] compatibleItemNames, short pixel_1_X, short pixel_1_Y, short pixel_2_X, short pixel_2_Y, (int Height, int Width) ImgSize)
            {
                float imgWidth = ImgSize.Width;
                float imgHeight = ImgSize.Height;

                PointName = pointName;
                Layer = layer;
                CompatibleItemNames = compatibleItemNames;
                AnchorMin1 = new Vector2(pixel_1_X / imgWidth, 1 - (pixel_1_Y / imgHeight));
                AnchorMax1 = new Vector2(pixel_1_X / imgWidth, 1 - (pixel_1_Y / imgHeight));
                AnchorMin2 = new Vector2(pixel_2_X / imgWidth, 1 - (pixel_2_Y / imgHeight));
                AnchorMax2 = new Vector2(pixel_2_X / imgWidth, 1 - (pixel_2_Y / imgHeight));
            }
        }
        public struct SP
        {
            public string PointName { set; get; }
            public int Layer { set; get; }
            public Vector2 AnchorMin1 { get; private set; }
            public Vector2 AnchorMax1 { get; private set; }
            public Vector2 AnchorMin2 { get; private set; }
            public Vector2 AnchorMax2 { get; private set; }
            public SP(string pointName, int layer, short pixel_1_X, short pixel_1_Y, short pixel_2_X, short pixel_2_Y, (int Height, int Width) ImgSize)
            {
                float imgWidth = ImgSize.Width;
                float imgHeight = ImgSize.Height;

                PointName = pointName;
                Layer = layer;
                AnchorMin1 = new Vector2(pixel_1_X / imgWidth, 1 - (pixel_1_Y / imgHeight));
                AnchorMax1 = new Vector2(pixel_1_X / imgWidth, 1 - (pixel_1_Y / imgHeight));
                AnchorMin2 = new Vector2(pixel_2_X / imgWidth, 1 - (pixel_2_Y / imgHeight));
                AnchorMax2 = new Vector2(pixel_2_X / imgWidth, 1 - (pixel_2_Y / imgHeight));
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
