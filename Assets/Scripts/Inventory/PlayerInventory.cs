using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine.UI;
using Assets.Scripts;
using System;
using System.Reflection;
using System.Linq;
using System.Drawing;
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

namespace PlayerInventoryClass
{
    public class PlayerInventory : MonoBehaviour
    {
        [HideInInspector] public static GameObject EquipmentsPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        [HideInInspector] public static GameObject SlotPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        [HideInInspector] public static GameObject LootPanelObject;//az inventory 3 alsóbrendűbb objektumának egyike
        [HideInInspector] public static GameObject InventoryObjectRef;

        [HideInInspector] public PlayerInventory playerInventoryData;//a player mindenhol elerheto inventoryja ez a gyökér mely referencia ként össze van kötve a playerInventoryVisual.cs - vel
                                                                     //a playerinventoryVisual equipmentenként osztja szét a root adatokat, így ezen adat az equipmetekig tart, utána az itemek egymás shyncronizálásáért felelnek 

        [HideInInspector] public Equipmnets equipments;//ez lényegében az inventory class adatai mivel a playerInventory többi része ezen változó kiszolgálásáért, mogosításáért felel vagy adatelérési antennaként (playerInventoryData) fumcionál

        [HideInInspector] public GameObject LootableObject;//ezt kapja mint adat
        private GameObject LootContainer;//ezt készíti el az adatokból mint objectum

        public class Equipmnets
        {
            public List<EquipmnetStruct> equipmentList;
            public Equipmnets()//az equipmnets változó adatait itt példányosítjuk. az összes equipmentet alapbeallitottsággal példányosítjuk
            {
                //1.az equpmentek adatlistáját pédányosítjuk
                equipmentList = new List<EquipmnetStruct>();
                PanelEquipments equipmentPanel = Resources.Load<GameObject>("GameElements/Equipment-Inventory").GetComponent<PanelEquipments>();
                for (int i = 0; i < equipmentPanel.EquipmentsSlots.Length; i++)
                {
                    //2. az equipment adatlistát feltöltjük az erőre megalkotot prefab adataival, továbbá létrehoz egy ures item példányt
                    equipmentList.Add(new EquipmnetStruct(equipmentPanel.EquipmentsSlots[i].GetComponent<EquipmentSlot>().SlotName, equipmentPanel.EquipmentsSlots[i].GetComponent<EquipmentSlot>().SlotType, null));
                }
            }
        }
        public class EquipmnetStruct
        {
            public string EquipmentSlotName;
            public string EquipmnetSlotType;
            public Item EquipmentItem;
            public EquipmnetStruct(string name, string type, Item item)
            {
                this.EquipmentSlotName = name;
                this.EquipmnetSlotType = type;
                this.EquipmentItem = item;
            }
        }
        public void EquipmentRefresh(EquipmnetStruct equipmnet)
        {
            foreach (EquipmnetStruct equipmnetStruct in equipments.equipmentList)
            {
                if (equipmnetStruct.EquipmentSlotName == equipmnet.EquipmentSlotName)
                {
                    equipmnetStruct.EquipmentItem = equipmnet.EquipmentItem;
                }
            }
        }
        private void Awake()//----------------------------------------------------------- ELSO LEPES AZ INVENTORY MEGALKOTASAKOR ---------------------------------------------------------------------------
        {
            InventoryObjectRef = gameObject;
            //0. egy uj pedlanyt hozunk letre az equipmnetkenek
            equipments = new Equipmnets();
            //3. ha van inventory mentés akkor betoltjük azt
            InventoryLoad();
        }

