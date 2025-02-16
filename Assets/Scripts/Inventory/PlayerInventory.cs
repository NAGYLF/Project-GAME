using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Linq;
using static MainData.SupportScripts;
using ItemHandler;
using Assets.Scripts.Inventory;
using NaturalInventorys;
using static ItemHandler.InventorySystem;

namespace PlayerInventoryClass
{
    public class PlayerInventory : MonoBehaviour
    {
        #region Personal variables
        public GameObject EquipmentsPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        public GameObject SlotPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        public GameObject LootPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        public Camera BlourCamera;
        public Material blurMaterial;
        public static GameObject InventoryObjectRef;

        [HideInInspector] public LevelManager levelManager;// ez lényegében az inventory adatait tartlamazza

        [HideInInspector] public GameObject LootableObject;//ezt kapja mint adat
        [HideInInspector] private GameObject LootContainer;//ezt készíti el az adatokból mint objectum
        #endregion
        public class LevelManager
        {
            public List<Item> Items;
            public int MaxLVL { get; private set; }
            public void SetMaxLVL_And_Sort()
            {
                MaxLVL = Items.Max(item => item.Lvl);
                Items = Items.OrderBy(item => item.Lvl).ThenBy(item => item.LowestSlotUseNumber + ((item.ParentItem == null ? -1 : Items.IndexOf(item.ParentItem)) * 10000)).ToList();
            }
            public LevelManager()
            {
                Items = new List<Item>();
            }
            public void Cleaning()
            {
                foreach (Item item in Items)
                {
                    if (item.ParentItem == null && !item.IsRoot)
                    {
                        InventorySystem.DataDelete(item);
                    }
                }
            }
        }
        private void Awake()
        {
            InventoryObjectRef = gameObject;

            InventoryLoad();
        }
        public void Start()
        {
            // 1. Kamera képarányának kiszámítása
            float aspectRatio = (float)Screen.width / Screen.height;

            // 2. Kamera arányának beállítása
            BlourCamera.aspect = aspectRatio;

            // 3. Shader képarány paraméterének átadása
            blurMaterial.SetFloat("_AspectRatio", aspectRatio);
        }
        public void InventorySave()
        {

        }
        public void CreateEmptyInvenotry()
        {
            Item RootData = new()
            {
                ItemName = "Root",
                Lvl = -1,
                IsRoot = true,
                IsEquipmentRoot = true,
                IsInPlayerInventory = true,
                Container = new Container("GameElements/PlayerInventory"),
                ContainerObject = gameObject,
                SectorDataGrid = gameObject.GetComponent<ContainerObject>().Sectors
            };
            levelManager = new LevelManager();
            levelManager.Items.Add(RootData);
            RootData.LevelManagerRef = levelManager;
            levelManager.SetMaxLVL_And_Sort();
            gameObject.GetComponent<ContainerObject>().SetDataRoute(RootData);
        }
        public void InventoryLoad()//kelelne egy save manager script ami a be ovasat es a kiirast kezelni ezzel lehet idot lehetni sporolni
        {
            if (File.Exists("UserSave.json"))
            {
                //string jsonString = File.ReadAllText("PlayerSave.json");
                //equipments = JsonConvert.DeserializeObject<Equipmnets>(jsonString);
            }
            else
            {
                CreateEmptyInvenotry();
            }
        }
        private bool CanBePlace(ItemSlotData[,] slots, int Y, int X, Item item)
        {
            if (X + item.SizeX <= slots.GetLength(1) && Y + item.SizeY <= slots.GetLength(0))
            {
                for (int y = Y; y < Y + item.SizeY; y++)
                {
                    for (int x = X; x < X + item.SizeX; x++)
                    {
                        if (slots[y, x].PartOfItemData != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }
        private bool AddingByCount(int lvl, Item Data)
        {
            bool ItemAdded = false;
            List<Item> itemsOfLvl = levelManager.Items.Where(Item => Item.Lvl == lvl).ToList();
            for (int itemIndex = 0; itemIndex < itemsOfLvl.Count; itemIndex++)
            {
                if (itemsOfLvl[itemIndex].ItemName == Data.ItemName && itemsOfLvl[itemIndex].Quantity != itemsOfLvl[itemIndex].MaxStackSize)
                {
                    int originalCount = itemsOfLvl[itemIndex].Quantity;
                    itemsOfLvl[itemIndex].Quantity += Data.Quantity;
                    if (itemsOfLvl[itemIndex].Quantity > itemsOfLvl[itemIndex].MaxStackSize)
                    {
                        Data.Quantity -= (itemsOfLvl[itemIndex].MaxStackSize - originalCount);
                        itemsOfLvl[itemIndex].Quantity = itemsOfLvl[itemIndex].MaxStackSize;
                    }
                    else
                    {
                        ItemAdded = true;
                    }
                }
            }
            return ItemAdded;
        }
        private bool AddingByNewItem(int lvl, Item Data)
        {
            List<Item> itemsOfLvl = levelManager.Items.Where(Item => Item.Lvl == lvl && Item.Container != null).ToList();
            for (int itemIndex = 0; itemIndex < itemsOfLvl.Count; itemIndex++)
            {
                for (int sectorIndex = 0; sectorIndex < itemsOfLvl[itemIndex].Container.Sectors.Length; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                {
                    if (itemsOfLvl[itemIndex].IsRoot || (itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(1) >= Data.SizeX && itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(0) >= Data.SizeY))
                    {
                        for (int Y = 0; Y < itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(0); Y++)//vegig iterálunk a sorokon
                        {
                            for (int X = 0; X < itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(1); X++)//a sorokon belul az oszlopokon
                            {
                                if ((itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].SlotType.Contains(Data.ItemType) || itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].SlotType == "") && itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].PartOfItemData == null && (CanBePlace(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex], Y, X, Data) || itemsOfLvl[itemIndex].IsRoot))//ha a slot nem tagja egy itemnek sem akkor target
                                {
                                    AddDataNonLive(Y,X, sectorIndex, itemsOfLvl[itemIndex], Data);
                                    //Debug.Log($"Item Added in container");
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        private bool AddingByNewItemByRotate(int lvl, Item Data)
        {
            Data.RotateDegree = 90;
            (Data.SizeX, Data.SizeY) = (Data.SizeY, Data.SizeX);
            List<Item> itemsOfLvl = levelManager.Items.Where(Item => Item.Lvl == lvl && Item.Container != null).ToList();
            for (int itemIndex = 0; itemIndex < itemsOfLvl.Count; itemIndex++)
            {
                for (int sectorIndex = 0; sectorIndex < itemsOfLvl[itemIndex].Container.Sectors.Length; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                {
                    if (itemsOfLvl[itemIndex].IsRoot || (itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(1) >= Data.SizeX && itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(0) >= Data.SizeY))
                    {
                        for (int Y = 0; Y < itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(0); Y++)//vegig iterálunk a sorokon
                        {
                            for (int X = 0; X < itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(1); X++)//a sorokon belul az oszlopokon
                            {
                                if ((itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].SlotType.Contains(Data.ItemType) || itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].SlotType == "") && itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].PartOfItemData == null && (CanBePlace(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex], Y, X, Data) || itemsOfLvl[itemIndex].IsRoot))//ha a slot nem tagja egy itemnek sem akkor target
                                {
                                    AddDataNonLive(Y, X, sectorIndex, itemsOfLvl[itemIndex], Data);
                                    (Data.SizeX, Data.SizeY) = (Data.SizeY, Data.SizeX);
                                    //Debug.Log($"Item Added in container");
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            Data.RotateDegree = 0;
            (Data.SizeX, Data.SizeY) = (Data.SizeY, Data.SizeX);
            return false;
        }
        public void InventoryAdd(Item item)//az equipmentekbe nem ad count szerint.
        {
            bool ItemAdded = false;
            int quantity = item.Quantity;
            if (item.MaxStackSize > 1)//gyorsitott item hozzadas mely nem ad uj elemet hanem csak quanity-t novel
            {
                for (int lvl = 0; lvl <= levelManager.MaxLVL && !ItemAdded; lvl++)//equipment
                {
                    ItemAdded = AddingByCount(lvl, item);
                }
            }
            if (!ItemAdded)//uj item hozzaadasa
            {
                for (int lvl = -1; lvl <= levelManager.MaxLVL && !ItemAdded; lvl++)//vegig iterálunk az osszes equipmenten
                {
                    ItemAdded = AddingByNewItem(lvl, item);
                    if (!ItemAdded)
                    {
                        ItemAdded = AddingByNewItemByRotate(lvl, item);
                    }
                }
            }
            if (!ItemAdded)
            {
                Debug.LogWarning($"item: {item.ItemName} cannot added, probably no space for that");
            }
        }
        private bool Removing(int lvl,Item Data)
        {
            bool ItemRemoved = false;
            List<Item> itemsOfLvl = levelManager.Items.Where(Item => Item.Lvl == lvl).ToList();
            for (int itemIndex = 0; itemIndex < itemsOfLvl.Count && !ItemRemoved; itemIndex++)
            {
                if (itemsOfLvl[itemIndex].ItemName == Data.ItemName && (itemsOfLvl[itemIndex].Container == null || itemsOfLvl[itemIndex].Container.Items.Count == 0))
                {
                    itemsOfLvl[itemIndex].Quantity -= Data.Quantity;
                    int count = 0;
                    if (itemsOfLvl[itemIndex].Quantity > 0)
                    {
                        ItemRemoved = true;//csak menyiseget törlünk
                    }
                    else if (itemsOfLvl[itemIndex].Quantity == 0)
                    {
                        ItemRemoved = true;
                        DataDelete(itemsOfLvl[itemIndex]);
                    }
                    else
                    {
                        count = Math.Abs(itemsOfLvl[itemIndex].Quantity);
                        Data.Quantity = count;
                        DataDelete(itemsOfLvl[itemIndex]);
                    }
                }
            }
            return ItemRemoved;
        }

        public void InventoryRemove(Item item)//newm torol olyan itemet melynek van item a containerében
        {
            Debug.Log($"Remove: {item.ItemName} - {item.Quantity}db in progress");
            bool ItemRemoved = false;
            for (int lvl = 1; lvl <= levelManager.MaxLVL && !ItemRemoved; lvl++)
            {
                ItemRemoved = Removing(lvl, item);
            }
            if (!ItemRemoved)
            {
                ItemRemoved = Removing(0, item);
            }
            if (!ItemRemoved)
            {
                Debug.LogWarning($"item: {item.ItemName} - {item.Quantity}db cannot be removed, probably not exist");
            }
        }
        
        public void CloseInventory()
        {
            for (int i = EquipmentsPanelObject.GetComponent<PanelMain>().Equipments.transform.childCount - 1; i >= 0; i--)
            {
                if (EquipmentsPanelObject.GetComponent<PanelMain>().Equipments.transform.GetChild(i).GetComponent<ItemObject>())
                {
                    Destroy(EquipmentsPanelObject.GetComponent<PanelMain>().Equipments.transform.GetChild(i).gameObject);
                }
            }
            for (int i = SlotPanelObject.GetComponent<PanelSlots>().Content.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(SlotPanelObject.GetComponent<PanelSlots>().Content.transform.GetChild(i).gameObject);
            }
            for (int i = LootPanelObject.GetComponent<PanelLoot>().Content.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(LootPanelObject.GetComponent<PanelLoot>().Content.transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < gameObject.transform.childCount; i++)//ha egy itememt mozgatunk azt itt teszzuk meg. ezert szukseg van arr, hogyha mozgatas kozben bezarjuk az inventoryt akkor az az obejctum megsemmisuljon
            {
                if (gameObject.transform.GetChild(i).gameObject.GetComponent<ItemObject>())
                {
                    Destroy(gameObject.transform.GetChild(i).gameObject);
                }
                else
                {
                    gameObject.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        public void OpenInventory()
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)//ha egy itememt mozgatunk azt itt teszzuk meg. ezert szukseg van arr, hogyha mozgatas kozben bezarjuk az inventoryt akkor az az obejctum megsemmisuljon
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
            gameObject.GetComponent<ContainerObject>().DataLoad();
        }
        public void LootCreate()
        {
            if (LootableObject != null)
            {
                if (LootableObject.GetComponent<SimpleInventory>() != null)
                {
                    Debug.Log("Player inventory ban " + LootableObject.GetComponent<Interact>().Title);
                    LootContainer = CreatePrefab(LootableObject.GetComponent<SimpleInventory>().PrefabPath);
                    LootContainer.GetComponent<ContainerObject>().SetDataRoute(LootableObject.GetComponent<SimpleInventory>().Root);
                }
            }
        }
        public void LootDelete()
        {
            if (LootContainer != null)
            {
                Destroy(LootContainer);
            }
        }
    }
}