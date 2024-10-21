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



namespace PlayerInventoryManagment
{

    public class PlayerInventory : MonoBehaviour
    {
        public Inventory inventory;
        //public EquipmentList;
        public PlayerInventory()
        {
            InventoryLoad();
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
        public void InventoryLoad()
        {
            if (File.Exists("UserSave.json"))
            {
                string jsonString = File.ReadAllText("PlayerSave.json");
                Player.playerInventory.inventory = JsonConvert.DeserializeObject<Inventory>(jsonString);
            }
            else
            {
                NewInventory();
            }
        }

        private void NewInventory()
        {

        }
    }



    public class Inventory
    {
       public Equiments equiments { get; set; }
    }

    public struct Equiments
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
        public Item PanstSlot { get; set; }
        public Item SkinSlot { get; set; }
    }

    public class ItemConstructorSelector
    {
        private Item item;
        public ItemConstructorSelector(string type,string name)
        {
            item = type switch
            {
                "TestWeapon" => new TestWeapon(),
                "TestBackpack" => new TestBackpack(),
                _ => throw new ArgumentException("Invalid type")
            };
        }
    }
}



namespace Items
{
    public abstract class Item : MonoBehaviour
    {
        /*
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
        public string ItemType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        private Vector2 position { get; set; }

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
        private List<Container_struct> Items { get; set; }
        public Container()
        {

        }
        public int[,] slotZones { get; set; }
    }
    public struct Container_struct
    {
        Item Item { get; set; }
        int[] SlotUse { get; set; }
        string sectorName { get; set; }
    }

    public class SlotSector
    {
        Vector2 position { get; set; }
    }

}
public class BulletType
{

}

public class Accessors
{

}