        public void InventorySave()
        {

        }
        public void InventoryLoad()//kelelne egy save manager script ami a be ovasat es a kiirast kezelni ezzel lehet idot lehetni sporolni
        {
            if (File.Exists("UserSave.json"))
            {
                string jsonString = File.ReadAllText("PlayerSave.json");
                equipments = JsonConvert.DeserializeObject<Equipmnets>(jsonString);
            }
            //4. ha nincs elmentett inventory akkor az eddig letrehozott ures inventory adatát a fő publikus playerInventoryData változó megkapja
            InventoryUpdate();
        }
        private void InventoryUpdate()//??? szügségtelennek tünik mégis mindenhonnani törlése problémát okoz.
        {
            playerInventoryData = this;
        }
        public void InventoryAdd(Item item)
        {
            //item.SetItem(item.ItemName);
            Debug.Log($"Add item: {item.ItemName}   in progress");
            bool ItemAdded = false;
            for (int i = 0; i < equipments.equipmentList.Count; i++)//equipment
            {
                //Debug.Log($"Adding into equipmnets... :    {equipments.equipmentList[i].EquipmentSlotName}   tipus:{equipments.equipmentList[i].EquipmnetSlotType} ItemType:{item.ItemType}   slot tartalma: {(equipments.equipmentList[i].EquipmentItem == null? "null": equipments.equipmentList[i].EquipmentItem)}");
                if (equipments.equipmentList[i].EquipmnetSlotType.Contains(item.ItemType) && equipments.equipmentList[i].EquipmentItem == null)
                {
                    item.SlotUse = new string[] { equipments.equipmentList[i].EquipmentSlotName };
                    equipments.equipmentList[i].EquipmentItem = item;
                    ItemAdded = true;
                    InventoryUpdate();
                    Debug.Log($"item: {item.ItemName} added in equipment: {equipments.equipmentList[i].EquipmentSlotName}");
                    break;
                }
            }
            if (!ItemAdded)//container gyorsitott item hozzadas mely nem ad uj elemet hanem csak quanity-t novel
            {
                for (int i = 0; i < equipments.equipmentList.Count && !ItemAdded; i++)//equipment
                {
                    if (equipments.equipmentList[i].EquipmentItem != null && equipments.equipmentList[i].EquipmentItem.Container != null)
                    {
                        for (int j = 0; j < equipments.equipmentList[i].EquipmentItem.Container.Items.Count && !ItemAdded; j++)
                        {
                            if (equipments.equipmentList[i].EquipmentItem.Container.Items[j].ItemName == item.ItemName && equipments.equipmentList[i].EquipmentItem.Container.Items[j].Quantity != equipments.equipmentList[i].EquipmentItem.Container.Items[j].MaxStackSize)
                            {
                                int originalCount = equipments.equipmentList[i].EquipmentItem.Container.Items[j].Quantity;
                                equipments.equipmentList[i].EquipmentItem.Container.Items[j].Quantity += item.Quantity;
                                if (equipments.equipmentList[i].EquipmentItem.Container.Items[j].Quantity > equipments.equipmentList[i].EquipmentItem.Container.Items[j].MaxStackSize)
                                {
                                    item.Quantity -= (equipments.equipmentList[i].EquipmentItem.Container.Items[j].MaxStackSize - originalCount);
                                    equipments.equipmentList[i].EquipmentItem.Container.Items[j].Quantity = equipments.equipmentList[i].EquipmentItem.Container.Items[j].MaxStackSize;
                                }
                                else
                                {
                                    ItemAdded = true;
                                    Debug.Log($"item: {item.ItemName} added in container with fast placing into --> {equipments.equipmentList[i].EquipmentItem.ItemName}");
                                    InventoryUpdate();
                                }
                            }
                        }
                    }

                }
            }
            if (!ItemAdded)//container
            {
                for (int equpmentIndex = 0; equpmentIndex < equipments.equipmentList.Count && !ItemAdded; equpmentIndex++)//vegig iterálunk az osszes equipmenten
                {
                    if (equipments.equipmentList[equpmentIndex].EquipmentItem != null && equipments.equipmentList[equpmentIndex].EquipmentItem.Container != null)//ha az equipmnetnek nincs containerje akkor kihadjuk
                    {
                        if (equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors == null) { Debug.LogError($"EquipmnetItem.Container.Sectrors == null"); };
                        for (int sectorIndex = 0; sectorIndex < equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors.Length && !ItemAdded; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                        {
                            if (equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex].GetLength(1) >= item.SizeX && equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex].GetLength(0) >= item.SizeY)//egy gyors ellenörzést végzünk, hogy az itemunk a feltetelezett teljesen ures sectorba belefér e, ha nem kihadjuk
                            {
                                for (int Y = 0; Y < equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex].GetLength(0) && !ItemAdded; Y++)//vegig iterálunk a sorokon
                                {
                                    for (int X = 0; X < equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex].GetLength(1) && !ItemAdded; X++)//a sorokon belul az oszlopokon
                                    {
                                        if (equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Y, X].PartOfItemData == null && CanBePlace(equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex], Y, X, item))//ha a slot nem tagja egy itemnek sem akkor target
                                        {
                                            Debug.Log($"AZ item elfér {equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Y, X].SlotName} --> kiindulasi pont:");
                                            int index = 0;
                                            item.SlotUse = new string[item.SizeX * item.SizeY];
                                            List<ItemSlotData> itemSlots = new List<ItemSlotData>();
                                            for (int y = Y; y < Y + item.SizeY; y++)
                                            {
                                                for (int x = X; x < X + item.SizeX; x++)
                                                {
                                                    Debug.Log($"Y:{y}/{Y + item.SizeY} X:{x}/{X + item.SizeX}  {equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][y, x].SlotName} part of item: {equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][y, x].PartOfItemData != null}");
                                                    itemSlots.Add(equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][y, x]);
                                                    item.SlotUse[index] = equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][y, x].SlotName;//ez alapjan azonositunk egy itemslotot
                                                    index++;
                                                }
                                            }
                                            int count = 0;
                                            if (item.Quantity > item.MaxStackSize)
                                            {
                                                count = item.Quantity - item.MaxStackSize;
                                                item.Quantity = item.MaxStackSize;
                                            }
                                            else
                                            {
                                                ItemAdded = true;
                                            }
                                            item.SetSlotUseId();
                                            foreach (ItemSlotData itemSlot in equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex])
                                            {
                                                if (itemSlots.Exists(slot => slot.SlotName == itemSlot.SlotName))
                                                {
                                                    itemSlot.PartOfItemData = item;
                                                }
                                            }
                                            equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Items.Add(item);
                                            InventoryUpdate();
                                            Debug.Log($"Item Added in container");
                                            item = new Item(item.ItemName, count);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!ItemAdded)
                {
                    Debug.LogWarning($"item: {item.ItemName} cannot added, probably no space for that");
                }
            }
            InventoryUpdate();
        }
        public void InventoryRemove(Item item)//pocitciot és rogatast figyelmen kivul hagy     ,    csak 1 db tavolit el
        {
            //item.SetItem(item.ItemName);
            Debug.Log($"Remove: {item.ItemName}  1db  in progress");
            bool ItemRemoved = false;
            if (!ItemRemoved)//container
            {
                for (int equpmentIndex = 0; equpmentIndex < equipments.equipmentList.Count && !ItemRemoved; equpmentIndex++)//vegig iterálunk az osszes equipmenten
                {
                    if (equipments.equipmentList[equpmentIndex].EquipmentItem != null && equipments.equipmentList[equpmentIndex].EquipmentItem.Container != null)//ha az equipmnetnek nincs containerje akkor kihadjuk
                    {
                        for (int sectorIndex = 0; sectorIndex < equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors.Length && !ItemRemoved; sectorIndex++)
                        {
                            for (int Row = 0; Row < equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex].GetLength(0) && !ItemRemoved; Row++)
                            {
                                for (int Col = 0; Col < equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex].GetLength(1) && !ItemRemoved; Col++)
                                {
                                    if (equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Row, Col].PartOfItemData != null && equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.ItemName == item.ItemName)
                                    {
                                        equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity -= item.Quantity;
                                        int count = 0;
                                        if (equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity > 0)
                                        {
                                            ItemRemoved = true;//csak menyiseget törlünk
                                        }
                                        else if (equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity == 0)
                                        {
                                            ItemRemoved = true;
                                            equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Items.RemoveAt(equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Items.FindIndex(item => item.SlotUse.Contains(equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Row, Col].SlotName)));

                                            Item RefItem = equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Row, Col].PartOfItemData;

                                            foreach (ItemSlotData itemSlot in equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex])
                                            {
                                                if (RefItem.GetSlotUseId().Contains(itemSlot.SlotName))
                                                {
                                                    itemSlot.PartOfItemData = null;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            count = Math.Abs(equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity);
                                            item.Quantity = count;
                                            equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Items.RemoveAt(equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Items.FindIndex(item => item.SlotUse.Contains(equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Row, Col].SlotName)));

                                            Item RefItem = equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex][Row, Col].PartOfItemData;

                                            foreach (ItemSlotData itemSlot in equipments.equipmentList[equpmentIndex].EquipmentItem.Container.Sectors[sectorIndex])
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
                    }
                }
                if (!ItemRemoved)
                {
                    Debug.LogWarning($"item: {item.ItemName} cannot removed, probably the item doesn't exist");
                }

            }
            if (!ItemRemoved)
            {
                for (int i = 0; i < equipments.equipmentList.Count && !ItemRemoved; i++)//equipment
                {
                    if (equipments.equipmentList[i].EquipmentItem != null && equipments.equipmentList[i].EquipmentItem.ItemName == item.ItemName)
                    {
                        equipments.equipmentList[i].EquipmentItem = null;
                        ItemRemoved = true;
                        InventoryUpdate();
                        Debug.Log($"item: {item.ItemName} removed from equipment: {equipments.equipmentList[i].EquipmentSlotName}");
                        break;
                    }
                }
            }
            InventoryUpdate();
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
            PanelEquipments panelEquipments = EquipmentsPanelObject.GetComponent<PanelEquipments>();

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

            for (int i = 0; i < panelEquipments.EquipmentsSlots.Length; i++)
            {
                //--> EquipmnetSlot.cs
                if (playerInventoryData.equipments.equipmentList[i].EquipmentItem != null)
                {
                    Debug.Log($"({playerInventoryData.equipments.equipmentList[i].EquipmentSlotName})    PlayerInventory.cs ------- SetDataRoute --------> EquipmentSlot.cs  ({panelEquipments.EquipmentsSlots[i].name})         RootItem:{playerInventoryData.equipments.equipmentList[i].EquipmentItem.ItemName}  ");
                }
                panelEquipments.EquipmentsSlots[i].GetComponent<EquipmentSlot>().SetRootDataRoute(playerInventoryData.equipments.equipmentList[i], gameObject);
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
    }
}



