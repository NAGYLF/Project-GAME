using ItemHandler;
using MainData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NaturalInventorys
{
    public class SimpleInventory : MonoBehaviour
    {
        [SerializeField] public string PrefabPath;//container prefab path

        [HideInInspector] public Item Root;// ez lényegében az inventory adatait tartlamazza
        public void Awake()
        {
            Root = new Item();
            Root.ItemName = "Root";
            Root.lvl = -1;
            Root.IsRoot = true;
            Root.IsLoot = true;
            Root.Container = new Container(PrefabPath);
        }
        public void InventoryAdd(Item item)
        {
            bool ItemAdded = false;
            if (!ItemAdded)//container gyorsitott item hozzadas mely nem ad uj elemet hanem csak quanity-t novel
            {
                for (int j = 0; j < Root.Container.Items.Count && !ItemAdded; j++)
                {
                    if (Root.Container.Items[j].ItemName == item.ItemName && Root.Container.Items[j].Quantity != Root.Container.Items[j].MaxStackSize)
                    {
                        int originalCount = Root.Container.Items[j].Quantity;
                        Root.Container.Items[j].Quantity += item.Quantity;
                        if (Root.Container.Items[j].Quantity > Root.Container.Items[j].MaxStackSize)
                        {
                            item.Quantity -= (Root.Container.Items[j].MaxStackSize - originalCount);
                            Root.Container.Items[j].Quantity = Root.Container.Items[j].MaxStackSize;
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
                for (int sectorIndex = 0; sectorIndex < Root.Container.Sectors.Length && !ItemAdded; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                {
                    if (Root.Container.Sectors[sectorIndex].GetLength(1) >= item.SizeX && Root.Container.Sectors[sectorIndex].GetLength(0) >= item.SizeY)//egy gyors ellenörzést végzünk, hogy az itemunk a feltetelezett teljesen ures sectorba belefér e, ha nem kihadjuk
                    {
                        for (int Y = 0; Y < Root.Container.Sectors[sectorIndex].GetLength(0) && !ItemAdded; Y++)//vegig iterálunk a sorokon
                        {
                            for (int X = 0; X < Root.Container.Sectors[sectorIndex].GetLength(1) && !ItemAdded; X++)//a sorokon belul az oszlopokon
                            {
                                if (Root.Container.Sectors[sectorIndex][Y, X].PartOfItemData == null && CanBePlace(Root.Container.Sectors[sectorIndex], Y, X, item))//ha a slot nem tagja egy itemnek sem akkor target
                                {
                                    int index = 0;
                                    item.SlotUse = new List<string>();
                                    List<ItemSlotData> itemSlots = new List<ItemSlotData>();
                                    for (int y = Y; y < Y + item.SizeY; y++)
                                    {
                                        for (int x = X; x < X + item.SizeX; x++)
                                        {
                                            itemSlots.Add(Root.Container.Sectors[sectorIndex][y, x]);
                                            item.SlotUse[index] = Root.Container.Sectors[sectorIndex][y, x].SlotName;//ez alapjan azonositunk egy itemslotot
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
                                    foreach (ItemSlotData itemSlot in Root.Container.Sectors[sectorIndex])
                                    {
                                        if (itemSlots.Exists(slot => slot.SlotName == itemSlot.SlotName))
                                        {
                                            itemSlot.PartOfItemData = item;
                                        }
                                    }
                                    Root.Container.Items.Add(item);
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
                for (int sectorIndex = 0; sectorIndex < Root.Container.Sectors.Length && !ItemRemoved; sectorIndex++)
                {
                    for (int Row = 0; Row < Root.Container.Sectors[sectorIndex].GetLength(0) && !ItemRemoved; Row++)
                    {
                        for (int Col = 0; Col < Root.Container.Sectors[sectorIndex].GetLength(1) && !ItemRemoved; Col++)
                        {
                            if (Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData != null && Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.ItemName == item.ItemName)
                            {
                                Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity -= item.Quantity;
                                int count = 0;
                                if (Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity > 0)
                                {
                                    ItemRemoved = true;//csak menyiseget törlünk
                                }
                                else if (Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity == 0)
                                {
                                    ItemRemoved = true;
                                    Root.Container.Items.RemoveAt(Root.Container.Items.FindIndex(item => item.SlotUse.Contains(Root.Container.Sectors[sectorIndex][Row, Col].SlotName)));

                                    Item RefItem = Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData;

                                    foreach (ItemSlotData itemSlot in Root.Container.Sectors[sectorIndex])
                                    {
                      
                                    }
                                }
                                else
                                {
                                    count = Math.Abs(Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity);
                                    item.Quantity = count;
                                    Root.Container.Items.RemoveAt(Root.Container.Items.FindIndex(item => item.SlotUse.Contains(Root.Container.Sectors[sectorIndex][Row, Col].SlotName)));

                                    Item RefItem = Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData;

                                    foreach (ItemSlotData itemSlot in Root.Container.Sectors[sectorIndex])
                                    {
                      
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