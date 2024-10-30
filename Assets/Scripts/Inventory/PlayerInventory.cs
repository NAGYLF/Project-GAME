using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Assets.Scripts;
using System;
using Weapons;
using Backpacks;
using System.Reflection;
using System.Linq;
using System.Drawing;
using MainData;
using static MainData.SupportScripts;
using PlayerInventoryVisualBuild;
using PlayerInventoryClass;



namespace PlayerInventoryClass
{
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory playerInventoryData;//a player mindenhol elerheto inventoryja ezzel tortenik meg a mentes is

        public static Equipmnets equipments;

        public class Equipmnets
        {
            public List<EquipmnetStruct> equipmnetsData;
            public Equipmnets()
            {
                equipmnetsData = new List<EquipmnetStruct>();
                Transform transform = Resources.Load<GameObject>("GameElements/Equipment-Inventory").transform;
                for (int i = 0; i < transform.childCount; i++)
                {
                    equipmnetsData.Add(new EquipmnetStruct(transform.GetChild(i).GetComponent<EquipmentSlotScript>().SlotName, transform.GetChild(i).GetComponent<EquipmentSlotScript>().SlotType,new Item()));
                }
            }
            public class EquipmnetStruct
            {
                public string EquipmentName;
                public string EquipmnetType;
                public Item EquipmnetItem;
                public EquipmnetStruct(string name, string type, Item item)
                {
                    this.EquipmentName = name;
                    this.EquipmnetType = type;
                    this.EquipmnetItem = item;
                }
            }
        }

