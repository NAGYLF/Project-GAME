using Ammunition;
using Armors;
using Assets.Scripts;
using Backpacks;
using Boots;
using Fingers;
using Headsets;
using Helmets;
using Masks;
using Melees;
using Pants;
using Skins;
using Vests;
using Weapons;
using System;
using Cash;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using NaturalInventorys;
using Newtonsoft.Json.Linq;
using PlayerInventoryClass;
using static PlayerInventoryClass.PlayerInventory;
using UI;

namespace ItemHandler
{
    public struct PlacerStruct
    {
        public List<GameObject> activeItemSlots { get; set; }
        public Item NewParentData { get; set; }
    }
    [Serializable]
    public class DataGrid
    {
        public int rowNumber;
        public int columnNumber;
        public List<RowData> col;
    }
    [Serializable]
    public class RowData
    {
        public List<GameObject> row;
    }
    public class ItemSlotData
    {
        public string SlotName;
        public string SlotType;
        public Item PartOfItemData;
        public ItemSlotData(string SlotName = "", string SlotType = "", Item PartOfItemData = null)
        {
            this.SlotName = SlotName;
            this.SlotType = SlotType;
            this.PartOfItemData = PartOfItemData;
        }
    }
    public class Item : NonGeneralItemProperties
    {
        //system variables
        public Item ParentItem;
        public GameObject SelfGameobject { get; set; }// a parent objectum
        public GameObject ContainerObject { get; set; }//conainer objectum
        public List<DataGrid> SectorDataGrid { get; set; }//ezek referanca pontokat atralamaznak amelyeken kersztul a tenyleges gameobjectumokat manipulalhatjuk
        public int lvl { get; set; }
        public string HotKey { get; set; } = "";
        public bool IsEquipment { set; get; } = false;
        public bool IsLoot { set; get; } = false;
        public bool IsSlot { set; get; } = false;
        public bool IsRoot { set; get; } = false;
        public bool IsEquipmentRoot { set; get; } = false;

        //general variables
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Value { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public string ImgPath { get; set; }
        public float RotateDegree { get; set; } = 0f;

        public List<string> SlotUse = new List<string>();
        public void SetSlotUse()
        {
            SlotUse.OrderBy(slotname => int.Parse(Regex.Match(slotname, @"\((\d+)\)").Groups[1].Value));
            LowestSlotUseNumber = Regex.Match(SlotUse.FirstOrDefault() ?? string.Empty, @"\((\d+)\)").Success ? int.Parse(Regex.Match(SlotUse.FirstOrDefault(), @"\((\d+)\)").Groups[1].Value) : 0;
            HighestSlotUseNumber = Regex.Match(SlotUse.LastOrDefault() ?? string.Empty, @"\((\d+)\)").Success ? int.Parse(Regex.Match(SlotUse.LastOrDefault(), @"\((\d+)\)").Groups[1].Value) : 0;
        }
        public int LowestSlotUseNumber { get; private set; }
        public int HighestSlotUseNumber { get; private set; }

        //action (Műveletek)
        public bool Drop { get; set; } = false;
        public bool Remove { get; set; } = false;
        public bool Unload { get; set; } = false;
        public bool Modification { get; set; } = false;
        public bool Open { get; set; } = false;
        private void CopyProperties(Item source)
        {
            //altalanos adatok
            ItemType = source.ItemType;//ez alapján kerülhet be egy slotba ugyan is vannak pecifikus slotok melyeknek typusváltozójában benen kell, hogy legyen.
            ItemName = source.ItemName;//ez alapján hozza létre egy item saját magát
            Description = source.Description;
            Quantity = source.Quantity;
            Value = source.Value;
            SizeX = source.SizeX;
            SizeY = source.SizeY;
            ImgPath = source.ImgPath;
            //Action
            Drop = source.Drop;
            Remove = source.Remove;
            Unload = source.Unload;
            Modification = source.Modification;
            Open = source.Open;
            //tartalom
            Container = source.Container;//tartalom
            //fegyver adatok
            DefaultMagasineSize = source.DefaultMagasineSize;
            Spread = source.Spread;
            Rpm = source.Rpm;
            Recoil = source.Recoil;
            Accturacy = source.Accturacy;
            Range = source.Range;
            Ergonomy = source.Ergonomy;
            BulletType = source.BulletType;
            Accessors = source.Accessors;
            //felhasznalhato e?
            usable = source.usable;
            MaxStackSize = source.MaxStackSize;
        }
        public void SetItem(string name, int count)
        {
            Item completedItem = name switch
            {
                //backpacks
                "TestBackpack" => new TestBackpack().Set(),

                //vests
                "TestVest" => new TestVest().Set(),

                //armors
                "TestArmor" => new TestArmor().Set(),

                //helmets
                "TestHelmet" => new TestHelmet().Set(),

                //gloves
                "TestFingers" => new TestFingers().Set(),

                //boots
                "TestBoots" => new TestBoots().Set(),

                //masks
                "TestMask" => new TestMask().Set(),

                //headsets
                "TestHeadset" => new TestHeadset().Set(),

                //torso
                "TestSkin" => new TestSkin().Set(),

                //Pants
                "TestPant" => new TestPant().Set(),

                //Main Weapons
                "TestWeapon" => new TestWeapon().Set(),
                "AK103" => new AK103().Set(),

                //secondary weapons
                "TestHandgun" => new TestHandgun().Set(),

                //Melees
                "TestMelee" => new TestMelee().Set(),

                //Ammunitons
                "7.62x39FMJ" => new Ammunition762x39FMJ().Set(),

                //money
                "Dollar_1" => new Dollar_1().Set(),

                _ => throw new ArgumentException($"Invalid type {name}")
            };
            completedItem.Quantity = count;
            CopyProperties(completedItem);
            Debug.Log($"Item created {completedItem.ItemName}");
        }
        public Item()//ha contume itememt akarunk letrehozni mint pl: egy Root item
        {

        }
        public Item(string name, int count = 1)// egy itemet mindeg név alapjan peldanyositunk
        {
            SetItem(name, count);
        }
    }
    public abstract class NonGeneralItemProperties// az item hozza letre azt a panelt a slot inventoryban amely a tartlomert felelos, de csak akkor ha ő equipment slotban van egybekent egy up relativ pozitcioju panelt hozzon letre mint az EFT-ban
    {
        //contain
        public Container Container { get; set; }
        //weapon
        public int? DefaultMagasineSize { get; set; }
        public double? Spread { get; set; }
        public int? Rpm { get; set; }
        public double? Recoil { get; set; }
        public double? Accturacy { get; set; }
        public double? Range { get; set; }
        public double? Ergonomy { get; set; }
        public BulletType BulletType { get; set; }
        public Accessors Accessors { get; set; }
        //usable
        public bool usable { get; set; } = false;
        public int MaxStackSize { get; set; } = 1;
        //ammo

