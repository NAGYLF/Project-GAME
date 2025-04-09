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

public class ContainerObject : MonoBehaviour
{
    #region DataSynch
    public AdvancedItem ActualData;//ezek alapján vizualizálja es szinkronizálja az itemeket az az ha valami ezt az objektumot staterként megkapja akkor ezt kell megvaltoztatni a szinkronizalashoz
    #endregion

    #region Personal variables
    public ItemSlot[][,] LiveSector;//ez egy mátrox lista amely tartalmazza az összes itemSlot Objectumot

    public SectorData[] StaticSectorDatas;
    #endregion

    [System.Serializable]
    public struct SectorData
    {
        public int Heigth;
        public int Widht;
        public GameObject SectorObject;
    }

    #region Active Slot Handler variables
    //Ezen változók szükségesek ahoz, hogy egy itemet helyezni tudjunk slotokból slotokba
    [HideInInspector] public HashSet<ItemSlot> interactibleSlots;
    [HideInInspector] private bool CanBePlaceble = true;
    //[HideInInspector] public GameObject PlaceableObject;

    [HideInInspector] public List<GameObject> activeSlots;
    [HideInInspector] public bool ChangedFlag = false;

    private Camera mainCam;

    private AdvancedItem IncomingItem = null;
    #endregion

    #region Active Slot Handler
    public IEnumerator Targeting()
    {
        if (ChangedFlag)
        {
            ChangedFlag = false;

            if (activeSlots.Count > 0)
            {
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
                    ItemSlot PrioritySlot = interactibleSlots.FirstOrDefault(slot => slot.MouseOver && slot.PartOfItemObject != null && slot.PartOfItemObject != IncomingItem.SelfGameobject);

                    AdvancedItem InteractiveItem = PrioritySlot?.PartOfItemObject.GetComponent<ItemObject>().ActualData;

                    //ha van interactive item
                    if (PrioritySlot != null)
                    {
                        IncomingItem.AvaiablePlacerMetodes.Clear();
                        //Debug.LogWarning("InteractiveItem");
                        CanBePlaceble = false;

                        if (InventorySystem.CanMergable(InteractiveItem, IncomingItem))
                        {
                            //Debug.LogWarning($"Merge {InteractiveItem.ItemName}     {IncomingItem.ItemName}");
                            InventorySystem.Merge ActionMerge = new(InteractiveItem, IncomingItem);
                            IncomingItem.AvaiablePlacerMetodes.Add(ActionMerge.Execute_Merge);
                            CanBePlaceble = true;
                        }
                        if (InteractiveItem.PartPut_IsPossible(IncomingItem).IsPossible)
                        {
                            //Debug.LogWarning("MergeParts");
                            InventorySystem.MergeParts ActionMergeParts = new(InteractiveItem, IncomingItem);
                            if (ActionMergeParts.IsPossible)
                            {
                                IncomingItem.AvaiablePlacerMetodes.Add(ActionMergeParts.Execute_MergeParts);
                                CanBePlaceble = true;
                            }
                            foreach (var item in ActionMergeParts.NewPosition.NonLiveCoordinates)
                            {
                                interactibleSlots.Add(LiveSector[ActionMergeParts.NewPosition.SectorIndex][item.Height, item.Widht]);
                            }
                        }
                        if (InventorySystem.CanSplitable(InteractiveItem, IncomingItem))
                        {
                            //Debug.LogWarning("Split");
                            InventorySystem.Split ActionSplit = new(IncomingItem, interactibleSlots.ToArray());
                            IncomingItem.AvaiablePlacerMetodes.Add(ActionSplit.Execute_Split);
                            CanBePlaceble = true;
                        }
                        foreach (var item in InteractiveItem.Coordinates)
                        {
                            interactibleSlots.Add(LiveSector[IncomingItem.SectorId][item.Item1, item.Item2]);
                        }
                    }
                    //ha nincs interacti item
                    else
                    {
                        //Debug.LogWarning("No   InteractiveItem");
                        foreach (ItemSlot slot in interactibleSlots)
                        {
                            if (!(slot.PartOfItemObject == null || slot.PartOfItemObject == IncomingItem.SelfGameobject))
                            {
                                //Debug.LogWarning("false 0");
                                CanBePlaceble = false;
                                break;
                            }
                            if (!(slot.SlotType == "" || slot.SlotType.Contains(IncomingItem.ItemType)))
                            {
                                //Debug.LogWarning("false 1");
                                CanBePlaceble = false;
                                break;
                            }
                        }

                        if (!(interactibleSlots.Count == IncomingItem.SizeX * IncomingItem.SizeY || (interactibleSlots.First().IsEquipment && interactibleSlots.Count == 1)))
                        {
                            //Debug.LogWarning("false 2");
                            CanBePlaceble = false;
                        }

                        IncomingItem.AvaiablePlacerMetodes.Clear();

                        if (CanBePlaceble)
                        {
                            if (InventorySystem.CanSplitable(InteractiveItem, IncomingItem) && interactibleSlots.FirstOrDefault(slot=>slot.PartOfItemObject != null) == null)
                            {
                                InventorySystem.Split ActionSplit = new(IncomingItem, interactibleSlots.ToArray());
                                IncomingItem.AvaiablePlacerMetodes.Add(ActionSplit.Execute_Split);
                            }
                            InventorySystem.RePlace ActionRePlace = new(IncomingItem, ActualData, interactibleSlots.ToArray());
                            IncomingItem.AvaiablePlacerMetodes.Add(ActionRePlace.Execute_RePlace);
                        }
                        //else
                        //{
                        //    InventorySystem.RePlace ActionRePlace = new(IncomingItem, IncomingItem.ParentItem, IncomingItem.ItemSlotObjectsRef.ToArray());
                        //    IncomingItem.AvaiablePlacerMetodes.Add(ActionRePlace.Execute_RePlace);
                        //}
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
            else
            {
                HashSet<ItemSlot> interactibleSlots_ = new(interactibleSlots);
                foreach (ItemSlot slot_ in interactibleSlots_)
                {
                    slot_.Deactivation();
                    interactibleSlots.Remove(slot_);
                }

                interactibleSlots_.Clear();

                if (IncomingItem != null)
                {
                    IncomingItem.AvaiablePlacerMetodes.Clear();
                    IncomingItem.AvaiableParentItem = null;
                }
            }
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
            ChangedFlag = true;
        }

        yield return null;
    }
    public void SetDataRoute(AdvancedItem Data)//(ezen eljárás ezen objektum játékbakerülése elõtt zajlik le)    Célja, hogy a gyökérbõl továbbított és egyben ennek az objektumnak szánt adatokat ezen VCP megkapja
    {
        ActualData = Data;
    }
    #endregion

    public void Inicialisation()//az objecktum létrehozásának elsõ pillanatában töltõdik be
    {
        LiveSector = new ItemSlot[StaticSectorDatas.Length][,];


        for (int sector = 0; sector < StaticSectorDatas.Length; sector++)
        {
            SectorData sectorData = StaticSectorDatas[sector];
            ItemSlot[] itemSlots = sectorData.SectorObject.GetComponentsInChildren<ItemSlot>();

            LiveSector[sector] = new ItemSlot[sectorData.Heigth, sectorData.Widht];

            for (int height = 0, index = 0; height < sectorData.Heigth; height++)
            {
                for (int width = 0; width < sectorData.Widht; width++)
                {
                    itemSlots[index].SlotParentItem = ActualData;
                    itemSlots[index].sectorId = sector;
                    itemSlots[index].Coordinate = (height,width);
                    ActualData.Container.Live_Sector[sector][height, width] = itemSlots[index];

                    LiveSector[sector][height, width] = itemSlots[index++];
                }
            }
        }

        interactibleSlots = new HashSet<ItemSlot>();
        activeSlots = new List<GameObject>();

        ActualData.Container.ContainerObject = gameObject;

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
        for (int i = 0; i < ActualData.Container.Items.Count; i++)//létrehozzuk itemObjektumait
        {
            //Debug.Log($"{ActualData.Container.Items[i].ItemName} creating into {ActualData.ItemName}'s container");
            GameObject itemObject = CreatePrefab(AdvancedItem.AdvancedItemObjectParth);
            itemObject.GetComponent<ItemObject>().SetDataRoute(ActualData.Container.Items[i], ActualData);//Létrehozzuk a szikronizálási utat ezen VirtualParentObject és a VirtualChildrenObject között
        }
    }
    private void VisualisationToLootPanel()
    {
        GameObject lootObject = InventoryObjectRef.gameObject.GetComponent<PlayerInventory>().LootPanelObject;
        gameObject.transform.SetParent(lootObject.GetComponent<PanelLoot>().Content.transform, false);
        foreach (SectorData sector in StaticSectorDatas)//beallitjuk a méretarányt
        {
            sector.SectorObject.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }
        //ActualData.ContainerObject = gameObject;
        //ActualData.Container.Live_Sector = gameObject.GetComponent<ContainerObject>().Sectors;
    }
    private void VisualisationToSlotPanel()
    {
        GameObject slotObject = InventoryObjectRef.gameObject.GetComponent<PlayerInventory>().SlotPanelObject;
        gameObject.transform.SetParent(slotObject.GetComponent<PanelSlots>().Content.transform, false);
        foreach (SectorData sector in StaticSectorDatas)//beallitjuk a méretarányt
        {
            sector.SectorObject.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }

        slotObject.GetComponent<PanelSlots>().ReFresh();
    }
}