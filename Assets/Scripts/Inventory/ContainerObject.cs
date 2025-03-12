using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using MainData;
using Assets.Scripts;
using System.Linq;
using static PlayerInventoryClass.PlayerInventory;
using static MainData.SupportScripts;
using Assets.Scripts.Inventory;
using PlayerInventoryClass;
using System;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Drawing;
using NPOI.SS.Formula.Functions;
using Unity.VisualScripting;
using System.Text;


public class ContainerObject : MonoBehaviour
{
    #region DataSynch
    public Item ActualData;//ezek alapj�n vizualiz�lja es szinkroniz�lja az itemeket az az ha valami ezt az objektumot staterk�nt megkapja akkor ezt kell megvaltoztatni a szinkronizalashoz
    #endregion

    #region Personal variables
    public List<DataGrid> Sectors;//ez egy m�trox lista amely tartalmazza az �sszes itemSlot Objectumot
    public GameObject[] SectorObjects;//ez egy gamObject lista amely tatalmmazza az �sszes sectort m�retez�si �s itemObject el�r�s�nek sz�nd�k�b�l
    #endregion

    #region Active Slot Handler variables
    //Ezen v�ltoz�k sz�ks�gesek ahoz, hogy egy itemet helyezni tudjunk slotokb�l slotokba
    [HideInInspector] public HashSet<ItemSlot> interactibleSlots;
    [HideInInspector] private bool CanBePlaceble = true;
    //[HideInInspector] public GameObject PlaceableObject;

    [HideInInspector] public List<GameObject> activeSlots;
    [HideInInspector] public bool ChangedFlag = false;

    private Camera mainCam;
    #endregion