        //med

        //armor
    }
    public class Container
    {
        //egy container az itemjéhez tartozik.
        //az item constructor selekciójánál itemet peldanyositunk: pl: TestWeapon
        //ebben az eddig null érékű container változó egy ures containerrre változik
        //az item pédányosításánál igy egy új példány készül a containerből is mely alapvetően tartalmazza a container PrefabPath-ét
        //a kostructora az igy megkapott prefabPath-ből lekerdezi a Sectorokat
        public List<Item> Items { get; set; }
        public string PrefabPath;
        public ItemSlotData[][,] Sectors { get; set; }
        public Container(string prefabPath)
        {
            PrefabPath = prefabPath;
            Items = new List<Item>();
            List<DataGrid> DataGrid = Resources.Load(prefabPath).GetComponent<ContainerObject>().Sectors;
            Sectors = new ItemSlotData[DataGrid.Count][,];
            for (int sector = 0; sector < Sectors.Length; sector++)
            {
                int index = 0;
                Sectors[sector] = new ItemSlotData[DataGrid[sector].columnNumber, DataGrid[sector].rowNumber];
                for (int col = 0; col < Sectors[sector].GetLength(0); col++)
                {
                    for (int row = 0; row < Sectors[sector].GetLength(1); row++)
                    {
                        Sectors[sector][col, row] = new ItemSlotData(DataGrid[sector].col[col].row[row].GetComponent<ItemSlot>().name, DataGrid[sector].col[col].row[row].GetComponent<ItemSlot>().SlotType);
                        index++;
                    }
                }
            }
        }
    }
    public class BulletType
    {

    }

    public class Accessors
    {

    }
    public static class LootRandomizer
    {
        private static readonly List<LootItem> Weapons = new List<LootItem>
        {
            new LootItem("TestHandgun",5f),
            new LootItem("TestWeapon",3f),
            new LootItem("AK103", 1f),
            new LootItem("TestMelee",2f),
            new LootItem("7.62x39FMJ",2f,0.1f,0.5f)//jelentese, hogy 10% és 50% staksize ban spawnolhat.
        };
        private struct LootItem
        {
            public string Name;
            public float SpawnRate;
            public float MinStack;
            public float MaxStack;

