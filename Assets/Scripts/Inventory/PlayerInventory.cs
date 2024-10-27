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
using MainData;
using System.Drawing;



namespace InventoryClass
{
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Playerinventory;//a player mindenhol elerheto inventoryja

        public DefaultInvetoryStruct equipments;

        private bool InventoryOpen = false;

        private GameObject InventoryObject;
        public GameObject EquipmentsObject;
        public GameObject SlotObject;
        public GameObject LootObject;
        public Item[] itemArray()
        {
            return new Item[]
            {
                //sector1
                equipments.HelmetSlot,
                equipments.ArmorSlot,
                equipments.PantsSlot,
                equipments.BootsSlot,
                //sector2
                equipments.MaskSlot,
                equipments.VestSlot,
                equipments.BackbackSlot,
                //sector3
                equipments.HeadsetSlot,
                equipments.SkinSlot,
                equipments.FingerSlot,
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
            InventoryLoad();
            InventoryEquipmentsBuld();
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

                    NewGameObject.AddComponent<Item>().SetItem(item);

                    NewGameObject.transform.SetParent(gameObject.transform);
                }


            }

        }
        public void OpenCloseInventory()//ez az inventoryt epiti fel
        {
            if (Input.GetKeyUp(KeyCode.Tab))
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

                    EquipmentsObject = CreatePrefab("GameElements/Equipment-Inventory");
                    EquipmentsObject.transform.SetParent(InventoryObject.transform);
                    EquipmentsObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[0], Main.DefaultHeight);
                    EquipmentsObject.GetComponent<RectTransform>().localPosition = new Vector3((aranyok[0] + aranyok[1]/2)*-1, 0, 0);
                    foreach (GameObject item in EquipmentsObject.GetComponent<EquipmentsPanelScript>().EquipmentsSlots)
                    {
                        for (int i = 0; i < itemArray().Length; i++)
                        {
                            if (itemArray()[i] != null && itemArray()[i].SlotUse[0] == item.GetComponent<EquipmentSlotScript>().SlotName)
                            {
                                GameObject ItemObject = new GameObject($"{itemArray()[i].name}");
                                item.transform.SetParent(ItemObject.transform);
                                ItemObject.AddComponent<Item>().SetItem(itemArray()[i]);
                            }
                        }
                    }

                    SlotObject = CreatePrefab("GameElements/Slots-Inventory");
                    SlotObject.transform.SetParent(InventoryObject.transform);
                    SlotObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[1], Main.DefaultHeight);
                    SlotObject.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1]*-1 / 2, 0, 0);
                    SlotObject.AddComponent<SlotsPanelScript>();

                    LootObject = CreatePrefab("GameElements/Loot-Inventory");
                    LootObject.transform.SetParent(InventoryObject.transform);
                    LootObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[2], Main.DefaultHeight);
                    LootObject.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1] / 2 + aranyok[2], 0, 0);
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
        public GameObject CreatePrefab(string path)
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









        public void InventorySave()
        {

        }
        public void InventoryLoad()//kelelne egy save manager script ami a be ovasat es a kiirast kezelni ezzel lehet idot lehetni sporolni
        {
            if (File.Exists("UserSave.json"))
            {
                string jsonString = File.ReadAllText("PlayerSave.json");
                equipments = JsonConvert.DeserializeObject<DefaultInvetoryStruct>(jsonString);
                Playerinventory = this;
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
            item.SetItem(item);
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
    using InventoryClass;
    public class Item : ItemStruct
    {
        private void Start()
        {
            //Slot/Slotok lefedése
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null) return;

            // Inicializáljuk a minimum és maximum pozíciókat
            Vector3 minPosition = Vector3.positiveInfinity;
            Vector3 maxPosition = Vector3.negativeInfinity;

            // Iteráljunk a gyerek objektumokon
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
            Vector2 newSize = maxPosition - minPosition;
            rectTransform.sizeDelta = newSize;

            // Pozicionáld a szülõt a középpontba
            rectTransform.localPosition = (minPosition + maxPosition) / 2;



            //Item kép létrehozása
            gameObject.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(this.ImgPath);

            //Ha van tartalma akkor azt generálásra küldjük a SlotPanel-be.
            if (this.Container != null)
            {
                Container.CreatContainer(PlayerInventory.Playerinventory.SlotObject.transform.GetChild(0).transform);
            }
        }
        private void CopyProperties(Item source)
        {
            //altalanos adatok
            ItemType = source.ItemType;//ez alapján kerülhet be egy slotba ugyan is vannak pecifikus slotok melyeknek typusváltozójában benen kell, hogy legyen.
            Name = source.Name;//ez alapján hozza létre egy item saját magát
            Description = source.Description;
            Quantity = source.Quantity;
            position = source.position;//a 2d pozitcioja. ez azert kell, hogy az item elfoglalja es teljesen le is fedje a slot objektumot (ATALAKITAS KELL: ennek egy trasformnak kell lenni, hogy orokolje mind pozitciojat mind nagysagat is)
            SlotUse = source.SlotUse;// ez a jelenleg elfoglalt helye, ezt a betolteskor hasznaljuk, hogy tudjuk mit hova raktunk el.
            ImgPath = source.ImgPath;
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
        public void SetItem(Item uncompletedItem)
        {
            Item completedItem = uncompletedItem.Name switch
            {
                "TestWeapon" => new TestWeapon().Set(),
                //"TestBackpack" => new TestBackpack(),
                _ => throw new ArgumentException("Invalid type")
            };
            CopyProperties(completedItem);
        }
    }
    public abstract class ItemStruct : MonoBehaviour// az item hozza letre azt a panelt a slot inventoryban amely a tartlomert felelos, de csak akkor ha õ equipment slotban van egybekent egy up relativ pozitcioju panelt hozzon letre mint az EFT-ban
    {

        //general
        public string ItemType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        public Vector2 position { get; set; }
        public string[] SlotUse { get; set; }
        public string ImgPath { get; set; }
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

    public class Container : MonoBehaviour
    {
        public List<Item> Items { get; set; }
        public string PrefabPath;

        public void CreatContainer(Transform ParentObject)
        {
            GameObject Prefab = PlayerInventory.Playerinventory.CreatePrefab(PrefabPath);
            Prefab.AddComponent<Container>().Set(this.Items);
            Prefab.name = PrefabPath.Split('\\').Last();
            ParentObject.GetComponent<SlotsPanelScript>().Containers.Add(Prefab);
        }
        public void Set(List<Item> items)
        {
            Items = items;
        }
    }
    public class BulletType
    {

    }

    public class Accessors
    {

    }
}