        private void Awake()
        {
            equipments = new Equipmnets();
            InventoryLoad();
            gameObject.AddComponent<PlayerInventoryVisual>();
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
                playerInventoryData = this;
            }
        }






        public static void InventoryAdd(Item item)
        {
            item.SetItem(item.ItemName);
            Debug.Log($"Addad item: {item.ItemName}");
            bool ItemAdded = false;
            for (int i = 0; i < equipments.equipmnetsData.Count; i++)//equipment
            {
                Debug.Log($"Adding into equipmnets...    {i}   equipments.equipmnetsData[i].EquipmnetType:{equipments.equipmnetsData[i].EquipmnetType} =?= item.ItemType:{item.ItemType}   equipments.equipmnetsData[i].EquipmnetItem.ItemName:{equipments.equipmnetsData[i].EquipmnetItem.ItemName == null}");
                if (equipments.equipmnetsData[i].EquipmnetType.Contains(item.ItemType) && equipments.equipmnetsData[i].EquipmnetItem.ItemName == null)
                {
                    item.SlotUse = new string[] {equipments.equipmnetsData[i].EquipmentName};
                    equipments.equipmnetsData[i].EquipmnetItem = item;
                    ItemAdded = true;
                    Debug.Log($"item: {item.ItemName} added in equipment: {equipments.equipmnetsData[i].EquipmentName}");
                    break;
                }
            }
            if (!ItemAdded)//container
            {
                for (int i = 0; i < equipments.equipmnetsData.Count; i++)
                {
                    if (equipments.equipmnetsData[i].EquipmnetItem.Container != null)
                    {
                        for (int j = 0; j < equipments.equipmnetsData[i].EquipmnetItem.Container.Sectrors.Length; j++)
                        {
                            if (equipments.equipmnetsData[i].EquipmnetItem.Container.Sectrors[j].GetLength(0) >= item.SizeY && equipments.equipmnetsData[i].EquipmnetItem.Container.Sectrors[j].GetLength(1) >= item.SizeX)
                            {
                                List<ItemSlot> tartgetSlots = new List<ItemSlot>();
                                for (int k = 0; k < equipments.equipmnetsData[i].EquipmnetItem.Container.Sectrors[j].GetLength(0) && !ItemAdded; k++)
                                {
                                    for (int i1 = 0; i1 < equipments.equipmnetsData[i].EquipmnetItem.Container.Sectrors[j].GetLength(1) && !ItemAdded; i1++)
                                    {
                                        if (equipments.equipmnetsData[i].EquipmnetItem.Container.Sectrors[j][k,i1].PartOfItem == null)
                                        {
                                            tartgetSlots.Add(equipments.equipmnetsData[i].EquipmnetItem.Container.Sectrors[j][k, i1]);
                                            if (tartgetSlots.Count == item.SizeX*item.SizeY)
                                            {
                                                item.SlotUse = new string[tartgetSlots.Count];
                                                for (int j1 = 0; j1 < tartgetSlots.Count; j1++)
                                                {
                                                    item.SlotUse[j1] = tartgetSlots[j1].SlotName;
                                                    tartgetSlots[j1].PartOfItem = item;
                                                }
                                                ItemAdded = true;
                                            }
                                        }
                                        else
                                        {
                                            tartgetSlots.Clear();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void InventoryDelete(Item item)
        {

        }
        public void InventoryModify(Item item)
        {

        }


    };



}















namespace ItemHandler
{
    public class ItemObject : MonoBehaviour
    {
        public Item Data { get; set; }
        private void Start()
        {/*
            //Ha van tartalma akkor azt generálásra küldjük a SlotPanel-be.
            if (Data.Container != null)
            {
                GameObject ContainerObjetc = CreatePrefab(Data.Container.PrefabPath);//ezen item containeradata alapján letrehozzuk a container-t
                ContainerObjetc.GetComponent<Container>().DataSyncronisation(Data.Container.Items);//szinkronizáljuk a tartalmat
                gameObject.GetComponent<ContainerObject>().ContainerVisualisation(PlayerInventoryVisual.SlotObject.transform.GetChild(0).transform);//vizuálisan elhelyezzuk
            }*/
        }
    }

    public class Item : ItemStruct
    {
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
        }
        public void SetItem(string name)
        {
            Item completedItem = name switch
            {
                "TestWeapon" => new TestWeapon().Set(),
                "TestBackpack" => new TestBackpack().Set(),
                //"TestArmor" => new TestArmor().Set(),
                _ => throw new ArgumentException("Invalid type")
            };
            CopyProperties(completedItem);
            Debug.Log($"Item created {this}");
        }
        public Item() { }
        public Item(string name)// egy itemet mindeg név alapjan peldanyositunk
        {
            SetItem(name);
        }
    }
    public abstract class ItemStruct// az item hozza letre azt a panelt a slot inventoryban amely a tartlomert felelos, de csak akkor ha õ equipment slotban van egybekent egy up relativ pozitcioju panelt hozzon letre mint az EFT-ban
    {

        //general
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string[] SlotUse { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public string ImgPath { get; set; }
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
        //ammo

        //med

        //armor
    }

    public class ContainerObject : MonoBehaviour
    {
        public Container Data { get; set; }
        public void ContainerVisualisation(Transform ParentObject)//erre azert van szükség mivel egy item objektumra aggatásakor az item start() eljárása vizualizálja az itemet, ez a vizualizáció pedig vizualizája az item containerét is ezzel az eljárással
        {
            gameObject.transform.SetParent(ParentObject, false);
        }
    }

    public class Container
    {
            //egy container az itemjéhez tartozik.
            //az item constructor selekciójánál itemet peldanyositunk: pl: TestWeapon
            //ebben az eddig null érékû container változó egy ures containerrre változik
            //az item pédányosításánál igy egy új példány készül a containerbõl is mely alapvetõen tartalmazza a container PrefabPath-ét
            //a kostructora az igy megkapott prefabPath-bõl lekerdezi a Sectorokat
            public List<Item> Items { get; set; }
            public string PrefabPath;
            public ItemSlot[][,] Sectrors { get; set; }

            public void DataSyncronisation(List<Item> source)
            {
                Items = source;
            }
            public Container()
            {
                Sectrors = CreatePrefab(PrefabPath).GetComponent<ContainerObject>().Data.Sectrors;
            }
        }
        public class BulletType
        {

        }

        public class Accessors
        {

        }
    }










namespace PlayerInventoryVisualBuild
{
    public class PlayerInventoryVisual : MonoBehaviour
    {

        private bool InventoryOpen = false;

        private GameObject InventoryObject;//az invenory fõ objektumának tárolásáért fele

        private PlayerInventory.Equipmnets equipmnets;//a PlayerInventoryData eltarolasara szolgal

        [HideInInspector] public static GameObject EquipmentsObject;//az inventory 3 alsóbrendûbb objektumának egyike
        [HideInInspector] public static GameObject SlotObject;//az inventory 3 alsóbrendûbb objektumának egyike
        [HideInInspector] public static GameObject LootObject;//az inventory 3 alsóbrendûbb objektumának egyike
        private void Update()
        {
            OpenCloseInventory();
        }
        public void OpenCloseInventory()//ez az inventoryt epiti fel
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (InventoryOpen)
                {
                    InventoryOpen = false;
                    Destroy(InventoryObject);
                }
                else
                {
                    InventoryOpen = true;
                    EquipmentInitialisation();
                }
            }
        }
        private void EquipmentInitialisation()
        {
            equipmnets = PlayerInventory.equipments;

            GameObject UI = GameObject.FindGameObjectWithTag("InGameUI");
            InventoryObject = new GameObject("Inventory");

            if (UI != null)
            {
                InventoryObject.transform.SetParent(UI.transform, false);

                InventoryObject.AddComponent<RectTransform>().localPosition = new Vector3(0, 0, UI.transform.position.z);
            }
            else
            {
                Debug.LogError("UI nem található!");
            }
            if (equipmnets == null || equipmnets.equipmnetsData == null)
            {
                Debug.LogError("Equipmnets vagy EquipmnetsData null!");
            }

            float[] aranyok = Aranyszamitas(new float[] { 6, 5, 6 }, Main.DefaultWidth);

            EquipmentsObject = CreatePrefab("GameElements/Equipment-Inventory");
            EquipmentsObject.transform.SetParent(InventoryObject.transform);
            EquipmentsObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[0], Main.DefaultHeight);
            EquipmentsObject.GetComponent<RectTransform>().localPosition = new Vector3((aranyok[0] + aranyok[1] / 2) * -1, 0, 0);
            EquipmentsPanelScript equipmentsPanelScript = EquipmentsObject.GetComponent<EquipmentsPanelScript>();

            for (int i = 0; i < equipmentsPanelScript.EquipmentsSlots.Length; i++)
            {
                for (int j = 0; j < equipmnets.equipmnetsData.Count; j++)
                {
                    if (equipmnets.equipmnetsData[j].EquipmnetItem.ItemName != null && equipmnets.equipmnetsData[j].EquipmnetItem.SlotUse.Contains(equipmentsPanelScript.EquipmentsSlots[i].name))
                    {
                        float CoordinateY = equipmentsPanelScript.EquipmentsSlots[i].GetComponent<RectTransform>().rect.y;
                        float CoordinateX = equipmentsPanelScript.EquipmentsSlots[i].GetComponent<RectTransform>().rect.x;
                        
                        GameObject itemObject = new GameObject($"{equipmnets.equipmnetsData[j].EquipmnetItem.ItemName}");//itemobjektum létrehozása
                        itemObject.AddComponent<ItemObject>().Data = equipmnets.equipmnetsData[j].EquipmnetItem;//item adatok itemobjektumba való adatátvitele
                        equipmentsPanelScript.EquipmentsSlots[i].GetComponent<EquipmentSlotScript>().Contains = itemObject;//az itemet tartó slot megkapja tatalmát az itemobjektumot

                        itemObject.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(equipmnets.equipmnetsData[j].EquipmnetItem.ImgPath);//az itemobjektum megkapja képét
                        itemObject.AddComponent<RectTransform>();
                        itemObject.GetComponent<RectTransform>().sizeDelta = equipmentsPanelScript.EquipmentsSlots[i].GetComponent<RectTransform>().sizeDelta;//meret

                        itemObject.transform.SetParent(EquipmentsObject.transform, false);//itemObj parent set
                        // Beállítjuk az anchor presetet középre
                        itemObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f); // Bal alsó sarok
                        itemObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f); // Jobb felsõ sarok
                        itemObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f); // Középpont

                        itemObject.GetComponent<RectTransform>().transform.localPosition = new Vector3(equipmentsPanelScript.EquipmentsSlots[i].GetComponent<RectTransform>().transform.localPosition.x, equipmentsPanelScript.EquipmentsSlots[i].GetComponent<RectTransform>().transform.localPosition.y,0);

                        equipmentsPanelScript.EquipmentsSlots[i].transform.SetParent(itemObject.transform);//az itemobjketum az equipmentSlot parentobjektuma lesz
                        equipmentsPanelScript.EquipmentsSlots[i].GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
                    }
                }
            }




            SlotObject = CreatePrefab("GameElements/Slots-Inventory");
            SlotObject.transform.SetParent(InventoryObject.transform);
            SlotObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[1], Main.DefaultHeight);
            SlotObject.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1] * -1 / 2, 0, 0);
            SlotObject.AddComponent<SlotsPanelScript>();

            LootObject = CreatePrefab("GameElements/Loot-Inventory");
            LootObject.transform.SetParent(InventoryObject.transform);
            LootObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[2], Main.DefaultHeight);
            LootObject.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1] / 2 + aranyok[2], 0, 0);

        }
    };
}