            public LootItem(string Name, float SpawnRate = 0,float MinStack = 1f,float MaxStack = 1f)
            {
                this.Name = Name;
                this.SpawnRate = SpawnRate;
                this.MinStack = MinStack;
                this.MaxStack = MaxStack;
            }
        }
        private static List<LootItem> GenerateLoot(string PaletteName)
        {
            List<LootItem> list = new List<LootItem>();
            switch (PaletteName)
            {
                case "weapons":

                    foreach (LootItem item in Weapons)
                    {
                        for (int i = 0; i < item.SpawnRate; i++)
                        {
                            list.Add(item);
                        }
                    }
                    return list;
                default:
                    return list;
            }
        }
        public static void FillSimpleInvenotry(SimpleInventory simpleInventory,string PaletteName,float Fullness)
        {
            float MaxSlotNumber = 0;
            foreach (ItemSlotData[,] row in simpleInventory.Root.Container.Sectors)
            {
                foreach (ItemSlotData slot in row)
                {
                    MaxSlotNumber++;
                }
            }
            float ActualSlotNumber = 0;
            Math.Round(MaxSlotNumber*=Fullness,0);
            List<LootItem> WeightedList = GenerateLoot(PaletteName);
            while (MaxSlotNumber > ActualSlotNumber)
            {
                LootItem LootItem = WeightedList[UnityEngine.Random.Range(0, WeightedList.Count)];
                Item item = new Item(LootItem.Name);
                if (item.MaxStackSize>1)
                {
                    item = new Item(LootItem.Name,UnityEngine.Random.Range(Mathf.RoundToInt(item.MaxStackSize*LootItem.MinStack), Mathf.RoundToInt(item.MaxStackSize * LootItem.MaxStack)));
                }
                ActualSlotNumber += item.SizeX*item.SizeY;
                simpleInventory.InventoryAdd(item);
            }
        }
    }

