using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Assets.Scripts;
using System;
using Weapons;
using Backpacks;
using System.Reflection;
using System.Linq;
using MainData;
using System.Drawing;



namespace InventoryClass
{
  
    public class Inventory : MonoBehaviour
    {
        public Equipmnet_Type_ObjSize[] EquipmentTypes_ObjSize = new Equipmnet_Type_ObjSize[]
        {
            new Equipmnet_Type_ObjSize("Vest", 1f, 1f ,1f,1f,1f),
            new Equipmnet_Type_ObjSize("Backpack", 1f, 1f ,1f,1f,1f),
            new Equipmnet_Type_ObjSize("Helmet", 1f, 1f ,1f,1f,1f),
            new Equipmnet_Type_ObjSize("Armor", 1f, 1f ,1f,1f,1f),
            new Equipmnet_Type_ObjSize("Headset", 1f, 1f ,1f,1f,1f),
            new Equipmnet_Type_ObjSize("Finger", 1f, 1f ,1f,1f,1f),
            new Equipmnet_Type_ObjSize("Mask", 1f, 1f ,1f,1f,1f),
            new Equipmnet_Type_ObjSize("Boots", 1f, 1f ,1f,1f,1f),
            new Equipmnet_Type_ObjSize("Pants", 1f, 1f ,1f,1f,1f),
            new Equipmnet_Type_ObjSize("Skin", 1f, 1f ,1f,1f,1f),
        };
        public struct Equipmnet_Type_ObjSize
        {
            public string type;
            public float sizeX;
            public float sizeY;
            public float positionX;
            public float positionY;
            public float positionZ;

            public Equipmnet_Type_ObjSize(string type, float sizeX, float sizeY,float positonX,float positonY,float positionZ)
            {
                this.type = type;
                this.sizeX = sizeX;
                this.sizeY = sizeY;
                this.positionX = positonX;
                this.positionY = positonY;
                this.positionZ = positionZ;
            }
        }

        public DefaultInvetoryStruct equipments;
        public string InventoryType;

        private bool InventoryOpen = false;

        private GameObject InventoryObject;
        private GameObject Equipments;
        private GameObject Slots;
        private GameObject Loot;
        public Item[] itemArray()
        {
            return new Item[]
            {
                equipments.VestSlot,
                equipments.BackbackSlot,
                equipments.HelmetSlot,
                equipments.ArmorSlot,
                equipments.HeadsetSlot,
                equipments.FingerSlot,
                equipments.HeadsetSlot,
                equipments.MaskSlot,
                equipments.BootsSlot,
                equipments.PantsSlot,
                equipments.SkinSlot,
            };
        }
        public struct DefaultInvetoryStruct
        {
            public Item VestSlot { get; set; }
            public Item BackbackSlot { get; set; }
            public Item HelmetSlot { get; set; }
            public Item ArmorSlot { get; set; }
            public Item PocketSlot { get; set; }
            public Item FingerSlot { get; set; }
            public Item HeadsetSlot { get; set; }
            public Item MaskSlot { get; set; }
            public Item BootsSlot { get; set; }
            public Item PantsSlot { get; set; }
            public Item SkinSlot { get; set; }
        }

