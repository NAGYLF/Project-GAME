using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Assets.Scripts;
using System;
using System.Linq;
using MainData;
using static MainData.SupportScripts;
using ItemHandler;
using Weapons;
using Backpacks;
using Vests;
using Armors;
using Helmets;
using Fingers;
using Boots;
using Masks;
using Headsets;
using Skins;
using Pants;
using Melees;
using Ammunition;
using Assets.Scripts.Inventory;
using NaturalInventorys;
using static ItemObject;
using System.Text.RegularExpressions;

namespace PlayerInventoryClass
{
    public class PlayerInventory : MonoBehaviour
    {
        #region Personal variables
        [HideInInspector] public static GameObject EquipmentsPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        [HideInInspector] public static GameObject SlotPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        [HideInInspector] public static GameObject LootPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        [HideInInspector] public static GameObject InventoryObjectRef;
        public List<GameObject> EquipmentSlots;//ez egy mátrix lista amely tartalmazza az összes itemSlot Objectumot

        [HideInInspector] public List<ItemSlotData> equipments;//ez lényegében az inventory class adatai mivel a playerInventory többi része ezen változó kiszolgálásáért, mogosításáért felel vagy adatelérési antennaként (playerInventoryData) fumcionál

        [HideInInspector] public GameObject LootableObject;//ezt kapja mint adat
        private GameObject LootContainer;//ezt készíti el az adatokból mint objectum
        #endregion

        #region Active Slot Handler variables
        //Ezen változók szükségesek ahoz, hogy egy itemet helyezni tudjunk slotokból slotokba
        [HideInInspector] public List<GameObject> activeSlots;
        [HideInInspector] public GameObject PlaceableObject;
        private int activeSlotsCount = 0;
        private PlacerStruct placer;
        #endregion

