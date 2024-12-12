using Assets.Scripts.Inventory;
using ItemHandler;
using MainData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace NaturalInventorys
{
    public class SimpleInventory : MonoBehaviour
    {
        [HideInInspector] public Item MainData;//az obejctumon lévõ másik script végzi a kezdeti feltöltest
        [SerializeField] public string PrefabPath;//container prefab path
        private GameObject Container;
        public void Test()
        {
            MainData = new Item();
            MainData.Container = new Container(PrefabPath);
        }
        public void Start()
        {
            Test();
        }
        public void DataUpdate(Item Data)
        {
            MainData = Data;
        }
        public void InventoryAdd(Item item)
        {
            bool ItemAdded = false;
            if (!ItemAdded)//container gyorsitott item hozzadas mely nem ad uj elemet hanem csak quanity-t novel
            {
                for (int j = 0; j < MainData.Container.Items.Count && !ItemAdded; j++)
                {
                    if (MainData.Container.Items[j].ItemName == item.ItemName && MainData.Container.Items[j].Quantity != MainData.Container.Items[j].MaxStackSize)
                    {
                        int originalCount = MainData.Container.Items[j].Quantity;
                        MainData.Container.Items[j].Quantity += item.Quantity;
                        if (MainData.Container.Items[j].Quantity > MainData.Container.Items[j].MaxStackSize)
                        {
                            item.Quantity -= (MainData.Container.Items[j].MaxStackSize - originalCount);
                            MainData.Container.Items[j].Quantity = MainData.Container.Items[j].MaxStackSize;
                        }
                        else
                        {
                            ItemAdded = true;
                        }
                    }
                }
            }
            if (!ItemAdded)//container
            {
                for (int sectorIndex = 0; sectorIndex < MainData.Container.Sectors.Length && !ItemAdded; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                {
                    if (MainData.Container.Sectors[sectorIndex].GetLength(1) >= item.SizeX && MainData.Container.Sectors[sectorIndex].GetLength(0) >= item.SizeY)//egy gyors ellenörzést végzünk, hogy az itemunk a feltetelezett teljesen ures sectorba belefér e, ha nem kihadjuk
                    {
                        for (int Y = 0; Y < MainData.Container.Sectors[sectorIndex].GetLength(0) && !ItemAdded; Y++)//vegig iterálunk a sorokon
                        {
                            for (int X = 0; X < MainData.Container.Sectors[sectorIndex].GetLength(1) && !ItemAdded; X++)//a sorokon belul az oszlopokon
                            {
                                if (MainData.Container.Sectors[sectorIndex][Y, X].PartOfItemData == null && CanBePlace(MainData.Container.Sectors[sectorIndex], Y, X, item))//ha a slot nem tagja egy itemnek sem akkor target
                                {
                                    int index = 0;
                                    item.SlotUse = new string[item.SizeX * item.SizeY];
                                    List<ItemSlotData> itemSlots = new List<ItemSlotData>();
                                    for (int y = Y; y < Y + item.SizeY; y++)
                                    {
                                        for (int x = X; x < X + item.SizeX; x++)
                                        {
                                            itemSlots.Add(MainData.Container.Sectors[sectorIndex][y, x]);
                                            item.SlotUse[index] = MainData.Container.Sectors[sectorIndex][y, x].SlotName;//ez alapjan azonositunk egy itemslotot
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
                                    foreach (ItemSlotData itemSlot in MainData.Container.Sectors[sectorIndex])
                                    {
                                        if (itemSlots.Exists(slot => slot.SlotName == itemSlot.SlotName))
                                        {
                                            itemSlot.PartOfItemData = item;
                                        }
                                    }
                                    MainData.Container.Items.Add(item);
                                    item = new Item(item.ItemName, count);
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
        public void InventoryRemove(Item item)//pocitciot és rogatast figyelmen kivul hagy     ,    csak 1 db tavolit el
        {
            //item.SetItem(item.ItemName);
            Debug.Log($"Remove: {item.ItemName}  1db  in progress");
            bool ItemRemoved = false;
            if (!ItemRemoved)//container
            {
                for (int sectorIndex = 0; sectorIndex < MainData.Container.Sectors.Length && !ItemRemoved; sectorIndex++)
                {
                    for (int Row = 0; Row < MainData.Container.Sectors[sectorIndex].GetLength(0) && !ItemRemoved; Row++)
                    {
                        for (int Col = 0; Col < MainData.Container.Sectors[sectorIndex].GetLength(1) && !ItemRemoved; Col++)
                        {
                            if (MainData.Container.Sectors[sectorIndex][Row, Col].PartOfItemData != null && MainData.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.ItemName == item.ItemName)
                            {
                                MainData.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity -= item.Quantity;
                                int count = 0;
                                if (MainData.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity > 0)
                                {
                                    ItemRemoved = true;//csak menyiseget törlünk
                                }
                                else if (MainData.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity == 0)
                                {
                                    ItemRemoved = true;
                                    MainData.Container.Items.RemoveAt(MainData.Container.Items.FindIndex(item => item.SlotUse.Contains(MainData.Container.Sectors[sectorIndex][Row, Col].SlotName)));

                                    Item RefItem = MainData.Container.Sectors[sectorIndex][Row, Col].PartOfItemData;

                                    foreach (ItemSlotData itemSlot in MainData.Container.Sectors[sectorIndex])
                                    {
                                        if (RefItem.GetSlotUseId().Contains(itemSlot.SlotName))
                                        {
                                            itemSlot.PartOfItemData = null;
                                        }
                                    }
                                }
                                else
                                {
                                    count = Math.Abs(MainData.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity);
                                    item.Quantity = count;
                                    MainData.Container.Items.RemoveAt(MainData.Container.Items.FindIndex(item => item.SlotUse.Contains(MainData.Container.Sectors[sectorIndex][Row, Col].SlotName)));

                                    Item RefItem = MainData.Container.Sectors[sectorIndex][Row, Col].PartOfItemData;

                                    foreach (ItemSlotData itemSlot in MainData.Container.Sectors[sectorIndex])
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
            if (!ItemRemoved)
            {
                Debug.LogWarning($"item: {item.ItemName} cannot removed, probably the item doesn't exist");
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
    }
}