        public void Start() 
        { 
            initalisation();
        }
        private void initalisation()
        {
            if (InventoryType == "Player")//player tipusu
            {
                equipments = new DefaultInvetoryStruct();
                InventoryLoad();
                InventoryEquipmentsBuld();
            }
        }
        private void InventoryEquipmentsBuld()//ez nem az inventoryban epiti fel az itememket hanem a playerre aggatja fel azokat, mint a fegyvert a kezebe adja, illetve a pancelt ra.   NOT WORKING!!!
        {
            Item[] items = itemArray();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null)
                {
                    GameObject NewGameObject = new GameObject();

                    Item item = items[i];

                    NewGameObject.AddComponent<Item>().ItemConstructorSelector(item);

                    NewGameObject.transform.SetParent(gameObject.transform);
                }


            }

        }
        public void Update()
        {
            OpenCloseInventory();
        }
        public void OpenCloseInventory()//ez az inventoryt epiti fel
        {
            if (Input.GetKeyUp(KeyCode.Tab) || Input.GetKeyUp(KeyCode.I))
            {
                if (InventoryOpen)
                {
                    InventoryOpen = false;
                    Destroy(InventoryObject);
                }
                else
                {
                    InventoryOpen= true;
                    GameObject UI = GameObject.FindGameObjectWithTag("InGameUI");
                    InventoryObject = new GameObject("Inventory");

               

                    if (UI != null)
                    {
                        InventoryObject.transform.SetParent(UI.transform,false);

                        InventoryObject.AddComponent<RectTransform>().localPosition = new Vector3(0, 0, UI.transform.position.z);

                    }
                    else
                    {
                        Debug.LogError("UI nem található!");
                    }

                    float[] aranyok = Aranyszamitas(new float[] {6,5,6},Main.DefaultWidth);

                    Equipments = CreatePrefab("GameElements/Equipment-Inventory");
                    Equipments.transform.SetParent(InventoryObject.transform);
                    Equipments.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[0], Main.DefaultHeight);
                    Equipments.GetComponent<RectTransform>().localPosition = new Vector3((aranyok[0] + aranyok[1]/2)*-1, 0, 0);

                    RectTransform EquipmentRectransform = Equipments.GetComponent<RectTransform>();

                    GameObject[] EquipmentsSlots = new GameObject[EquipmentTypes_ObjSize.Length];
                    for (int i = 0; i < EquipmentsSlots.Length; i++)
                    {
                        EquipmentsSlots[i] = CreatePrefab("GameElements/EquipmentSlot");
                        EquipmentsSlots[i].name = $"{EquipmentTypes_ObjSize[i].type}Slot";
                        if (itemArray()[i] != null){
                            EquipmentsSlots[i].AddComponent<Item>().ItemConstructorSelector(itemArray()[i]);
                        }
                        EquipmentsSlots[i].transform.SetParent(EquipmentRectransform);
                        EquipmentsSlots[i].GetComponent<RectTransform>().sizeDelta = new Vector2(EquipmentTypes_ObjSize[i].sizeX, EquipmentTypes_ObjSize[i].sizeY);//nem biztos hogy mukodik
                        EquipmentsSlots[i].GetComponent<RectTransform>().localPosition = new Vector3(EquipmentTypes_ObjSize[i].positionX, EquipmentTypes_ObjSize[i].positionY, EquipmentTypes_ObjSize[i].positionZ);//nics kesz
                    }
                    //feladat: mindegyik slotot el kell megfeleloen rendezni, ehez algoritmust kell irni mely kiszamitja és azokat megfelelo helyre rakja. lehetoleg visszatero erteke egy float[] legyen!



                    Slots = CreatePrefab("GameElements/Slots-Inventory");
                    Slots.transform.SetParent(InventoryObject.transform);
                    Slots.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[1], Main.DefaultHeight);
                    Slots.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1]*-1 / 2, 0, 0);

                    Loot = CreatePrefab("GameElements/Loot-Inventory");
                    Loot.transform.SetParent(InventoryObject.transform);
                    Loot.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[2], Main.DefaultHeight);
                    Loot.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1] / 2 + aranyok[2], 0, 0);
                }
            }
        }
        private float[] Aranyszamitas(float[] szamok, float max)
        {
            float szam4 = max / szamok.Sum();
            float[] retunvalues = new float[szamok.Length];
            for (int i = 0; i < retunvalues.Length; i++)
            {
                retunvalues[i] = szam4 * szamok[i];
            }
            return retunvalues;
        }
        private GameObject CreatePrefab(string path)
        {
            GameObject prefab = Instantiate(Resources.Load<GameObject>(path));
            if (prefab != null)
            {
                return prefab;
            }
            else
            {
                Debug.LogError($"{path} prefab nem található!");
                return null;
            }
        }










        public void InventoryLoad()//kelelne egy save manager script ami a be ovasat es a kiirast kezelni ezzel lehet idot lehetni sporolni
        {
            if (File.Exists("UserSave.json"))
            {
                string jsonString = File.ReadAllText("PlayerSave.json");
                equipments = JsonConvert.DeserializeObject<DefaultInvetoryStruct>(jsonString);
            }
            else
            {
                NewInventory();
            }
        }
        private void NewInventory()
        {
            equipments = new DefaultInvetoryStruct();
        }
        public void InventoryAdd(Item item)
        {

        }
        public void InventoryDelete(Item item)
        {

        }
        public void InventorySave()
        {

        }


    };



}
  




