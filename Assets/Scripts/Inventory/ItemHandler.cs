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
using Meds;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using NaturalInventorys;
using PlayerInventoryClass;
using static PlayerInventoryClass.PlayerInventory;
using UI;


namespace ItemHandler
{
    public struct PlacerStruct
    {
        public List<GameObject> ActiveItemSlots { get; set; }
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
        public int Lvl { get; set; }
        public string HotKey { get; set; } = "";

        //general variables
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; } = "...";
        public int MaxStackSize { get; set; } = 1;
        public int Quantity { get; set; }
        public int Value { get; set; } = 1;
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public string ImgPath { get; set; }
        public float RotateDegree { get; set; } = 0f;
        public bool IsInPlayerInventory { get; set; } = false;// a player inventory tagja az item
        public bool IsEquipment { set; get; } = false;// az item egy equipment
        public bool IsLoot { set; get; } = false;// az item a loot conténerekben van
        public bool IsSlot { set; get; } = false;// az item a sloot konténerekben van
        public bool IsRoot { set; get; } = false;// az item egy root data
        public bool IsEquipmentRoot { set; get; } = false;// az item a player equipmentjeinek rootja ebbol csak egy lehet

        //SlotUse
        public List<string> SlotUse = new();
        public void SetSlotUse()
        {
            SlotUse.OrderBy(slotname => int.Parse(Regex.Match(slotname, @"\((\d+)\)").Groups[1].Value));
            LowestSlotUseNumber = Regex.Match(SlotUse.FirstOrDefault() ?? string.Empty, @"\((\d+)\)").Success ? int.Parse(Regex.Match(SlotUse.FirstOrDefault(), @"\((\d+)\)").Groups[1].Value) : 0;
            HighestSlotUseNumber = Regex.Match(SlotUse.LastOrDefault() ?? string.Empty, @"\((\d+)\)").Success ? int.Parse(Regex.Match(SlotUse.LastOrDefault(), @"\((\d+)\)").Groups[1].Value) : 0;
        }
        public int LowestSlotUseNumber { get; private set; }
        public int HighestSlotUseNumber { get; private set; }

        //action (Műveletek)
        public bool IsDropAble { get; set; } = false;
        public bool IsRemoveAble { get; set; } = true;
        public bool IsUnloadAble { get; set; } = false;
        public bool IsModificationAble { get; set; } = false;
        public bool IsOpenAble { get; set; } = false;
        public bool IsUsable { get; set; } = false;

        //Ez egy Totális Törlés ami azt jelenti, hogy mindenhonnan törli. Ez nem jo akkor ha valahonnan torolni akarjuk de mashol meg hozzadni
        public void Remove()
        {
            if (IsRemoveAble)
            {
                InventorySystem.DataRemove(ParentItem, this);
                if (SelfGameobject)
                {
                    SelfGameobject.GetComponent<ItemObject>().DestroyContainer();
                    GameObject.Destroy(SelfGameobject);
                }
            }
        }
        public void Use()
        {
            if (IsUsable)
            {
                UseLeft--;
                if (UseLeft == 0)
                {
                    InventorySystem.DataRemove(ParentItem, this);
                    if (SelfGameobject)
                    {
                        SelfGameobject.GetComponent<ItemObject>().DestroyContainer();
                        GameObject.Destroy(SelfGameobject);
                    }
                }
            }
        }
        public void Open()
        {

        }
        public void Modification()
        {

        }
        public void Unload()
        {

        }
        public void Drop()
        {

        }
        public Item()//ha contume itememt akarunk letrehozni mint pl: egy Root item
        {

        }
        public Item(string name, int count = 1)// egy itemet mindeg név alapjan peldanyositunk
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

                //Useables
                "AI_2" => new AI_2().Set(),

