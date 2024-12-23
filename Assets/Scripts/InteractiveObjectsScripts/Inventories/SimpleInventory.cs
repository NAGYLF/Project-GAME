using ItemHandler;
using System.Collections.Generic;
using UnityEngine;
using static ItemHandler.InventorySystem;

namespace NaturalInventorys
{
    public class SimpleInventory : MonoBehaviour
    {
        [SerializeField] public string PaletteName;
        [SerializeField] public float Fullness;
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
            LootRandomizer.FillSimpleInvenotry(GetComponent<SimpleInventory>(),PaletteName,Fullness);
        }
        public void InventoryAdd(Item Data)
        {
            bool ItemAdded = false;
            if (!ItemAdded)//container
            {
                for (int sectorIndex = 0; sectorIndex < Root.Container.Sectors.Length && !ItemAdded; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                {
                    if (Root.Container.Sectors[sectorIndex].GetLength(1) >= Data.SizeX && Root.Container.Sectors[sectorIndex].GetLength(0) >= Data.SizeY)//egy gyors ellenörzést végzünk, hogy az itemunk a feltetelezett teljesen ures sectorba belefér e, ha nem kihadjuk
                    {
                        for (int Y = 0; Y < Root.Container.Sectors[sectorIndex].GetLength(0) && !ItemAdded; Y++)//vegig iterálunk a sorokon
                        {
                            for (int X = 0; X < Root.Container.Sectors[sectorIndex].GetLength(1) && !ItemAdded; X++)//a sorokon belul az oszlopokon
                            {
                                if (Root.Container.Sectors[sectorIndex][Y, X].PartOfItemData == null && CanBePlace(Root.Container.Sectors[sectorIndex], Y, X, Data))//ha a slot nem tagja egy itemnek sem akkor target
                                {
                                    SetNewDataParent(Root, Data);
                                    SetSlotUseBySector(Y,X,sectorIndex,Root,Data);
                                    DataAdd(Root,Data);
                                    ItemAdded = true;
                                }
                            }
                        }
                    }
                }
            }
            if (!ItemAdded)
            {
                Debug.LogWarning($"item: {Data.ItemName} cannot added, probably no space for that");
            }
        }
        //public void InventoryRemove(Item item)//pocitciot és rogatast figyelmen kivul hagy     ,    csak 1 db tavolit el
        //{
        //    //item.SetItem(item.ItemName);
        //    Debug.Log($"Remove: {item.ItemName}  1db  in progress");
        //    bool ItemRemoved = false;
        //    if (!ItemRemoved)//container
        //    {
        //        for (int sectorIndex = 0; sectorIndex < Root.Container.Sectors.Length && !ItemRemoved; sectorIndex++)
        //        {
        //            for (int Row = 0; Row < Root.Container.Sectors[sectorIndex].GetLength(0) && !ItemRemoved; Row++)
        //            {
        //                for (int Col = 0; Col < Root.Container.Sectors[sectorIndex].GetLength(1) && !ItemRemoved; Col++)
        //                {
        //                    if (Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData != null && Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.ItemName == item.ItemName)
        //                    {
        //                        Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity -= item.Quantity;
        //                        int count = 0;
        //                        if (Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity > 0)
        //                        {
        //                            ItemRemoved = true;//csak menyiseget törlünk
        //                        }
        //                        else if (Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity == 0)
        //                        {
        //                            ItemRemoved = true;
        //                            Root.Container.Items.RemoveAt(Root.Container.Items.FindIndex(item => item.SlotUse.Contains(Root.Container.Sectors[sectorIndex][Row, Col].SlotName)));

        //                            Item RefItem = Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData;

        //                            foreach (ItemSlotData itemSlot in Root.Container.Sectors[sectorIndex])
        //                            {

        //                            }
        //                        }
        //                        else
        //                        {
        //                            count = Math.Abs(Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData.Quantity);
        //                            item.Quantity = count;
        //                            Root.Container.Items.RemoveAt(Root.Container.Items.FindIndex(item => item.SlotUse.Contains(Root.Container.Sectors[sectorIndex][Row, Col].SlotName)));

        //                            Item RefItem = Root.Container.Sectors[sectorIndex][Row, Col].PartOfItemData;

        //                            foreach (ItemSlotData itemSlot in Root.Container.Sectors[sectorIndex])
        //                            {
                      
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (!ItemRemoved)
        //    {
        //        Debug.LogWarning($"item: {item.ItemName} cannot removed, probably the item doesn't exist");
        //    }
        //}
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