namespace Items
{
    public abstract class Item : MonoBehaviour// az item hozza letre azt a panelt a slot inventoryban amely a tartlomert felelos, de csak akkor ha õ equipment slotban van egybekent egy up relativ pozitcioju panelt hozzon letre mint az EFT-ban
    {
        private void CopyProperties(Item source)
        {
            //altalanos adatok
            ItemType = source.ItemType;
            Name = source.Name;
            Description = source.Description;
            Quantity = source.Quantity;
            position = source.position;
            //Nem biztosak
            SlotUse = source.SlotUse;
            sectorName = source.sectorName;
            //tartalom
            Container = source.Container;
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
        public void ItemConstructorSelector(Item uncompletedItem)
        {
            Item completedItem = uncompletedItem.ItemType switch
            {
                "TestWeapon" => new TestWeapon(),
                "TestBackpack" => new TestBackpack(),
                _ => throw new ArgumentException("Invalid type")
            };
            CopyProperties(completedItem);
        }

        private void Start()
        {
            //self building / constructing

        }
        /* lefedes!!!!!
        public void Start()//nem biztos hogy mukodik
        {
            RectTransform parentRect = GetComponent<RectTransform>();
            if (parentRect == null) return;

            // Inicializáljuk a minimum és maximum pozíciókat
            Vector3 minPosition = Vector3.positiveInfinity;
            Vector3 maxPosition = Vector3.negativeInfinity;

            // Iterálj a gyerek objektumokon
            foreach (RectTransform child in transform)
            {
                // A gyerek objektumok pozíciója a szülõhöz képest
                Vector3 childPosition = child.localPosition;
                Vector3 childSize = child.rect.size;

                // Számold ki a gyerek pozíciójának sarkait
                Vector3 childMin = childPosition - new Vector3(childSize.x / 2, childSize.y / 2, 0);
                Vector3 childMax = childPosition + new Vector3(childSize.x / 2, childSize.y / 2, 0);

                // Frissítsd a minimum és maximum pozíciókat
                minPosition = Vector3.Min(minPosition, childMin);
                maxPosition = Vector3.Max(maxPosition, childMax);
            }

            // Számold ki a szülõ méretét
            Vector3 newSize = maxPosition - minPosition;
            parentRect.sizeDelta = new Vector2(newSize.x, newSize.y);

            // Pozicionáld a szülõt a középpontba
            parentRect.localPosition = (minPosition + maxPosition) / 2;
        }
        */

        //general
        public string ItemType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        public Vector2 position { get; set; }
        public int[] SlotUse { get; set; }
        public string sectorName { get; set; }

        //contain
        public Container Container { get; set; } = null;



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

    public class Container
    {
        public List<Container_struct> Items { get; set; }
        public Container()
        {

        }
        public int[,] slotZones { get; set; }
    }
    public struct Container_struct
    {
        public Item Item { get; set; }
    }

    public class SlotSector
    {
        public Vector2 position { get; set; }
    }

}
public class BulletType
{

}

public class Accessors
{

}