    #region Active Slot Handler
    public IEnumerator Targeting()
    {
        if (ChangedFlag)
        {
            ChangedFlag = false;

            if (activeSlots.Count > 0)
            {
                Item IncomingItem;
                CanBePlaceble = true;

                if (activeSlots.First().GetComponent<ItemSlot>().ActualPartOfItemObject.GetComponent<ItemObject>())
                {
                    IncomingItem = activeSlots.First().GetComponent<ItemSlot>().ActualPartOfItemObject.GetComponent<ItemObject>().ActualData;
                }
                else
                {
                    IncomingItem = activeSlots.First().GetComponent<ItemSlot>().ActualPartOfItemObject.GetComponent<TemporaryItemObject>().ActualData;
                }

                int Y = 0;
                int X = 0;
                if (IncomingItem.RotateDegree == 90 || IncomingItem.RotateDegree == 270)
                {
                    X = IncomingItem.SizeY;
                    Y = IncomingItem.SizeX;
                }
                else
                {
                    Y = IncomingItem.SizeY;
                    X = IncomingItem.SizeX;
                }

                Dictionary<int, ItemSlot[]> SlotsBySectors = activeSlots
                    .GroupBy(slot => slot.GetComponent<ItemSlot>().sectorId)
                    .ToDictionary(sector => sector.Key, slots =>

                        slots
                       .GroupBy(slot => slot.GetComponent<ItemSlot>().Coordinate.Height)
                       .ToDictionary(Y_slots => Y_slots.Key, Y_slots =>
                        Y_slots.ToArray())

                       .OrderBy(dictionary => dictionary.Key)
                       .TakeLast(Y)

                       .SelectMany(slots => slots.Value)
                       .GroupBy(slot => slot.GetComponent<ItemSlot>().Coordinate.Width)
                       .OrderBy(dictionary => dictionary.Key)
                       .TakeLast(X)

                       .SelectMany(objectArrays => objectArrays)
                       .Select(slot => slot.GetComponent<ItemSlot>())
                       .ToArray()
                    );



                foreach (ItemSlot slot_ in interactibleSlots)
                {
                    slot_.Deactivation();
                }

                interactibleSlots.Clear();

                if (IncomingItem.AvaiablePlacerMetodes == null) IncomingItem.AvaiablePlacerMetodes = new List<Action>();
                IncomingItem.AvaiablePlacerMetodes.Clear();

                IncomingItem.AvaiableParentItem = ActualData;

                for (int i = 0; i < SlotsBySectors.Count; i++)
                {
                    foreach (ItemSlot slot_ in SlotsBySectors.ElementAt(i).Value)
                    {
                        interactibleSlots.Add(slot_);
                    }
                }



                if (SlotsBySectors.Count == 1)
                {
                    foreach (ItemSlot slot in interactibleSlots)
                    {
                        if (!(slot.PartOfItemObject == null || slot.PartOfItemObject == IncomingItem.SelfGameobject))
                        {
                            CanBePlaceble = false;
                            break;
                        }
                        if (!(slot.SlotType == "" || slot.SlotType.Contains(IncomingItem.ItemType)))
                        {
                            CanBePlaceble = false;
                            break;
                        }
                    }

                    if (!(interactibleSlots.Count == IncomingItem.SizeX * IncomingItem.SizeY || interactibleSlots.First().IsEquipment))
                    {
                        CanBePlaceble = false;
                    }

                    ItemSlot RefSlot = interactibleSlots.FirstOrDefault(slot => slot.MouseOver && slot.PartOfItemObject != null && slot.PartOfItemObject != IncomingItem.SelfGameobject);

  

                    Item InteractiveItem = RefSlot?.ActualPartOfItemObject.GetComponent<ItemObject>().ActualData;

                    if (RefSlot != null)
                    {
                        //if (InventorySystem.CanMergable(InteractiveItem, IncomingItem))
                        //{
                        //    InventorySystem.Merge ActionMerge = new(InteractiveItem, IncomingItem);
                        //    IncomingItem.AvaiablePlacerMetodes.Add(ActionMerge.Execute_Merge);
                        //}
                        //if (InteractiveItem.PartPut_IsPossible(IncomingItem).IsPossible)
                        //{
                        //    InventorySystem.MergeParts ActionMergeParts = new(InteractiveItem, IncomingItem);
                        //    IncomingItem.AvaiablePlacerMetodes.Add(ActionMergeParts.Execute_MergeParts);
                        //}
                        //if (InventorySystem.CanSplitable(InteractiveItem, IncomingItem))
                        //{
                        //    InventorySystem.Split ActionSplit = new(InteractiveItem, interactibleSlots.ToArray());
                        //    IncomingItem.AvaiablePlacerMetodes.Add(ActionSplit.Execute_Split);
                        //}
                    }
                    else if(CanBePlaceble)
                    {
                        IncomingItem.AvaiablePlacerMetodes.Clear();

                        if (InventorySystem.CanSplitable(InteractiveItem, IncomingItem))
                        {
                            InventorySystem.Split ActionSplit = new(IncomingItem, interactibleSlots.ToArray());
                            IncomingItem.AvaiablePlacerMetodes.Add(ActionSplit.Execute_Split);
                        }
                        InventorySystem.RePlace ActionRePlace = new(IncomingItem, ActualData, interactibleSlots.ToArray());
                        IncomingItem.AvaiablePlacerMetodes.Add(ActionRePlace.Execute_RePlace);
                    }

                    if (CanBePlaceble)
                    {

                        //open all
                        foreach (ItemSlot slot in interactibleSlots)
                        {
                            slot.Open();
                        }
                    }
                    else
                    {
                        //close all slot
                        foreach (ItemSlot slot in interactibleSlots)
                        {
                            slot.Close();
                        }
                    }
                }
                else
                {
                    CanBePlaceble = false;
                    //close all slot
                    foreach (ItemSlot slot in interactibleSlots)
                    {
                        slot.Close();
                    }
                }
            }
        }
        else if (activeSlots.Count == 0)
        {
            HashSet<ItemSlot> interactibleSlots_ = new(interactibleSlots);
            foreach (ItemSlot slot_ in interactibleSlots_)
            {
                slot_.Deactivation();
                interactibleSlots.Remove(slot_);
            }
            interactibleSlots_.Clear();
        }
        yield return null;
    }
    private void Update()
    {
        StartCoroutine(CheckMousePosition());
        StartCoroutine(Targeting());
    }
    public void Start()
    {
        Inicialisation();
        LoadItemObjects();
    }
    private void Awake()
    {
        mainCam = Camera.main;
    }
    private IEnumerator CheckMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        foreach (GameObject go in activeSlots)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            ItemSlot itemSlot = go.GetComponent<ItemSlot>();