namespace ItemHandler
{
    public class ItemSlotData
    {
        public string SlotName;
        public string SlotType;
        public Item PartOfItemData;
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
            string id = "";
            for (int i = 0; i < SlotUse.Length; i++)
            {
                id += SlotUse[i];
            }
            SlotUseId = id;
            Debug.Log($"Set {ItemName} : slotuse id = {SlotUseId}");
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
            GameObject[] sectors = Resources.Load(prefabPath).GetComponent<ContainerObject>().SectorManagers;
            Sectors = new ItemSlotData[sectors.Length][,];
            for (int i = 0; i < Sectors.Length; i++)
            {
                int index = 0;
                Sectors[i] = new ItemSlotData[sectors[i].GetComponent<SectorManager>().row, sectors[i].GetComponent<SectorManager>().columb];
                for (int row = 0; row < Sectors[i].GetLength(0); row++)
                {
                    for (int col = 0; col < Sectors[i].GetLength(1); col++)
                    {
                        Sectors[i][row, col] = new ItemSlotData();
                        Sectors[i][row, col].SlotType = sectors[i].GetComponent<SectorManager>().ItemSlots[index].GetComponent<ItemSlot>().SlotType;
                        Sectors[i][row, col].SlotName = sectors[i].GetComponent<SectorManager>().ItemSlots[index].GetComponent<ItemSlot>().name;
                        Sectors[i][row, col].PartOfItemData = null;
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