    public static class InventorySystem
    {
        public static void DataRemoveFromLevelManager(Item Data)
        {
            InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.Items.Remove(Data);
            InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.SetMaxLVL_And_Sort();
        }
        public static void DataAddToPlayerInventory(Item Data)
        {
            InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.Items.Add(Data);
            InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.SetMaxLVL_And_Sort();
        }
        public static void SetNewDataParent(Item SetTo,Item Data)
        {
            Data.ParentItem = SetTo;
        }
        public static void SetSlotUseByPlacer(PlacerStruct Placer, Item Data)// 0.
        {
            Data.SlotUse.Clear();
            if (Placer.NewParentData.IsEquipmentRoot)
            {
                Data.IsEquipment = true;
            }
            else
            {
                Data.IsEquipment = false;
            }
            for (int i = 0; i < Placer.activeItemSlots.Count; i++)
            {
                Data.SlotUse.Add(Placer.activeItemSlots[i].name);
            }
            Data.SetSlotUse();
            if (Data.IsEquipment)
            {
                if (Data.HotKey != "")
                {
                    switch (int.Parse(Data.HotKey))
                    {
                        case 1:
                            InGameUI.SetHotKey1(null);
                            break;
                        case 2:
                            InGameUI.SetHotKey2(null);
                            break;
                        case 3:
                            InGameUI.SetHotKey3(null);
                            break;
                        case 4:
                            InGameUI.SetHotKey4(null);
                            break;
                        case 5:
                            InGameUI.SetHotKey5(null);
                            break;
                        case 6:
                            InGameUI.SetHotKey6(null);
                            break;
                        case 7:
                            InGameUI.SetHotKey7(null);
                            break;
                        case 8:
                            InGameUI.SetHotKey8(null);
                            break;
                        case 9:
                            InGameUI.SetHotKey9(null);
                            break;
                        default:
                            break;
                    }
                }
                switch (Data.LowestSlotUseNumber)
                {
                    case 10:
                        Data.HotKey = "1";
                        InGameUI.SetHotKey1(Data);
                        break;
                    case 11:
                        Data.HotKey = "2";
                        InGameUI.SetHotKey2(Data);
                        break;
                    case 12:
                        Data.HotKey = "3";
                        InGameUI.SetHotKey3(Data);
                        break;
                    case 13:
                        Data.HotKey = "4";
                        InGameUI.SetHotKey4(Data);
                        break;
                    default:
                        break;
                }
            }
        }
        public static void SetSlotUseBySector(int Y,int X,int sectorIndex, Item AddTo,Item Data)
        {
            Data.SlotUse.Clear();
            if (AddTo.IsEquipmentRoot)
            {
                Data.SlotUse.Add(AddTo.Container.Sectors[sectorIndex][Y, X].SlotName);//ez alapjan azonositunk egy itemslotot
                Data.IsEquipment = true;
            }
            else
            {
                Data.IsEquipment = false;
                for (int y = Y; y < Y + Data.SizeY; y++)
                {
                    for (int x = X; x < X + Data.SizeX; x++)
                    {
                        Data.SlotUse.Add(AddTo.Container.Sectors[sectorIndex][y, x].SlotName);//ez alapjan azonositunk egy itemslotot
                    }
                }
            }
            Data.SetSlotUse();
            if (Data.IsEquipment)
            {
                if (Data.HotKey != "")
                {
                    switch (int.Parse(Data.HotKey))
                    {
                        case 1:
                            InGameUI.SetHotKey1(null);
                            break;
                        case 2:
                            InGameUI.SetHotKey2(null);
                            break;
                        case 3:
                            InGameUI.SetHotKey3(null);
                            break;
                        case 4:
                            InGameUI.SetHotKey4(null);
                            break;
                        case 5:
                            InGameUI.SetHotKey5(null);
                            break;
                        case 6:
                            InGameUI.SetHotKey6(null);
                            break;
                        case 7:
                            InGameUI.SetHotKey7(null);
                            break;
                        case 8:
                            InGameUI.SetHotKey8(null);
                            break;
                        case 9:
                            InGameUI.SetHotKey9(null);
                            break;
                        default:
                            break;
                    }
                }
                switch (Data.LowestSlotUseNumber)
                {
                    case 10:
                        Data.HotKey = "1";
                        InGameUI.SetHotKey1(Data);
                        break;
                    case 11:
                        Data.HotKey = "2";
                        InGameUI.SetHotKey2(Data);
                        break;
                    case 12:
                        Data.HotKey = "3";
                        InGameUI.SetHotKey3(Data);
                        break;
                    case 13:
                        Data.HotKey = "4";
                        InGameUI.SetHotKey4(Data);
                        break;
                    default:
                        break;
                }
            }
        }
        public static void DataAdd(Item AddTo, Item Data)// 1.
        {
            foreach (ItemSlotData[,] sector in AddTo.Container.Sectors)
            {
                foreach (ItemSlotData slot in sector)
                {
                    if (Data.SlotUse.Contains(slot.SlotName))
                    {
                        slot.PartOfItemData = Data;
                    }
                }
            }
            AddTo.Container.Items.Add(Data);
            int lvl = AddTo.lvl;
            Data.lvl = ++lvl;
            if (Data.SelfGameobject)
            {
                foreach (DataGrid dataGrid in AddTo.SectorDataGrid)
                {
                    foreach (RowData rowData in dataGrid.col)
                    {
                        foreach (GameObject slot in rowData.row)
                        {
                            if (Data.SlotUse.Contains(slot.name))
                            {
                                slot.GetComponent<ItemSlot>().PartOfItemObject = Data.SelfGameobject;
                            }
                        }
                    }
                }
            }
        }
        public static void DataRemove(Item RemoveFrom, Item Data)// 1.
        {
            foreach (ItemSlotData[,] sector in RemoveFrom.Container.Sectors)
            {
                foreach (ItemSlotData slot in sector)
                {
                    if (Data.SlotUse.Contains(slot.SlotName))
                    {
                        slot.PartOfItemData = null;
                    }
                }
            }
            RemoveFrom.Container.Items.Remove(Data);
            if (Data.SelfGameobject)
            {
                foreach (DataGrid dataGrid in RemoveFrom.SectorDataGrid)
                {
                    foreach (RowData rowData in dataGrid.col)
                    {
                        foreach (GameObject slot in rowData.row)
                        {
                            if (Data.SlotUse.Contains(slot.name))
                            {
                                slot.GetComponent<ItemSlot>().PartOfItemObject = null;
                            }
                        }
                    }
                }
            }
            if (Data.IsEquipment)
            {
                switch (Data.LowestSlotUseNumber)
                {
                    case 10:
                        Data.HotKey = "";
                        InGameUI.SetHotKey1(null);
                        break;
                    case 11:
                        Data.HotKey = "";
                        InGameUI.SetHotKey2(null);
                        break;
                    case 12:
                        Data.HotKey = "";
                        InGameUI.SetHotKey3(null);
                        break;
                    case 13:
                        Data.HotKey = "";
                        InGameUI.SetHotKey4(null);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}