            bool isInside = RectTransformUtility.RectangleContainsScreenPoint(rt, mousePosition, mainCam);
            itemSlot.MouseOver = isInside;
        }

        yield return null;
    }
    public void SetDataRoute(Item Data)//(ezen elj�r�s ezen objektum j�t�kbaker�l�se el�tt zajlik le)    C�lja, hogy a gy�k�rb�l tov�bb�tott �s egyben ennek az objektumnak sz�nt adatokat ezen VCP megkapja
    {
        ActualData = Data;
    }
    #endregion

    public void Inicialisation()//az objecktum l�trehoz�s�nak els� pillanat�ban t�lt�dik be
    {
        for (int sector = 0; sector < Sectors.Count; sector++)
        {
            for (int col = 0; col < Sectors[sector].columnNumber; col++)
            {
                for (int row = 0; row < Sectors[sector].rowNumber; row++)
                {
                    ItemSlot slot = Sectors[sector].col[col].row[row].GetComponent<ItemSlot>();
                    slot.SlotParentItem = ActualData;
                    slot.sectorId = sector;
                    slot.Coordinate = (col,row);
                }
            }
        }

        interactibleSlots = new HashSet<ItemSlot>();
        activeSlots = new List<GameObject>();

        ActualData.ContainerObject = gameObject;
        ActualData.Container.Live_Sector = gameObject.GetComponent<ContainerObject>().Sectors;

        if (ActualData.IsEquipment)
        {
            VisualisationToSlotPanel();
        }
        else if (ActualData.IsLoot)
        {
            VisualisationToLootPanel();
        }
    }
    private void LoadItemObjects()
    {
        for (int i = 0; i < ActualData.Container.Items.Count; i++)//l�trehozzuk itemObjektumait
        {
            //Debug.Log($"{ActualData.Container.Items[i].ItemName} creating into {ActualData.ItemName}'s container");
            GameObject itemObject = CreatePrefab(ActualData.Container.Items[i].ObjectPath);
            itemObject.GetComponent<ItemObject>().SetDataRoute(ActualData.Container.Items[i], ActualData);//L�trehozzuk a szikroniz�l�si utat ezen VirtualParentObject �s a VirtualChildrenObject k�z�tt
        }
    }
    private void VisualisationToLootPanel()
    {
        GameObject lootObject = InventoryObjectRef.gameObject.GetComponent<PlayerInventory>().LootPanelObject;
        gameObject.transform.SetParent(lootObject.GetComponent<PanelLoot>().Content.transform, false);
        foreach (GameObject sector in SectorObjects)//beallitjuk a m�retar�nyt
        {
            sector.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }
        //ActualData.ContainerObject = gameObject;
        //ActualData.Container.Live_Sector = gameObject.GetComponent<ContainerObject>().Sectors;
    }
    private void VisualisationToSlotPanel()
    {
        GameObject slotObject = InventoryObjectRef.gameObject.GetComponent<PlayerInventory>().SlotPanelObject;
        gameObject.transform.SetParent(slotObject.GetComponent<PanelSlots>().Content.transform, false);
        foreach (GameObject sector in SectorObjects)//beallitjuk a m�retar�nyt
        {
            sector.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }

        slotObject.GetComponent<PanelSlots>().ReFresh();
    }
}