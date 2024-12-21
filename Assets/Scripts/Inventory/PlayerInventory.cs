﻿using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Linq;
using static MainData.SupportScripts;
using ItemHandler;
using Assets.Scripts.Inventory;
using NaturalInventorys;

namespace PlayerInventoryClass
{
    public class PlayerInventory : MonoBehaviour
    {
        #region Personal variables
        public GameObject EquipmentsPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        public GameObject SlotPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        public GameObject LootPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        public static GameObject InventoryObjectRef;

        [HideInInspector] public LevelManager levelManager;// ez lényegében az inventory adatait tartlamazza

        [HideInInspector] public GameObject LootableObject;//ezt kapja mint adat
        private GameObject LootContainer;//ezt készíti el az adatokból mint objectum
        #endregion
        public class LevelManager
        {
            public List<Item> Items;
            public int MaxLVL { get; private set; }
            public void SetMaxLVL_And_Sort()
            {
                MaxLVL = Items.Max(item => item.lvl);
                Items = Items.OrderBy(item => item.lvl).ThenBy(item => item.LowestSlotUseNumber + ((item.ParentItem == null ? -1 : Items.IndexOf(item.ParentItem)) * 10000)).ToList();
            }
            public LevelManager()
            {
                Items = new List<Item>();
            }
        }
        private void Awake()
        {
            InventoryObjectRef = gameObject;

            InventoryLoad();
        }
        public void InventorySave()
        {

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
                Item RootData = new Item();
                RootData.ItemName = "Root";
                RootData.lvl = -1;
                RootData.IsRoot = true;
                RootData.Container = new Container("GameElements/PlayerInventory");
                RootData.ContainerObject = gameObject;
                RootData.SectorDataGrid = gameObject.GetComponent<ContainerObject>().Sectors;
                levelManager = new LevelManager();
                levelManager.Items.Add(RootData);
                levelManager.SetMaxLVL_And_Sort();
                gameObject.GetComponent<ContainerObject>().SetDataRoute(RootData);
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
            List<Item> itemsOfLvl = levelManager.Items.Where(Item => Item.lvl == lvl).ToList();
            for (int itemIndex = 0; itemIndex < itemsOfLvl.Count && !ItemAdded; itemIndex++)
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
        private (bool, int) AddingByNewItem(int lvl, Item Data)
        {
            bool ItemAdded = false;
            List<Item> itemsOfLvl = levelManager.Items.Where(Item => Item.lvl == lvl && Item.Container != null).ToList();
            for (int itemIndex = 0; itemIndex < itemsOfLvl.Count && !ItemAdded; itemIndex++)
            {
                for (int sectorIndex = 0; sectorIndex < itemsOfLvl[itemIndex].Container.Sectors.Length && !ItemAdded; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                {
                    if (itemsOfLvl[itemIndex].IsRoot || (itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(1) >= Data.SizeX && itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(0) >= Data.SizeY))
                    {
                        for (int Y = 0; Y < itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(0) && !ItemAdded; Y++)//vegig iterálunk a sorokon
                        {
                            for (int X = 0; X < itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(1) && !ItemAdded; X++)//a sorokon belul az oszlopokon
                            {
                                if ((itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].SlotType.Contains(Data.ItemType) || itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].SlotType == "") && itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].PartOfItemData == null && (CanBePlace(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex], Y, X, Data) || itemsOfLvl[itemIndex].IsRoot))//ha a slot nem tagja egy itemnek sem akkor target
                                {
                                    List<ItemSlotData> itemSlots = new();
                                    if (itemsOfLvl[itemIndex].IsRoot)
                                    {
                                        itemSlots.Add(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X]);
                                        Data.SlotUse.Add(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].SlotName);//ez alapjan azonositunk egy itemslotot
                                    }
                                    else
                                    {
                                        for (int y = Y; y < Y + Data.SizeY; y++)
                                        {
                                            for (int x = X; x < X + Data.SizeX; x++)
                                            {
                                                itemSlots.Add(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][y, x]);
                                                Data.SlotUse.Add(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][y, x].SlotName);//ez alapjan azonositunk egy itemslotot
                                            }
                                        }
                                    }
                                    Data.SetSlotUse();
                                    Data.ParentItem = itemsOfLvl[itemIndex];
                                    Data.lvl = itemsOfLvl[itemIndex].lvl;
                                    Data.lvl++;
                                    levelManager.Items.Add(Data);
                                    levelManager.SetMaxLVL_And_Sort();
                                    foreach (ItemSlotData itemSlot in itemsOfLvl[itemIndex].Container.Sectors[sectorIndex])
                                    {
                                        if (itemSlots.Exists(slot => slot.SlotName == itemSlot.SlotName))
                                        {
                                            itemSlot.PartOfItemData = Data;
                                        }
                                    }
                                    itemsOfLvl[itemIndex].Container.Items.Add(Data);
                                    Debug.Log($"Item Added in container");
                                    int count = 0;
                                    if (Data.Quantity > Data.MaxStackSize)
                                    {
                                        count = Data.Quantity - Data.MaxStackSize;
                                        Data.Quantity = Data.MaxStackSize;
                                        Data = new Item(Data.ItemName, count);
                                    }
                                    else
                                    {
                                        return  (true, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return (ItemAdded, Data.Quantity);
        }
        private (bool,int) AddingByNewItemByRotate(int lvl, Item Data)
        {
            bool ItemAdded = false;
            Data.RotateDegree = 90;
            (Data.SizeX, Data.SizeY) = (Data.SizeY, Data.SizeX);
            List<Item> itemsOfLvl = levelManager.Items.Where(Item => Item.lvl == lvl && Item.Container != null).ToList();
            for (int itemIndex = 0; itemIndex < itemsOfLvl.Count && !ItemAdded; itemIndex++)
            {
                for (int sectorIndex = 0; sectorIndex < itemsOfLvl[itemIndex].Container.Sectors.Length && !ItemAdded; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                {
                    if (itemsOfLvl[itemIndex].IsRoot || (itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(1) >= Data.SizeX && itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(0) >= Data.SizeY))
                    {
                        for (int Y = 0; Y < itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(0) && !ItemAdded; Y++)//vegig iterálunk a sorokon
                        {
                            for (int X = 0; X < itemsOfLvl[itemIndex].Container.Sectors[sectorIndex].GetLength(1) && !ItemAdded; X++)//a sorokon belul az oszlopokon
                            {
                                if ((itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].SlotType.Contains(Data.ItemType) || itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].SlotType == "") && itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].PartOfItemData == null && (CanBePlace(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex], Y, X, Data) || itemsOfLvl[itemIndex].IsRoot))//ha a slot nem tagja egy itemnek sem akkor target
                                {
                                    List<ItemSlotData> itemSlots = new();
                                    if (itemsOfLvl[itemIndex].IsRoot)
                                    {
                                        itemSlots.Add(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X]);
                                        Data.SlotUse.Add(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][Y, X].SlotName);//ez alapjan azonositunk egy itemslotot
                                    }
                                    else
                                    {
                                        for (int y = Y; y < Y + Data.SizeY; y++)
                                        {
                                            for (int x = X; x < X + Data.SizeX; x++)
                                            {
                                                itemSlots.Add(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][y, x]);
                                                Data.SlotUse.Add(itemsOfLvl[itemIndex].Container.Sectors[sectorIndex][y, x].SlotName);//ez alapjan azonositunk egy itemslotot
                                            }
                                        }
                                    }
                                    Data.SetSlotUse();
                                    Data.ParentItem = itemsOfLvl[itemIndex];
                                    Data.lvl = itemsOfLvl[itemIndex].lvl;
                                    Data.lvl++;
                                    (Data.SizeX, Data.SizeY) = (Data.SizeY, Data.SizeX);
                                    levelManager.Items.Add(Data);
                                    levelManager.SetMaxLVL_And_Sort();
                                    foreach (ItemSlotData itemSlot in itemsOfLvl[itemIndex].Container.Sectors[sectorIndex])
                                    {
                                        if (itemSlots.Exists(slot => slot.SlotName == itemSlot.SlotName))
                                        {
                                            itemSlot.PartOfItemData = Data;
                                        }
                                    }
                                    itemsOfLvl[itemIndex].Container.Items.Add(Data);
                                    Debug.Log($"Item Added in container");
                                    int count = 0;
                                    if (Data.Quantity > Data.MaxStackSize)
                                    {
                                        count = Data.Quantity - Data.MaxStackSize;
                                        Data.Quantity = Data.MaxStackSize;
                                        Data = new Item(Data.ItemName, count)
                                        {
                                            RotateDegree = 90
                                        };
                                        (Data.SizeX, Data.SizeY) = (Data.SizeY, Data.SizeX);
                                    }
                                    else
                                    {
                                        return (true, 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Data.RotateDegree = 0;
            (Data.SizeX, Data.SizeY) = (Data.SizeY, Data.SizeX);
            return (ItemAdded, Data.Quantity);
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
                    item = new Item(item.ItemName, quantity);
                    (ItemAdded, quantity) = AddingByNewItem(lvl, item);
                    if (!ItemAdded)
                    {
                        item = new Item(item.ItemName,quantity);
                        (ItemAdded, quantity) = AddingByNewItemByRotate(lvl, item);
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
            List<Item> itemsOfLvl = levelManager.Items.Where(Item => Item.lvl == lvl).ToList();
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
                        levelManager.Items.Remove(itemsOfLvl[itemIndex]);
                        itemsOfLvl[itemIndex].ParentItem.Container.Items.Remove(itemsOfLvl[itemIndex]);
                        foreach (ItemSlotData[,] sector in itemsOfLvl[itemIndex].ParentItem.Container.Sectors)
                        {
                            foreach (ItemSlotData itemSlot in sector)
                            {
                                if (itemsOfLvl[itemIndex].SlotUse.Contains(itemSlot.SlotName))
                                {
                                    itemSlot.PartOfItemData = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        count = Math.Abs(itemsOfLvl[itemIndex].Quantity);
                        Data.Quantity = count;
                        levelManager.Items.Remove(itemsOfLvl[itemIndex]);
                        itemsOfLvl[itemIndex].ParentItem.Container.Items.Remove(itemsOfLvl[itemIndex]);
                        foreach (ItemSlotData[,] sector in itemsOfLvl[itemIndex].ParentItem.Container.Sectors)
                        {
                            foreach (ItemSlotData itemSlot in sector)
                            {
                                if (itemsOfLvl[itemIndex].SlotUse.Contains(itemSlot.SlotName))
                                {
                                    itemSlot.PartOfItemData = null;
                                }
                            }
                        }
                    }
                }
            }
            levelManager.SetMaxLVL_And_Sort();
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
            for (int i = EquipmentsPanelObject.transform.childCount - 1; i >= 0; i--)
            {
                if (EquipmentsPanelObject.transform.GetChild(i).GetComponent<ItemObject>())
                {
                    Destroy(EquipmentsPanelObject.transform.GetChild(i).gameObject);
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
            EquipmentsPanelObject.SetActive(false);
            SlotPanelObject.SetActive(false);
            LootPanelObject.SetActive(false);
        }
        public void OpenInventory()
        {
            EquipmentsPanelObject.SetActive(true);
            SlotPanelObject.SetActive(true);
            LootPanelObject.SetActive(true);
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