using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;




namespace PlayerInventoryManagment
{
    public class PlayerInventory : MonoBehaviour
    {
       

        public static void PlayerInventoryAdd(Item item)
        {

        }
        public static void PlayerInventoryDelete(Item item)
        {

        }
        public static void PlayerInventorySave()
        {

        }
        public static void PlayerInventoryLoad()
        {

        }
    }

    class PlayerInventoryContent
    {

    }
}



namespace Items
{
    public class Item
    {
        public Item(Item item)
        {
            switch (item.ItemType)
            {
                case "Weapon" :
                    
                    break;
                default:
                    break;
            }
        }

        public string ItemType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }


        public Item ItemDataGet()
        {
            return this;
        }












    }

}
class Weapon
{
    Weapon()
    {

    }

    public int DefaultMagasineSize { get; set; }
    public double Spread { get; set; }
    public int Rpm { get; set; }
    public double Recoil { get; set; }
    public double Accturacy { get; set; }
    public double Range { get; set; }
    public double Ergonomy { get; set; }
    public BulletType BulletType { get; set; }
    public Accessors Accessors { get; set; }
    public string Sound { get; set; }

}


class BulletType
{

}

class Accessors
{

}

class Containers
{
    private List<Item> Items { get; set; }
    Containers()
    {

    }
}