        #region Active Slot Handler
        //Ezen eljárások szükségesek ahoz, hogy egy itemet helyezni tudjunk slotokból slotokba
        private IEnumerator Targeting()
        {
            if (activeSlots.Count > 0)
            {
                PlaceableObject = activeSlots.First().GetComponent<ItemSlot>().ActualPartOfItemObject;
                placer.activeItemSlots = activeSlots;
                placer.NewVirtualParentObject = gameObject;
                PlaceableObject.GetComponent<ItemObject>().placer = placer;
                yield return null;
            }
            activeSlotsCount = activeSlots.Count;
        }
        private void Update()
        {
            if (activeSlots.Count != activeSlotsCount)
            {
                StartCoroutine(Targeting());
            }
        }
        #endregion
        private void Awake()
        {
            activeSlots = new List<GameObject>();
            placer.activeItemSlots = new List<GameObject>();

            InventoryObjectRef = gameObject;

            equipments = new List<ItemSlotData>();

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
                List<GameObject> list = Resources.Load("GameElements/Equipment-Inventory").GetComponent<PanelEquipments>().EquipmentsSlots.ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    equipments.Add(new ItemSlotData(list[i].name, list[i].GetComponent<ItemSlot>().SlotType));
                }
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
        private bool AddingByCount(List<Item> DataList, Item Data)
        {
            bool ItemAdded = false;
            for (int itemIndex = 0; itemIndex < DataList.Count && !ItemAdded; itemIndex++)
            {
                if (DataList[itemIndex].ItemName == Data.ItemName && DataList[itemIndex].Quantity != DataList[itemIndex].MaxStackSize)
                {
                    int originalCount = DataList[itemIndex].Quantity;
                    DataList[itemIndex].Quantity += Data.Quantity;
                    if (DataList[itemIndex].Quantity > DataList[itemIndex].MaxStackSize)
                    {
                        Data.Quantity -= (DataList[itemIndex].MaxStackSize - originalCount);
                        DataList[itemIndex].Quantity = DataList[itemIndex].MaxStackSize;
                    }
                    else
                    {
                        ItemAdded = true;
                    }
                }
            }
            if (!ItemAdded)
            {
                for (int itemIndex = 0; itemIndex < DataList.Count; itemIndex++)
                {
                    if (DataList[itemIndex].Container != null)
                    {
                        ItemAdded = AddingByCount(DataList[itemIndex].Container.Items, Data);
                    }
                }
            }
            return ItemAdded;
        }
        private bool AddingByNewItem(List<Container> ContainerList, Item Data)
        {
            bool ItemAdded = false;
            for (int ContainerIndex = 0; ContainerIndex < ContainerList.Count && !ItemAdded; ContainerIndex++)
            {
                for (int sectorIndex = 0; sectorIndex < ContainerList[ContainerIndex].Sectors.Length && !ItemAdded; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                {
                    if (ContainerList[ContainerIndex].Sectors[sectorIndex].GetLength(1) >= Data.SizeX && ContainerList[ContainerIndex].Sectors[sectorIndex].GetLength(0) >= Data.SizeY)//egy gyors ellenörzést végzünk, hogy az itemunk a feltetelezett teljesen ures sectorba belefér e, ha nem kihadjuk
                    {
                        for (int Y = 0; Y < ContainerList[ContainerIndex].Sectors[sectorIndex].GetLength(0) && !ItemAdded; Y++)//vegig iterálunk a sorokon
                        {
                            for (int X = 0; X < ContainerList[ContainerIndex].Sectors[sectorIndex].GetLength(1) && !ItemAdded; X++)//a sorokon belul az oszlopokon
                            {
                                if (ContainerList[ContainerIndex].Sectors[sectorIndex][Y, X].PartOfItemData == null && CanBePlace(ContainerList[ContainerIndex].Sectors[sectorIndex], Y, X, Data))//ha a slot nem tagja egy itemnek sem akkor target
                                {
                                    int index = 0;
                                    Data.SlotUse = new string[Data.SizeX * Data.SizeY];
                                    List<ItemSlotData> itemSlots = new();
                                    for (int y = Y; y < Y + Data.SizeY; y++)
                                    {
                                        for (int x = X; x < X + Data.SizeX; x++)
                                        {
                                            itemSlots.Add(ContainerList[ContainerIndex].Sectors[sectorIndex][y, x]);
                                            Data.SlotUse[index] = ContainerList[ContainerIndex].Sectors[sectorIndex][y, x].SlotName;//ez alapjan azonositunk egy itemslotot
                                            index++;
                                        }
                                    }
                                    int count = 0;
                                    if (Data.Quantity > Data.MaxStackSize)
                                    {
                                        count = Data.Quantity - Data.MaxStackSize;
                                        Data.Quantity = Data.MaxStackSize;
                                    }
                                    else
                                    {
                                        ItemAdded = true;
                                    }
                                    Data.SetSlotUseId();
                                    foreach (ItemSlotData itemSlot in ContainerList[ContainerIndex].Sectors[sectorIndex])
                                    {
                                        if (itemSlots.Exists(slot => slot.SlotName == itemSlot.SlotName))
                                        {
                                            itemSlot.PartOfItemData = Data;
                                        }
                                    }
                                    ContainerList[ContainerIndex].Items.Add(Data);
                                    Debug.Log($"Item Added in container");
                                    Data = new Item(Data.ItemName, count);
                                }
                            }
                        }
                    }
                }
            }
            if (!ItemAdded)
            {
                List<Container> containers = new();
                for (int itemIndex = 0; itemIndex < ContainerList.Count; itemIndex++)
                {
                    for (int i = 0; i < ContainerList[itemIndex].Items.Count; i++)
                    {
                        if (ContainerList[itemIndex].Items[i].Container != null)
                        {
                            containers.Add(ContainerList[itemIndex].Items[i].Container);
                        }
                    }
                }
                for (int i = 0; i < containers.Count && !ItemAdded; i++)
                {
                    ItemAdded = AddingByNewItem(containers, Data);
                }
            }
            return ItemAdded;
        }
        public void InventoryAdd(Item item)//az equipmentekbe nem ad count szerint.
        {
            bool ItemAdded = false;
            for (int i = 0; i < equipments.Count; i++)//equipment
            {
                Debug.Log($"Adding into equipmnets... :    {equipments[i].SlotName}   tipus:{equipments[i].SlotType} ItemType:{item.ItemType}   slot tartalma: {(equipments[i].PartOfItemData == null? "null": equipments[i].PartOfItemData)}");
                if (equipments[i].SlotType.Contains(item.ItemType) && equipments[i].PartOfItemData == null)
                {
                    item.SlotUse = new string[] { equipments[i].SlotName };
                    equipments[i].PartOfItemData = item;
                    ItemAdded = true;
                    break;
                }
            }
            if (!ItemAdded && item.MaxStackSize > 1)//container gyorsitott item hozzadas mely nem ad uj elemet hanem csak quanity-t novel
            {
                List<Item> Items = new List<Item>();
                for (int i = 0; i < equipments.Count; i++)
                {
                    if (equipments[i].PartOfItemData != null)
                    {
                        Items.Add(equipments[i].PartOfItemData);
                    }
                }
                for (int i = 0; i < Items.Count && !ItemAdded; i++)//equipment
                {
                    ItemAdded = AddingByCount(Items, item);
                }
            }
            if (!ItemAdded)//container
            {
                List<Container> containers = new();
                for (int i = 0; i < equipments.Count; i++)
                {
                    if (equipments[i].PartOfItemData != null && equipments[i].PartOfItemData.Container != null)
                    {
                        containers.Add(equipments[i].PartOfItemData.Container);
                    }
                }
                for (int containerIndex = 0; containerIndex < containers.Count && !ItemAdded; containerIndex++)//vegig iterálunk az osszes equipmenten
                {
                    ItemAdded = AddingByNewItem(containers, item);
                }
            }
            if (!ItemAdded)
            {
                Debug.LogWarning($"item: {item.ItemName} cannot added, probably no space for that");
            }
        }
        private bool Removing(List<Container> ContainerList,Item Data)
        {
            bool ItemRemoved = false;
            for (int containerIndex = 0; containerIndex < ContainerList.Count && !ItemRemoved; containerIndex++)
            {
                for (int i = 0; i < ContainerList[containerIndex].Items.Count && !ItemRemoved; i++)
                {
                    if (ContainerList[containerIndex].Items[i].ItemName == Data.ItemName && (ContainerList[containerIndex].Items[i].Container == null || ContainerList[containerIndex].Items[i].Container.Items.Count == 0))
                    {
                        ContainerList[containerIndex].Items[i].Quantity -= Data.Quantity;
                        int count = 0;
                        if (ContainerList[containerIndex].Items[i].Quantity > 0)
                        {
                            ItemRemoved = true;//csak menyiseget törlünk
                        }
                        else if (ContainerList[containerIndex].Items[i].Quantity == 0)
                        {
                            ItemRemoved = true;
                            Item RefItem = ContainerList[containerIndex].Items[i];
                            ContainerList[containerIndex].Items.RemoveAt(ContainerList[containerIndex].Items.FindIndex(item => item.GetSlotUseId().Contains(ContainerList[containerIndex].Items[i].GetSlotUseId())));
                            foreach (ItemSlotData[,] sector in ContainerList[containerIndex].Sectors)
                            {
                                foreach (ItemSlotData itemSlot in sector)
                                {
                                    if (RefItem.GetSlotUseId().Contains(itemSlot.SlotName))
                                    {
                                        itemSlot.PartOfItemData = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            count = Math.Abs(ContainerList[containerIndex].Items[i].Quantity);
                            Data.Quantity = count;
                            Item RefItem = ContainerList[containerIndex].Items[i];
                            ContainerList[containerIndex].Items.RemoveAt(ContainerList[containerIndex].Items.FindIndex(item => item.GetSlotUseId().Contains(ContainerList[containerIndex].Items[i].GetSlotUseId())));
                            foreach (ItemSlotData[,] sector in ContainerList[containerIndex].Sectors)
                            {
                                foreach (ItemSlotData itemSlot in sector)
                                {
                                    if (RefItem.GetSlotUseId().Contains(itemSlot.SlotName))
                                    {
                                        itemSlot.PartOfItemData = null;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!ItemRemoved)
            {
                for (int i = 0; i < ContainerList.Count && !ItemRemoved; i++)
                {
                    List<Container> containers = new List<Container>();
                    for (int j = 0; j < ContainerList[i].Items.Count && !ItemRemoved; j++)
                    {
                        if (ContainerList[i].Items[j].Container != null)
                        {
                            containers.Add(ContainerList[i].Items[j].Container);
                        }
                    }
                    ItemRemoved = Removing(containers, Data);
                }
            }
            return ItemRemoved;
        }
        public void InventoryRemove(Item item)//newm torol olyan itemet melynek van item a containerében
        {
            Debug.Log($"Remove: {item.ItemName}  1db  in progress");
            bool ItemRemoved = false;
            if (!ItemRemoved)//container
            {
                List<Container> containers = new List<Container>();
                for (int i = 0; i < equipments.Count; i++)
                {
                    if (equipments[i].PartOfItemData != null && equipments[i].PartOfItemData.Container != null)
                    {
                        containers.Add(equipments[i].PartOfItemData.Container);
                    }
                }
                for (int containerIndex = 0; containerIndex < containers.Count && !ItemRemoved; containerIndex++)//vegig iterálunk az osszes equipmenten
                {
                    ItemRemoved = Removing(containers, item);
                }
                if (!ItemRemoved)
                {
                    Debug.LogWarning($"item: {item.ItemName} cannot removed, probably the item doesn't exist");
                }
            }
            if (!ItemRemoved)
            {
                for (int i = 0; i < equipments.Count && !ItemRemoved; i++)//equipment
                {
                    if (equipments[i].PartOfItemData != null && equipments[i].PartOfItemData.ItemName == item.ItemName)
                    {
                        equipments[i].PartOfItemData = null;
                        ItemRemoved = true;
                        break;
                    }
                }
            }
        }
        public void CloseInventory()
        {
            for (int i = InventoryObjectRef.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(InventoryObjectRef.transform.GetChild(i).gameObject);
            }
        }
        public void OpenInventory()
        {
            EquipmentInitialisation();
        }
        private void EquipmentInitialisation()//7. ha az inventory megnyitódik akkor az inventory adatai felépítik az inventoryt
        {
            float[] aranyok = Aranyszamitas(new float[] { 6, 5, 6 }, Main.DefaultWidth);

            EquipmentsPanelObject = CreatePrefab("GameElements/Equipment-Inventory");
            EquipmentsPanelObject.transform.SetParent(gameObject.transform);
            EquipmentsPanelObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[0], Main.DefaultHeight);
            EquipmentsPanelObject.GetComponent<RectTransform>().localPosition = new Vector3((aranyok[0] + aranyok[1] / 2) * -1, 0, 0);
            //PanelEquipments panelEquipments = EquipmentsPanelObject.GetComponent<PanelEquipments>();

            SlotPanelObject = CreatePrefab("GameElements/Slots-Inventory");
            SlotPanelObject.transform.SetParent(gameObject.transform);
            SlotPanelObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[1], Main.DefaultHeight);
            SlotPanelObject.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1] * -1 / 2, 0, 0);
            PanelSlots panelSlots = SlotPanelObject.GetComponent<PanelSlots>();

            LootPanelObject = CreatePrefab("GameElements/Loot-Inventory");
            LootPanelObject.transform.SetParent(gameObject.transform);
            LootPanelObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[2], Main.DefaultHeight);
            LootPanelObject.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1] / 2 + aranyok[2], 0, 0);
            PanelLoot panelLoot = LootPanelObject.GetComponent<PanelLoot>();

            EquipmentSlots = EquipmentsPanelObject.GetComponent<PanelEquipments>().EquipmentsSlots.ToList();

            for (int i = 0; i < EquipmentSlots.Count; i++)
            {
                EquipmentSlots[i].GetComponent<ItemSlot>().ParentObject = gameObject;
            }

            for (int slot = 0; slot < EquipmentSlots.Count; slot++)
            {
                if (equipments[slot].PartOfItemData != null)
                {
                    GameObject itemObject = CreatePrefab("GameElements/ItemObject");
                    itemObject.name = equipments[slot].PartOfItemData.ItemName;
                    itemObject.GetComponent<ItemObject>().SetDataRoute(equipments[slot].PartOfItemData, gameObject);//item adatok itemobjektumba való adatátvitele//itemobjektum létrehozása
                }
            }
        }
        public void LootCreate()
        {
            if (LootableObject != null)
            {
                if (LootableObject.GetComponent<SimpleInventory>() != null)
                {
                    Debug.Log("Player inventory ban " + LootableObject.GetComponent<Interact>().Title);
                    LootContainer = CreatePrefab(LootableObject.GetComponent<SimpleInventory>().PrefabPath);
                    LootContainer.GetComponent<ContainerObject>().SetDataRoute(LootableObject.GetComponent<SimpleInventory>().MainData, LootableObject); 
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
        public void DataOut(Item Data)
        {
            foreach (ItemSlotData equipment in equipments)
            {
                if (Data.SlotUse.Contains(equipment.SlotName))
                {
                    equipment.PartOfItemData = null;
                }
            }
        }
        public void DataUpdate(Item Data, GameObject VirtualChildObject)
        {
            foreach (ItemSlotData equipment in equipments)
            {
                if (Data.SlotUse.Contains(equipment.SlotName))
                {
                    equipment.PartOfItemData = Data;
                }
            }
        }
        public void DataIn(Item Data, GameObject VirtualChildObject)
        {
            foreach (ItemSlotData equipment in equipments)
            {
                if (Data.SlotUse.Contains(equipment.SlotName))
                {
                    equipment.PartOfItemData = Data;
                    VirtualChildObject.GetComponent<ItemObject>().SetDataRoute(Data, gameObject);
                }
            }
        }
    }
}



namespace ItemHandler
{
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
        //general
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public string ImgPath { get; set; }
        public float RotateDegree { get; set; } = 0f;
        public string[] SlotUse { get; set; }

        public int LowestSlotUseNumber;
        public int HighestSlotUseNumber;
        private string SlotUseId;//ezt az azonositot a localis containeren belul használjuk a container listában lévő item adatok megkülönbözetésére

        //action (Műveletek)
        public bool Drop { get; set; } = true;
        public bool Remove { get; set; } = true;
        public bool Unload { get; set; } = true;
        public bool Modification { get; set; } = true;
        public bool Open { get; set; } = true;
        public string GetSlotUseId()
        {
            return SlotUseId;
        }
        public void SetSlotUseId()
        {
            SlotUse = SlotUse.OrderBy(slotname => int.Parse(Regex.Match(slotname, @"\((\d+)\)").Groups[1].Value)).ToArray();
            LowestSlotUseNumber = int.Parse(Regex.Match(SlotUse.First(), @"\((\d+)\)").Groups[1].Value);
            HighestSlotUseNumber = int.Parse(Regex.Match(SlotUse.Last(), @"\((\d+)\)").Groups[1].Value);
            string id = "";
            for (int i = 0; i < SlotUse.Length; i++)
            {
                id += SlotUse[i];
            }
            SlotUseId = id;
        }
        private void CopyProperties(Item source)
        {
            //altalanos adatok
            ItemType = source.ItemType;//ez alapján kerülhet be egy slotba ugyan is vannak pecifikus slotok melyeknek typusváltozójában benen kell, hogy legyen.
            ItemName = source.ItemName;//ez alapján hozza létre egy item saját magát
            Description = source.Description;
            Quantity = source.Quantity;
            SlotUse = source.SlotUse;// ez a jelenleg elfoglalt helye, ezt a betolteskor hasznaljuk, hogy tudjuk mit hova raktunk el.
            ImgPath = source.ImgPath;
            SizeX = source.SizeX;
            SizeY = source.SizeY;
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
        public void SetItem(string name,int count)
        {
            Item completedItem = name switch
            {
                "TestWeapon" => new TestWeapon().Set(),
                "TestHandgun" => new TestHandgun().Set(),
                "AK103" => new AK103().Set(),
                "TestBackpack" => new TestBackpack().Set(),
                "TestVest" => new TestVest().Set(),
                "TestArmor" => new TestArmor().Set(),
                "TestHelmet" => new TestHelmet().Set(),
                "TestFingers" => new TestFingers().Set(),
                "TestBoots" => new TestBoots().Set(),
                "TestMask" => new TestMask().Set(),
                "TestHeadset" => new TestHeadset().Set(),
                "TestSkin" => new TestSkin().Set(),
                "TestPant" => new TestPant().Set(),
                "TestMelee" => new TestMelee().Set(),
                "7.62x39FMJ" => new Ammunition762x39FMJ().Set(),
                _ => throw new ArgumentException($"Invalid type {name}")
            };
            completedItem.Quantity = count;
            CopyProperties(completedItem);
            Debug.Log($"Item created {this}");
        }
        public Item()
        {

        }
        public Item(string name,int count = 1)// egy itemet mindeg név alapjan peldanyositunk
        {
            SetItem(name,count);
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
        public List<Item> Items {  get; set; }
        public string PrefabPath;
        public ItemSlotData[][,] Sectors { get; set; }
        public Container(string prefabPath)
        {
            PrefabPath = prefabPath;
            Items = new List<Item>();
            List<ContainerObject.DataGrid> DataGrid = Resources.Load(prefabPath).GetComponent<ContainerObject>().Sectors;
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
}