                _ => throw new ArgumentException($"Invalid type {name}")
            };
            completedItem.Quantity = count;

            //altalanos adatok
            ItemType = completedItem.ItemType;//ez alapján kerülhet be egy slotba ugyan is vannak pecifikus slotok melyeknek typusváltozójában benen kell, hogy legyen.
            ItemName = completedItem.ItemName;//ez alapján hozza létre egy item saját magát
            Description = completedItem.Description;
            Quantity = completedItem.Quantity;
            Value = completedItem.Value;
            SizeX = completedItem.SizeX;
            SizeY = completedItem.SizeY;
            ImgPath = completedItem.ImgPath;
            MaxStackSize = completedItem.MaxStackSize;
            //Action
            IsDropAble = completedItem.IsDropAble;
            IsRemoveAble = completedItem.IsRemoveAble;
            IsUnloadAble = completedItem.IsUnloadAble;
            IsModificationAble = completedItem.IsModificationAble;
            IsOpenAble = completedItem.IsOpenAble;
            IsUsable = completedItem.IsUsable;
            //tartalom
            Container = completedItem.Container;//tartalom
            //fegyver adatok
            DefaultMagasineSize = completedItem.DefaultMagasineSize;
            Spread = completedItem.Spread;
            Rpm = completedItem.Rpm;
            Recoil = completedItem.Recoil;
            Accturacy = completedItem.Accturacy;
            Range = completedItem.Range;
            Ergonomy = completedItem.Ergonomy;
            BulletType = completedItem.BulletType;
            Accessors = completedItem.Accessors;
            AmmoType = completedItem.AmmoType;
            //hasznalhato e?
            UseLeft = completedItem.UseLeft;
            Debug.Log($"Item created {completedItem.ItemName}");
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
        public string AmmoType { get; set; } = "";
        public BulletType BulletType { get; set; }
        public Accessors Accessors { get; set; }
        //usable
        public int UseLeft { get; set; } = 0;
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
        private static readonly List<LootItem> weapons = new()
        {
            new LootItem("TestHandgun",5f),
            new LootItem("TestWeapon",3f),
            new LootItem("AK103", 1f),
            new LootItem("TestMelee",2f),
            new LootItem("7.62x39FMJ",2f,0.1f,0.5f)//jelentese, hogy 10% és 50% staksize ban spawnolhat.
        };
        private static readonly List<LootItem> equipments = new()
        {
            new LootItem("TestArmor",1f),
            new LootItem("TestBackpack",2f),
            new LootItem("TestBoots", 1f),
            new LootItem("TestFingers", 1f),
            new LootItem("TestHeadset", 1f),
            new LootItem("TestHelmet", 1f),
            new LootItem("TestMask", 1f),
            new LootItem("TestPant", 1f),
            new LootItem("TestVest", 2f),                                                                                 new LootItem("TestBoots", 1f),
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
            List<LootItem> list = new();
            switch (PaletteName)
            {
                case "weapons":
                    foreach (LootItem item in weapons)
                    {
                        for (int i = 0; i < item.SpawnRate; i++)
                        {
                            list.Add(item);
                        }
                    }
                    return list;
                case "equipments":
                    foreach (LootItem item in equipments)
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
                Item item = new(LootItem.Name);
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
        //Uj Parent Itemet allit be erre akkor van szukseg ha meg akarod határozni, hogy melyik item tárolja őt
        public static void SetNewDataParent(Item SetTo,Item Data)
        {
            Data.ParentItem = SetTo;
        }

        //A placer egy Live változó ez azt jelenti, hogy csak akkor létezik ha az inventory meg van nyitva, illetve a kijelölt(Sárga színű) slotobjectumokat használja targetnek
        //feladata, hogy az itemObjectuma számára megmondja, hogy Parentobjectumán belül melyik slotokat foglalja el.
        public static void SetSlotUseByPlacer(PlacerStruct Placer, Item Data)// 0.
        {
            Data.SlotUse.Clear();
            for (int i = 0; i < Placer.ActiveItemSlots.Count; i++)
            {
                Data.SlotUse.Add(Placer.ActiveItemSlots[i].name);
            }
            Data.SetSlotUse();

            if ((Data.IsEquipment && !Placer.NewParentData.IsEquipmentRoot) || !Placer.NewParentData.IsInPlayerInventory)
            {
                UnSetHotKey(Data);
            }
            else if (Placer.NewParentData.IsEquipmentRoot)
            {
                UnSetHotKey(Data);
                AutoSetHotkey(Data);
            }

            if (Placer.NewParentData.IsEquipmentRoot)
            {
                Data.IsEquipment = true;
            }
            else
            {
                Data.IsEquipment = false;
            }
        }

        //Ez egy NonLive metodus az az csak adatok alapján dolgozik az inevntory bezárásakor.
        //mukodese: abba az itembe(AddTo) amelybe helyezni akarjuk az itemunket(Data) meghatározhatjuk, hogy melyik sectorában(sectorIndex) és, hogy melyik slot(X,Y) legyen az itemunk(Data) bal felső sarka
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

            if ((Data.IsEquipment && !AddTo.IsEquipmentRoot) || !AddTo.IsInPlayerInventory)
            {
                UnSetHotKey(Data);
            }
            else if (AddTo.IsEquipmentRoot)
            {
                UnSetHotKey(Data);
                AutoSetHotkey(Data);
            }

            if (AddTo.IsEquipmentRoot)
            {
                Data.IsEquipment = true;
            }
            else
            {
                Data.IsEquipment = false;
            }
        }

        //Hozzáad egy itemet egy másik itemhez
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
            int lvl = AddTo.Lvl;
            Data.Lvl = ++lvl;
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
            if (AddTo.IsInPlayerInventory)
            {
                Data.IsInPlayerInventory = true;
                InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.Items.Add(Data);
                InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.SetMaxLVL_And_Sort();
            }
        }

        //eltávolít egy itemet egy másik itemből
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
            if (Data.IsInPlayerInventory)
            {
                Data.IsInPlayerInventory = false;
                InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.Items.Remove(Data);
                InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.SetMaxLVL_And_Sort();
            }
        }
        public static void UnSetHotKey(Item Data)
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
                Data.HotKey = "";
            }
        }
        public static void AutoSetHotkey(Item Data)
        {
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
}